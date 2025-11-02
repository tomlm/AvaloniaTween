using System;

namespace AvaloniaAnimate
{
    public class KeyFrameBuilder
    {
        private readonly PropertyTrackBuilder _parent;
        private readonly PropertyAnimationTrack _track;
        private readonly double _cue;

        public KeyFrameBuilder(PropertyTrackBuilder parent, PropertyAnimationTrack track, double cue)
        {
            _parent = parent;
            _track = track;
            _cue = cue;
        }

    }
}