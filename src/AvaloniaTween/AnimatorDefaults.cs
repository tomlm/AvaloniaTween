using Avalonia.Animation.Easings;
using System;

namespace AvaloniaAnimate
{
    public static class AnimatorDefaults
    {
        public static TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(1);
        public static Easing Easing { get; set; } = new CubicEaseInOut();
        public static double SpeedRatio { get; set; } = 1.0;
    }
}
