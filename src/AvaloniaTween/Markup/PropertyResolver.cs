using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AvaloniaTweener.Markup
{
    /// <summary>
    /// Resolves property names like "TranslateTransform.YProperty" or "Button.BackgroundProperty" 
    /// to their corresponding AvaloniaProperty instances.
    /// </summary>
    internal static class PropertyResolver
    {
        private static readonly Dictionary<string, AvaloniaProperty> _propertyCache = new();

        // Common property mappings for convenience
        private static readonly Dictionary<string, AvaloniaProperty> _shortcuts = new()
        {
            // Transform properties
            { "translatetransform.x", TranslateTransform.XProperty },
            { "translatetransform.xproperty", TranslateTransform.XProperty },
            { "translatetransform.y", TranslateTransform.YProperty },
            { "translatetransform.yproperty", TranslateTransform.YProperty },
            { "scaletransform.scalex", ScaleTransform.ScaleXProperty },
            { "scaletransform.scalexproperty", ScaleTransform.ScaleXProperty },
            { "scaletransform.scaley", ScaleTransform.ScaleYProperty },
            { "scaletransform.scaleyproperty", ScaleTransform.ScaleYProperty },
            { "rotatetransform.angle", RotateTransform.AngleProperty },
            { "rotatetransform.angleproperty", RotateTransform.AngleProperty },

            // Common control properties
            { "button.background", Button.BackgroundProperty },
            { "button.backgroundproperty", Button.BackgroundProperty },
            { "button.foreground", Button.ForegroundProperty },
            { "button.foregroundproperty", Button.ForegroundProperty },
            { "control.opacity", Visual.OpacityProperty },
            { "visual.opacity", Visual.OpacityProperty },
            { "visual.opacityproperty", Visual.OpacityProperty },
            { "layoutable.width", Layoutable.WidthProperty },
            { "layoutable.widthproperty", Layoutable.WidthProperty },
            { "layoutable.height", Layoutable.HeightProperty },
            { "layoutable.heightproperty", Layoutable.HeightProperty },
            { "control.margin", Decorator.PaddingProperty },
            { "button.margin", Button.MarginProperty },
            { "button.marginproperty", Button.MarginProperty },
            { "canvas.left", Canvas.LeftProperty },
            { "canvas.leftproperty", Canvas.LeftProperty },
            { "canvas.top", Canvas.TopProperty },
            { "canvas.topproperty", Canvas.TopProperty },
        };

        public static AvaloniaProperty? Resolve(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return null;

            var normalizedName = propertyName.Trim().ToLowerInvariant();

            // Check cache first
            if (_propertyCache.TryGetValue(normalizedName, out var cachedProperty))
                return cachedProperty;

            // Check shortcuts
            if (_shortcuts.TryGetValue(normalizedName, out var shortcutProperty))
            {
                _propertyCache[normalizedName] = shortcutProperty;
                return shortcutProperty;
            }

            // Parse format "TypeName.PropertyName" or "TypeName.PropertyNameProperty"
            var parts = propertyName.Split('.');
            if (parts.Length != 2)
                throw new ArgumentException($"Property name must be in format 'TypeName.PropertyName' or 'TypeName.PropertyNameProperty'. Got: '{propertyName}'");

            var typeName = parts[0].Trim();
            var propName = parts[1].Trim();

            // Ensure property name ends with "Property"
            if (!propName.EndsWith("Property", StringComparison.OrdinalIgnoreCase))
            {
                propName += "Property";
            }

            // Try to resolve the type and property
            var property = FindProperty(typeName, propName);

            if (property != null)
            {
                _propertyCache[normalizedName] = property;
            }

            return property;
        }

        private static AvaloniaProperty? FindProperty(string typeName, string propertyName)
        {
            // Search in common Avalonia assemblies
            var assemblies = new[]
            {
                typeof(Control).Assembly,        // Avalonia.Controls
                typeof(Visual).Assembly,         // Avalonia.Base/Avalonia.Visuals
                typeof(TranslateTransform).Assembly, // Avalonia.Media
            };

            foreach (var assembly in assemblies)
            {
                // Try exact type name first
                var type = assembly.GetTypes().FirstOrDefault(t => 
                    t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase) ||
                    t.FullName?.EndsWith("." + typeName, StringComparison.OrdinalIgnoreCase) == true);

                if (type != null)
                {
                    var field = type.GetField(propertyName, BindingFlags.Public | BindingFlags.Static);
                    if (field?.GetValue(null) is AvaloniaProperty property)
                    {
                        return property;
                    }
                }
            }

            // Last resort: search all types in the assemblies
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                    {
                        var field = type.GetField(propertyName, BindingFlags.Public | BindingFlags.Static);
                        if (field?.GetValue(null) is AvaloniaProperty property)
                        {
                            return property;
                        }
                    }
                }
            }

            throw new ArgumentException($"Could not find property '{propertyName}' on type '{typeName}'");
        }
    }
}
