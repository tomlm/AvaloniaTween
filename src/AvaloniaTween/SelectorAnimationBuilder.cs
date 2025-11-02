using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaAnimate
{
    public class SelectorAnimationBuilder
    {
        private readonly List<Visual> _targets;
        private readonly List<PropertyAnimationTrack> _tracks = new();
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly List<Task> _runningTasks = new();
        
        // Playback state
        private bool _isPaused;
        private double _progress;
        private TimeSpan _staggerDelay = TimeSpan.Zero;

        public SelectorAnimationBuilder(IEnumerable<Visual> targets)
        {
            _targets = targets.OfType<Visual>().ToList();
        }

        // Properties
        public bool IsPlaying => _runningTasks.Any(t => !t.IsCompleted);
        public bool IsPaused => _isPaused;
        public double Progress => _progress;

        public PropertyTrackBuilder Animate<T>(AvaloniaProperty<T> property)
        {
            var track = new PropertyAnimationTrack { Property = property };
            _tracks.Add(track);
            return new PropertyTrackBuilder(this, track);
        }

        // Batch operations - apply to all tracks
        public SelectorAnimationBuilder WithDelay(TimeSpan delay)
        {
            foreach (var track in _tracks)
            {
                track.Delay = delay;
            }
            return this;
        }

        public SelectorAnimationBuilder WithEasing(Easing easing)
        {
            foreach (var track in _tracks)
            {
                track.Easing = easing;
            }
            return this;
        }

        public SelectorAnimationBuilder WithSpeed(double speedMultiplier)
        {
            foreach (var track in _tracks)
            {
                track.SpeedRatio = speedMultiplier;
            }
            return this;
        }

        public SelectorAnimationBuilder Repeat(int count = -1)
        {
            foreach (var track in _tracks)
            {
                track.RepeatCount = count;
            }
            return this;
        }

        public SelectorAnimationBuilder Yoyo(bool enabled = true)
        {
            foreach (var track in _tracks)
            {
                track.Yoyo = enabled;
            }
            return this;
        }

        // Stagger - add incremental delay between targets
        public SelectorAnimationBuilder Stagger(TimeSpan delay)
        {
            _staggerDelay = delay;
            return this;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _runningTasks.Clear();
            _progress = 0;
            _isPaused = false;

            var tasks = new List<Task>();
            var targetIndex = 0;

            foreach (var target in _targets)
            {
                var staggerDelay = _staggerDelay * targetIndex;
                
                // Group tracks - run different properties in parallel
                foreach (var track in _tracks)
                {
                    if (!track.UseKeyFrames)
                    {
                        throw new InvalidOperationException("Animation track must have keyframes. Use .From() and .To() to define the animation.");
                    }

                    // Sort keyframes by cue
                    var sortedKeyFrames = track.KeyFrames!.OrderBy(kf => kf.Cue).ToList();

                    // Run segments sequentially for this track
                    tasks.Add(RunTrackAsync(target, track, sortedKeyFrames, staggerDelay, _cancellationTokenSource.Token));
                }

                targetIndex++;
            }

            _runningTasks.AddRange(tasks);
            await Task.WhenAll(tasks);
        }

        private async Task RunTrackAsync(
            Visual target, 
            PropertyAnimationTrack track, 
            List<KeyFrameDefinition> sortedKeyFrames, 
            TimeSpan staggerDelay,
            CancellationToken cancellationToken)
        {
            try
            {
                // Apply stagger delay
                if (staggerDelay > TimeSpan.Zero)
                {
                    await Task.Delay(staggerDelay, cancellationToken);
                }

                // Apply track delay
                if (track.Delay > TimeSpan.Zero)
                {
                    await Task.Delay(track.Delay, cancellationToken);
                }

                var iteration = 0;
                var repeatCount = track.RepeatCount;

                while (iteration < repeatCount || repeatCount == -1)
                {
                    if (iteration == 0)
                    {
                        track.OnStart?.Invoke();
                    }
                    else
                    {
                        track.OnRepeat?.Invoke();
                    }

                    var shouldReverse = track.Yoyo && iteration % 2 == 1;
                    var keyFramesToUse = shouldReverse 
                        ? sortedKeyFrames.AsEnumerable().Reverse().ToList() 
                        : sortedKeyFrames;

                    // Run each segment sequentially
                    for (int i = 0; i < keyFramesToUse.Count - 1; i++)
                    {
                        var fromKf = keyFramesToUse[i];
                        var toKf = keyFramesToUse[i + 1];

                        var segmentDuration = toKf.Duration ?? TimeSpan.FromSeconds(1);
                        
                        // Apply speed ratio
                        if (track.SpeedRatio != 1.0)
                        {
                            segmentDuration = TimeSpan.FromMilliseconds(segmentDuration.TotalMilliseconds / track.SpeedRatio);
                        }

                        var segmentEasing = toKf.Easing ?? new LinearEasing();

                        var isLastSegment = i == keyFramesToUse.Count - 2;
                        var isLastIteration = iteration == repeatCount - 1 && repeatCount != -1;

                        var animation = new Animation
                        {
                            Duration = segmentDuration,
                            Easing = segmentEasing,
                            FillMode = (isLastSegment && isLastIteration) ? track.FillMode : FillMode.Forward,
                            Children =
                            {
                                new KeyFrame
                                {
                                    Cue = new Cue(0d),
                                    Setters = { new Setter(track.Property, fromKf.Value) }
                                },
                                new KeyFrame
                                {
                                    Cue = new Cue(1d),
                                    Setters = { new Setter(track.Property, toKf.Value) }
                                }
                            }
                        };

                        // Run animation with progress tracking
                        await RunAnimationWithProgressAsync(animation, target, track, cancellationToken);
                    }

                    iteration++;

                    if (cancellationToken.IsCancellationRequested)
                        break;
                }

                track.OnComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                // Animation was cancelled
            }
        }

        private async Task RunAnimationWithProgressAsync(
            Animation animation, 
            Visual target, 
            PropertyAnimationTrack track,
            CancellationToken cancellationToken)
        {
            if (track.OnUpdate != null)
            {
                // Run animation with progress tracking
                var stopwatch = Stopwatch.StartNew();
                var animationTask = animation.RunAsync(target, cancellationToken);
                
                while (!animationTask.IsCompleted)
                {
                    var elapsed = stopwatch.Elapsed;
                    var progress = Math.Min(1.0, elapsed.TotalMilliseconds / animation.Duration.TotalMilliseconds);
                    track.OnUpdate(progress);
                    _progress = progress;
                    
                    await Task.Delay(16, cancellationToken); // ~60fps updates
                }
                
                track.OnUpdate(1.0);
                await animationTask;
            }
            else
            {
                await animation.RunAsync(target, cancellationToken);
            }
        }

        // Playback control
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _isPaused = false;
        }

        public void Pause()
        {
            _isPaused = true;
            // Note: Avalonia doesn't have built-in pause/resume
            // This is a simplified implementation
            _cancellationTokenSource?.Cancel();
        }

        public void Resume()
        {
            if (_isPaused)
            {
                _isPaused = false;
                // Note: Full resume would require storing state
                // This is a placeholder for future implementation
            }
        }
    }
}