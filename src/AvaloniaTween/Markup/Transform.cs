using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Collections;
using Avalonia.Metadata;
using AvaloniaTweener.Fluent;
using System;
using System.ComponentModel;
using System.Linq;

namespace AvaloniaTweener.Markup
{
    /// <summary>
    /// Represents a property animation track with one or more animation steps.
    /// </summary>
    public class Transform
    {
        public Transform()
        {
            Animates = new AvaloniaList<Animate>();
        }

        /// <summary>
        /// The property to animate (e.g., "TranslateTransform.YProperty", "Button.BackgroundProperty")
        /// Can be set as a string for convenience or as an AvaloniaProperty
        /// </summary>
        public string? PropertyName { get; set; }

        /// <summary>
        /// The AvaloniaProperty to animate
        /// </summary>
        public AvaloniaProperty? Property { get; set; }

        /// <summary>
        /// Whether to reset the property to its original value after animation completes
        /// </summary>
        [DefaultValue(false)]
        public bool Reset { get; set; }

        /// <summary>
        /// Global delay before this transform starts
        /// </summary>
        [TypeConverter(typeof(TimeSpanTypeConverter))]
        public TimeSpan? Delay { get; set; }

        /// <summary>
        /// Collection of animation steps
        /// </summary>
        [Content]
        public AvaloniaList<Animate> Animates { get; set; }

        /// <summary>
        /// Applies this transform to the animation builder
        /// </summary>
        internal void Apply(SelectorAnimationBuilder builder)
        {
            var property = ResolveProperty();
            if (property == null)
                throw new InvalidOperationException("Property must be specified for Transform");

            // Use reflection to call the generic Animate method
            var animateMethod = typeof(SelectorAnimationBuilder)
                .GetMethods()
                .FirstOrDefault(m => m.Name == "Animate" && m.IsGenericMethod);

            if (animateMethod == null)
                throw new InvalidOperationException("Could not find Animate method on SelectorAnimationBuilder");

            var genericMethod = animateMethod.MakeGenericMethod(property.PropertyType);
            dynamic propertyBuilder = genericMethod.Invoke(builder, new object[] { property });

            if (propertyBuilder == null)
                throw new InvalidOperationException($"Failed to create property builder for {property.Name}");

            // Apply global delay if specified
            if (Delay.HasValue && Delay.Value > TimeSpan.Zero)
            {
                propertyBuilder = propertyBuilder.WithDelay(Delay.Value);
            }

            // Apply each animation step
            foreach (var animate in Animates)
            {
                animate.Apply(propertyBuilder);
            }

            // Apply reset if specified
            if (Reset)
            {
                propertyBuilder = propertyBuilder.Reset();
            }

            // Build the track
            propertyBuilder.Build();
        }

        private AvaloniaProperty? ResolveProperty()
        {
            if (Property != null)
                return Property;

            if (string.IsNullOrEmpty(PropertyName))
                return null;

            // Parse property name like "TranslateTransform.YProperty" or "Button.BackgroundProperty"
            return PropertyResolver.Resolve(PropertyName);
        }
    }
}
