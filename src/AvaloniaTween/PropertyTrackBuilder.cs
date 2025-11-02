using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using System;
using System.Threading.Tasks;

namespace AvaloniaTweener
{
    public class PropertyTrackBuilder
    {
        private readonly SelectorAnimationBuilder _parent;
        private readonly PropertyAnimationTrack _track;
        private KeyFrameDefinition? _currentKeyFrame;

        public PropertyTrackBuilder(SelectorAnimationBuilder parent, PropertyAnimationTrack track)
        {
            _parent = parent;
            _track = track;
        }

        // From sets keyframe at 0%
        public PropertyTrackBuilder From<T>(T value)
        {
            // Initialize keyframes list if first time
            _track.KeyFrames ??= new();

            _currentKeyFrame = new KeyFrameDefinition { Cue = 0.0, Value = value };
            _track.KeyFrames.Add(_currentKeyFrame);
            return this;
        }

        // To with duration, defaults to cue 1.0 (100%)
        public PropertyTrackBuilder To<T>(T value, TimeSpan duration)
        {
            return To(value, duration, 1.0);
        }

        // To with cue and duration
        public PropertyTrackBuilder To<T>(T value, TimeSpan duration, double cue)
        {
            if (cue < 0.0 || cue > 1.0)
                throw new ArgumentOutOfRangeException(nameof(cue), "Cue must be between 0.0 and 1.0");

            // Initialize keyframes list if first time
            _track.KeyFrames ??= new();

            _currentKeyFrame = new KeyFrameDefinition
            {
                Cue = cue,
                Value = value,
                Duration = duration
            };
            _track.KeyFrames.Add(_currentKeyFrame);
            return this;
        }

        // Shorthand for From + To
        public PropertyTrackBuilder FromTo<T>(T from, T to, TimeSpan duration)
        {
            return From(from).To(to, duration);
        }

        // Easing applies to the segment leading to the current keyframe
        public PropertyTrackBuilder WithEasing(Easing easing)
        {
            if (_currentKeyFrame != null)
            {
                _currentKeyFrame.Easing = easing;
            }
            else
            {
                // Fallback: applies to whole animation if no keyframes yet
                _track.Easing = easing;
            }
            return this;
        }

        // Delay before animation starts
        public PropertyTrackBuilder WithDelay(TimeSpan delay)
        {
            _track.Delay = delay;
            return this;
        }

        // Speed multiplier
        public PropertyTrackBuilder WithSpeed(double speedMultiplier)
        {
            if (speedMultiplier <= 0)
                throw new ArgumentOutOfRangeException(nameof(speedMultiplier), "Speed multiplier must be greater than 0");
            
            _track.SpeedRatio = speedMultiplier;
            return this;
        }

        // Repeat configuration
        public PropertyTrackBuilder Repeat(int count = -1)
        {
            _track.RepeatCount = count;
            return this;
        }

        public PropertyTrackBuilder Yoyo(bool enabled = true)
        {
            _track.Yoyo = enabled;
            return this;
        }

        // Callbacks
        public PropertyTrackBuilder OnStart(Action callback)
        {
            _track.OnStart = callback;
            return this;
        }

        public PropertyTrackBuilder OnComplete(Action callback)
        {
            _track.OnComplete = callback;
            return this;
        }

        public PropertyTrackBuilder OnUpdate(Action<double> callback)
        {
            _track.OnUpdate = callback;
            return this;
        }

        public PropertyTrackBuilder OnRepeat(Action callback)
        {
            _track.OnRepeat = callback;
            return this;
        }

        // Fill modes
        public PropertyTrackBuilder FillMode(FillMode fillMode)
        {
            _track.FillMode = fillMode;
            return this;
        }

        public PropertyTrackBuilder Reset()
        {
            _track.FillMode = Avalonia.Animation.FillMode.None;
            _track.RestoreOriginalValue = true;
            return this;
        }

        public PropertyTrackBuilder Hold()
        {
            _track.FillMode = Avalonia.Animation.FillMode.Forward;
            return this;
        }

        // Chain to next animation
        public PropertyTrackBuilder Animate<T>(AvaloniaProperty<T> property) => _parent.Animate(property);

        // Get parent builder (for timeline support)
        public SelectorAnimationBuilder Build() => _parent;

        public Task StartAsync() => _parent.StartAsync();
    }
}