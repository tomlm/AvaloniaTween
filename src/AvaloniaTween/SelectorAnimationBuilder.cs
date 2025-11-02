using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
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

        public SelectorAnimationBuilder(IEnumerable<Visual> targets)
        {
            _targets = targets.OfType<Visual>().ToList();
        }

        public PropertyTrackBuilder Animate<T>(AvaloniaProperty<T> property)
        {
            var track = new PropertyAnimationTrack { Property = property };
            _tracks.Add(track);
            return new PropertyTrackBuilder(this, track);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var tasks = new List<Task>();

            foreach (var target in _targets)
            {
                foreach (var track in _tracks)
                {
                    if (!track.UseKeyFrames)
                    {
                        throw new InvalidOperationException("Animation track must have keyframes. Use .From() and .To() to define the animation.");
                    }

                    // Sort keyframes by cue
                    var sortedKeyFrames = track.KeyFrames!.OrderBy(kf => kf.Cue).ToList();
                    
                    // Calculate total duration from segments
                    var totalDuration = sortedKeyFrames
                        .Where(kf => kf.Duration.HasValue)
                        .Select(kf => kf.Duration!.Value)
                        .Aggregate(TimeSpan.Zero, (sum, d) => sum + d);

                    if (totalDuration == TimeSpan.Zero)
                        totalDuration = track.Duration; // Fallback to track duration

                    var animation = new Animation
                    {
                        Duration = totalDuration,
                        FillMode = track.FillMode
                    };

                    // Add keyframes
                    foreach (var kf in sortedKeyFrames)
                    {
                        animation.Children.Add(new KeyFrame
                        {
                            Cue = new Cue(kf.Cue),
                            Setters = { new Setter(track.Property, kf.Value) }
                        });
                    }

                    tasks.Add(animation.RunAsync(target, _cancellationTokenSource.Token));
                }
            }

            await Task.WhenAll(tasks);
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}