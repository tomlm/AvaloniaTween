using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Linq;

namespace AvaloniaTweener.Controls
{
    /// <summary>
    /// Parses compact tween syntax: "opacity:0->1@500ms ease:OutCubic; left:100->200@1s"
    /// </summary>
    public static class TweenParser
    {
        public static AnimationResource Parse(string tween)
        {
            var animation = new AnimationResource { Selector = "Self" };

            // Split by semicolon for multiple tracks
            var trackStrings = tween.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var trackString in trackStrings)
            {
                var track = ParseTrack(trackString.Trim());
                if (track != null)
                    animation.Tracks.Add(track);
            }

            return animation;
        }

        private static AnimationTrack? ParseTrack(string trackString)
        {
            // Format: "property:from->to@duration ease:easing"
            // Examples: 
            //   "opacity:0->1@500ms"
            //   "left:100->200@1s ease:OutCubic"
            //   "opacity:0->1 ease:OutBack"

            var track = new AnimationTrack();
            var parts = trackString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                if (part.Contains(':'))
                {
                    var kvp = part.Split(':', 2);
                    var key = kvp[0].ToLower();
                    var value = kvp[1];

                    switch (key)
                    {
                        case "ease":
                        case "easing":
                            track.Easing = ParseEasing(value);
                            break;
                        case "delay":
                            track.Delay = ParseTimeSpan(value);
                            break;
                        case "repeat":
                            track.Repeat = int.Parse(value);
                            break;
                        default:
                            // Assume it's a property animation
                            track.Property = ParseProperty(key);
                            ParseValues(value, track);
                            break;
                    }
                }
            }

            return track.Property != null ? track : null;
        }

        private static void ParseValues(string value, AnimationTrack track)
        {
            // Format: "from->to@duration" or "from->to" or "to"
            var parts = value.Split("@", StringSplitOptions.RemoveEmptyEntries);
            var valuePart = parts[0];

            if (parts.Length > 1)
                track.Duration = ParseTimeSpan(parts[1]);

            if (valuePart.Contains("->"))
            {
                var values = valuePart.Split("->", StringSplitOptions.RemoveEmptyEntries);
                track.From = ParseValue(values[0]);
                track.To = ParseValue(values[1]);
            }
            else
            {
                track.To = ParseValue(valuePart);
            }
        }

        private static AvaloniaProperty ParseProperty(string propertyName)
        {
            // Map common property names
            return propertyName.ToLower() switch
            {
                "opacity" => Visual.OpacityProperty,
                "left" => Canvas.LeftProperty,
                "top" => Canvas.TopProperty,
                "width" => Layoutable.WidthProperty,
                "height" => Layoutable.HeightProperty,
                "angle" => RotateTransform.AngleProperty,
                "scalex" => ScaleTransform.ScaleXProperty,
                "scaley" => ScaleTransform.ScaleYProperty,
                "scale" => ScaleTransform.ScaleXProperty, // Will need special handling for both X and Y
                "x" => TranslateTransform.XProperty,
                "y" => TranslateTransform.YProperty,
                _ => throw new ArgumentException($"Unknown property: {propertyName}")
            };
        }

        private static object ParseValue(string value)
        {
            value = value.Trim();
            
            if (double.TryParse(value, out var doubleValue))
                return doubleValue;

            throw new ArgumentException($"Cannot parse value: {value}");
        }

        private static TimeSpan ParseTimeSpan(string value)
        {
            value = value.Trim().ToLower();

            if (value.EndsWith("ms"))
                return TimeSpan.FromMilliseconds(double.Parse(value[..^2]));

            if (value.EndsWith("s"))
                return TimeSpan.FromSeconds(double.Parse(value[..^1]));

            // Default to seconds
            return TimeSpan.FromSeconds(double.Parse(value));
        }

        private static Easing ParseEasing(string value)
        {
            return value.ToLower() switch
            {
                "linear" => new LinearEasing(),
                "inback" or "backin" => new BackEaseIn(),
                "outback" or "backout" => new BackEaseOut(),
                "inoutback" or "backinout" => new BackEaseInOut(),
                "inbounce" or "bouncein" => new BounceEaseIn(),
                "outbounce" or "bounceout" => new BounceEaseOut(),
                "inoutbounce" or "bounceinout" => new BounceEaseInOut(),
                "incubic" or "cubicin" => new CubicEaseIn(),
                "outcubic" or "cubicout" => new CubicEaseOut(),
                "inoutcubic" or "cubicinout" => new CubicEaseInOut(),
                "inelastic" or "elasticin" => new ElasticEaseIn(),
                "outelastic" or "elasticout" => new ElasticEaseOut(),
                "inoutelastic" or "elasticinout" => new ElasticEaseInOut(),
                "inquad" or "quadin" => new QuadraticEaseIn(),
                "outquad" or "quadout" => new QuadraticEaseOut(),
                "inoutquad" or "quadinout" => new QuadraticEaseInOut(),
                "inexpo" or "expoin" => new ExponentialEaseIn(),
                "outexpo" or "expoout" => new ExponentialEaseOut(),
                "inoutexpo" or "expoinout" => new ExponentialEaseInOut(),
                "incirc" or "circin" => new CircularEaseIn(),
                "outcirc" or "circout" => new CircularEaseOut(),
                "inoutcirc" or "circinout" => new CircularEaseInOut(),
                "insine" or "sinein" => new SineEaseIn(),
                "outsine" or "sineout" => new SineEaseOut(),
                "inoutsine" or "sineinout" => new SineEaseInOut(),
                _ => new LinearEasing()
            };
        }
    }
}
