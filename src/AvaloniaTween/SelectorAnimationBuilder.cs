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
                    tasks.Add(RunTrackAsync(target, track, sortedKeyFrames, _cancellationTokenSource.Token));
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task RunTrackAsync(Visual target, PropertyAnimationTrack track, List<KeyFrameDefinition> sortedKeyFrames, CancellationToken cancellationToken)
        {
            // Run each segment sequentially
            for (int i = 0; i < sortedKeyFrames.Count - 1; i++)
            {
                var fromKf = sortedKeyFrames[i];
                var toKf = sortedKeyFrames[i + 1];

                var segmentDuration = toKf.Duration ?? TimeSpan.FromSeconds(1);
                var segmentEasing = toKf.Easing ?? new LinearEasing();

                var animation = new Animation
                {
                    Duration = segmentDuration,
                    Easing = segmentEasing,
                    FillMode = (i == sortedKeyFrames.Count - 2) ? track.FillMode : FillMode.Forward,
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

                await animation.RunAsync(target, cancellationToken);
            }
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}