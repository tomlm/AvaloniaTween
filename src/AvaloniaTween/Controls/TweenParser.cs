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
        public static AnimationResource Parse(string tween, Visual? target = null)
        {
            var animation = new AnimationResource { Selector = "Self" };

            // Split by semicolon for multiple tracks
            var trackStrings = tween.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var trackString in trackStrings)
            {
                var track = ParseTrack(trackString.Trim(), target);
                if (track != null)
                    animation.Tracks.Add(track);
            }

            return animation;
        }

        private static AnimationTrack? ParseTrack(string trackString, Visual? target)
        {
            // Format: "property:from->to@duration ease:easing reset"
            // Examples: 
            //   "opacity:0->1@500ms"
            //   "left:100->200@1s ease:OutCubic"
            //   "opacity:0->1 ease:OutBack reset"
            //   "Canvas.RightProperty:0->200@1s reset"

            var track = new AnimationTrack();
            var parts = trackString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var lowerPart = part.ToLower();
                
                // Check for reset keyword (standalone, without colon)
                if (lowerPart.StartsWith("reset"))
                {
                    if (lowerPart == "reset")   
                        track.RestoreOriginalValue = true;
                    else 
                        track.RestoreOriginalValue = (lowerPart == "reset:true");
                    continue;
                }

                if (part.Contains(':'))
                {
                    var kvp = part.Split(':', 2);
                    var key = kvp[0];
                    var value = kvp[1];
                    var keyLower = key.ToLower();

                    switch (keyLower)
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
                            track.Property = ParseProperty(key, target);
                            ParseValues(value, track, track.Property);
                            break;
                    }
                }
            }

            return track.Property != null ? track : null;
        }

        private static void ParseValues(string value, AnimationTrack track, AvaloniaProperty property)
        {
            // Format: "from->to@duration" or "from->to" or "to"
            var parts = value.Split("@", StringSplitOptions.RemoveEmptyEntries);
            var valuePart = parts[0];

            if (parts.Length > 1)
                track.Duration = ParseTimeSpan(parts[1]);

            if (valuePart.Contains("->"))
            {
                var values = valuePart.Split("->", StringSplitOptions.RemoveEmptyEntries);
                track.From = ParseValue(values[0], property.PropertyType);
                track.To = ParseValue(values[1], property.PropertyType);
            }
            else
            {
                track.To = ParseValue(valuePart, property.PropertyType);
            }
        }

        private static AvaloniaProperty ParseProperty(string propertyName, Visual? target)
        {
            // Check if it's a fully qualified property name like "Canvas.RightProperty"
            if (propertyName.Contains('.'))
            {
                var parts = propertyName.Split('.');
                if (parts.Length == 2)
                {
                    var typeName = parts[0];
                    var propName = parts[1];
                    
                    // Try to find the type in common namespaces
                    var potentialTypes = new[]
                    {
                        $"Avalonia.Controls.{typeName}",
                        $"Avalonia.{typeName}",
                        $"Avalonia.Layout.{typeName}",
                        $"Avalonia.Media.{typeName}",
                        typeName // In case it's already fully qualified
                    };

                    foreach (var fullTypeName in potentialTypes)
                    {
                        var type = Type.GetType($"{fullTypeName}, Avalonia.Controls") 
                            ?? Type.GetType($"{fullTypeName}, Avalonia.Base")
                            ?? Type.GetType($"{fullTypeName}, Avalonia.Visuals");
                        
                        if (type != null)
                        {
                            var propertyField = type.GetField(propName,
                                System.Reflection.BindingFlags.Public |
                                System.Reflection.BindingFlags.Static);

                            if (propertyField != null && propertyField.GetValue(null) is AvaloniaProperty avaloniaProperty)
                                return avaloniaProperty;
                        }
                    }
                }
            }

            var lowerName = propertyName.ToLower();

            // Try common shorthand names first
            AvaloniaProperty? property = null;
            switch (lowerName)
            {
                case "opacity":
                    property = Visual.OpacityProperty;
                    break;
                case "left":
                    property = Canvas.LeftProperty;
                    break;
                case "top":
                    property = Canvas.TopProperty;
                    break;
                case "right":
                    property = Canvas.RightProperty;
                    break;
                case "bottom":
                    property = Canvas.BottomProperty;
                    break;
                case "width":
                    property = Layoutable.WidthProperty;
                    break;
                case "height":
                    property = Layoutable.HeightProperty;
                    break;
                case "minwidth":
                    property = Layoutable.MinWidthProperty;
                    break;
                case "minheight":
                    property = Layoutable.MinHeightProperty;
                    break;
                case "maxwidth":
                    property = Layoutable.MaxWidthProperty;
                    break;
                case "maxheight":
                    property = Layoutable.MaxHeightProperty;
                    break;
                case "margin":
                    property = Layoutable.MarginProperty;
                    break;
                case "angle":
                    property = RotateTransform.AngleProperty;
                    break;
                case "scalex":
                    property = ScaleTransform.ScaleXProperty;
                    break;
                case "scaley":
                    property = ScaleTransform.ScaleYProperty;
                    break;
                case "scale":
                    property = ScaleTransform.ScaleXProperty;
                    break;
                case "x":
                    property = TranslateTransform.XProperty;
                    break;
                case "y":
                    property = TranslateTransform.YProperty;
                    break;
                case "skewx":
                    property = SkewTransform.AngleXProperty;
                    break;
                case "skewy":
                    property = SkewTransform.AngleYProperty;
                    break;
                default:
                    property = null;
                    break;
            }

            if (property != null)
                return property;

            // If we have a target, try to find the property on it
            if (target != null)
            {
                var targetType = target.GetType();

                // Try to find an AvaloniaProperty by name (try different casings)
                var propertyFieldName = ToPascalCase(propertyName) + "Property";
                var propertyField = targetType.GetField(propertyFieldName,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Static |
                    System.Reflection.BindingFlags.FlattenHierarchy);

                if (propertyField != null && propertyField.GetValue(null) is AvaloniaProperty avaloniaProperty)
                    return avaloniaProperty;

                // Try attached properties from common types
                var attachedPropertyTypes = new[]
                {
                    typeof(Canvas),
                    typeof(Grid),
                    typeof(DockPanel),
                    typeof(RelativePanel)
                };

                foreach (var attachedType in attachedPropertyTypes)
                {
                    var attachedField = attachedType.GetField(propertyFieldName,
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.Static);

                    if (attachedField != null && attachedField.GetValue(null) is AvaloniaProperty attachedProperty)
                        return attachedProperty;
                }
            }

            throw new ArgumentException($"Unknown property: {propertyName}");
        }

        private static string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            // Convert first character to uppercase
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        private static object ParseValue(string value, Type propertyType)
        {
            value = value.Trim();

            // Handle Thickness (margin, padding)
            if (propertyType == typeof(Thickness))
            {
                var parts = value.Split(',');
                if (parts.Length == 1 && double.TryParse(parts[0], out var uniform))
                    return new Thickness(uniform);
                if (parts.Length == 2 && double.TryParse(parts[0], out var horizontal) && double.TryParse(parts[1], out var vertical))
                    return new Thickness(horizontal, vertical);
                if (parts.Length == 4 && 
                    double.TryParse(parts[0], out var left) && 
                    double.TryParse(parts[1], out var top) &&
                    double.TryParse(parts[2], out var right) &&
                    double.TryParse(parts[3], out var bottom))
                    return new Thickness(left, top, right, bottom);
            }

            // Handle double/int
            if (propertyType == typeof(double) || propertyType == typeof(int) || propertyType == typeof(float))
            {
                if (double.TryParse(value, out var doubleValue))
                    return propertyType == typeof(int) ? (int)doubleValue : doubleValue;
            }

            // Handle Color
            if (propertyType == typeof(Color))
            {
                return Color.Parse(value);
            }

            // Handle SolidColorBrush
            if (propertyType == typeof(IBrush) || propertyType == typeof(SolidColorBrush))
            {
                return new SolidColorBrush(Color.Parse(value));
            }

            // Default: try double
            if (double.TryParse(value, out var fallbackValue))
                return fallbackValue;

            throw new ArgumentException($"Cannot parse value: {value} for type {propertyType.Name}");
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
