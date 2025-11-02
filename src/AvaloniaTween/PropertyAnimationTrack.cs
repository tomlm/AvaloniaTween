using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using System;
using System.Collections.Generic;

namespace AvaloniaTweener
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

        // New properties
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;
        public int RepeatCount { get; set; } = 1; // 1 = play once, -1 = infinite
        public bool Yoyo { get; set; } = false;
        public double SpeedRatio { get; set; } = 1.0;

        // Callbacks
        public Action? OnStart { get; set; }
        public Action? OnComplete { get; set; }
        public Action<double>? OnUpdate { get; set; } // progress 0-1
        public Action? OnRepeat { get; set; }
    }

    public class KeyFrameDefinition
    {
        public double Cue { get; set; }
        public object? Value { get; set; }
        public TimeSpan? Duration { get; set; }  // Duration to reach this keyframe
        public Easing? Easing { get; set; }      // Easing for segment to this keyframe
    }
}