using Avalonia.Animation.Easings;
using System;
using System.ComponentModel;

namespace AvaloniaTweener.Markup
{
    /// <summary>
    /// Represents a single animation step with From/To values, duration, and easing.
    /// </summary>
    public class Animate
    {
        /// <summary>
        /// Starting value for the animation (optional, will use current value if not specified)
        /// </summary>
        public object? From { get; set; }

        /// <summary>
        /// Target value for the animation
        /// </summary>
        public object? To { get; set; }

        /// <summary>
        /// Duration of this animation step
        /// </summary>
        [TypeConverter(typeof(TimeSpanTypeConverter))]
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Delay before this animation step starts
        /// </summary>
        [TypeConverter(typeof(TimeSpanTypeConverter))]
        public TimeSpan? Delay { get; set; }

        /// <summary>
        /// Easing function for this animation step
        /// </summary>
        public Easing? Easing { get; set; }

        /// <summary>
        /// Applies this animation step to the property builder
        /// </summary>
        internal void Apply(dynamic propertyBuilder)
        {
            if (propertyBuilder == null)
                throw new ArgumentNullException(nameof(propertyBuilder));

            try
            {
                // Apply From if specified
                if (From != null)
                {
                    propertyBuilder = propertyBuilder.From(From);
                }

                // Apply To with duration
                if (To != null)
                {
                    if (Duration.HasValue && Duration.Value > TimeSpan.Zero)
                    {
                        propertyBuilder = propertyBuilder.To(To, Duration.Value);
                    }
                    else
                    {
                        // Default duration if not specified
                        propertyBuilder = propertyBuilder.To(To, TimeSpan.FromMilliseconds(300));
                    }
                }

                // Apply Easing if specified
                if (Easing != null)
                {
                    propertyBuilder = propertyBuilder.WithEasing(Easing);
                }

                // Apply Delay if specified
                if (Delay.HasValue && Delay.Value > TimeSpan.Zero)
                {
                    propertyBuilder = propertyBuilder.WithDelay(Delay.Value);
                }
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to apply animation. Value type mismatch or invalid property configuration.", ex);
            }
        }
    }

    /// <summary>
    /// Custom TimeSpan type converter to support "1s", "1000ms" format in XAML
    /// </summary>
    public class TimeSpanTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
        {
            if (value is string str)
            {
                str = str.Trim();

                // Handle milliseconds: "500ms" or "1000ms"
                if (str.EndsWith("ms", StringComparison.OrdinalIgnoreCase))
                {
                    var msStr = str.Substring(0, str.Length - 2).Trim();
                    if (double.TryParse(msStr, out var ms))
                    {
                        return TimeSpan.FromMilliseconds(ms);
                    }
                }

                // Handle seconds: "1s" or "1.5s"
                if (str.EndsWith("s", StringComparison.OrdinalIgnoreCase))
                {
                    var secStr = str.Substring(0, str.Length - 1).Trim();
                    if (double.TryParse(secStr, out var sec))
                    {
                        return TimeSpan.FromSeconds(sec);
                    }
                }

                // Try standard TimeSpan parsing as fallback
                if (TimeSpan.TryParse(str, out var timeSpan))
                {
                    return timeSpan;
                }

                throw new FormatException($"Invalid TimeSpan format: '{str}'. Use formats like '1s', '500ms', or '00:00:01'");
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
