using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using System;

namespace AvaloniaAnimate
{
    public class PropertyAnimationTrack
    {
        public AvaloniaProperty Property = null!;
        public object? From;
        public object? To;
        public TimeSpan Duration = TimeSpan.FromSeconds(1);
        public Easing? Easing = new LinearEasing();
        public bool HasFrom;
        public FillMode FillMode = FillMode.Forward;
    }
}