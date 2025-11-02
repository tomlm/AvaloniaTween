using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using System;
using System.Collections.Generic;

namespace AvaloniaAnimate
{
    public class PropertyAnimationTrack
    {
        public AvaloniaProperty Property = null!;
        public TimeSpan Duration = TimeSpan.FromSeconds(1);
        public Easing? Easing = new LinearEasing();
        public FillMode FillMode = FillMode.Forward;
        
        // Support for explicit keyframes
        public List<KeyFrameDefinition>? KeyFrames { get; set; }
        public bool UseKeyFrames => KeyFrames != null && KeyFrames.Count > 0;
    }

    public class KeyFrameDefinition
    {
        public double Cue { get; set; }
        public object? Value { get; set; }
        public TimeSpan? Duration { get; set; }  // Duration to reach this keyframe
        public Easing? Easing { get; set; }      // Easing for segment to this keyframe
    }
}