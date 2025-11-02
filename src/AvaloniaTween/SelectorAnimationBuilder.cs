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
                    var animation = new Animation
                    {
                        Duration = track.Duration,
                        Easing = track.Easing ?? new LinearEasing(),
                        Children =
                        {
                            new KeyFrame
                            {
                                Cue = new Cue(0d),
                                Setters = {
                                    new Setter(track.Property, track.HasFrom ? track.From : target.GetValue(track.Property))
                                }
                            },
                            new KeyFrame
                            {
                                Cue = new Cue(1d),
                                Setters = {
                                    new Setter(track.Property, track.To)
                                }
                            }
                        }
                    };

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