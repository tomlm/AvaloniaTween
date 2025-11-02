using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using System;
using System.Threading.Tasks;

namespace AvaloniaAnimate
{
    public class PropertyTrackBuilder
    {
        private readonly SelectorAnimationBuilder _parent;
        private readonly PropertyAnimationTrack _track;

        public PropertyTrackBuilder(SelectorAnimationBuilder parent, PropertyAnimationTrack track)
        {
            _parent = parent;
            _track = track;
        }

        public PropertyTrackBuilder From<T>(T value)
        {
            _track.From = value;
            _track.HasFrom = true;
            return this;
        }

        public PropertyTrackBuilder To<T>(T value)
        {
            _track.To = value;
            return this;
        }

        public PropertyTrackBuilder Duration(TimeSpan duration)
        {
            _track.Duration = duration;
            return this;
        }

        public PropertyTrackBuilder Ease(Easing easing)
        {
            _track.Easing = easing;
            return this;
        }

        public PropertyTrackBuilder FillMode(FillMode fillMode)
        {
            _track.FillMode = fillMode;
            return this;
        }

        public PropertyTrackBuilder Reset()
        {
            _track.FillMode = Avalonia.Animation.FillMode.None;
            return this;
        }

        public PropertyTrackBuilder Animate<T>(AvaloniaProperty<T> property) => _parent.Animate(property);

        public Task StartAsync() => _parent.StartAsync();
    }
}