using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Metadata;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AvaloniaTweener.Controls
{
    /// <summary>
    /// Defines an animation that can be used as a XAML resource
    /// </summary>
    public class AnimationResource
    {
        public AnimationResource()
        {
            Tracks = new ObservableCollection<AnimationTrack>();
        }

        /// <summary>
        /// The name to register this animation with (optional)
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Selector for target elements
        /// </summary>
        public string? Selector { get; set; }

        /// <summary>
        /// Collection of property animations
        /// </summary>
        [Content]
        public ObservableCollection<AnimationTrack> Tracks { get; set; }

        /// <summary>
        /// Global delay applied to all tracks
        /// </summary>
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Global speed multiplier
        /// </summary>
        public double Speed { get; set; } = 1.0;

        /// <summary>
        /// Stagger delay between targets
        /// </summary>
        public TimeSpan Stagger { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Global repeat count (-1 for infinite)
        /// </summary>
        public int Repeat { get; set; } = 1;

        /// <summary>
        /// Enable yoyo effect
        /// </summary>
        public bool Yoyo { get; set; }

        /// <summary>
        /// Register this animation with the AnimatorRegistry
        /// </summary>
        public void Register()
        {
            if (string.IsNullOrEmpty(Name))
                throw new InvalidOperationException("Animation must have a Name to be registered.");

            Animator.Register(Name, builder => Apply(builder));
        }

        /// <summary>
        /// Apply this animation configuration to a builder
        /// </summary>
        public void Apply(SelectorAnimationBuilder builder)
        {
            foreach (var track in Tracks)
            {
                // Extract the generic type argument from AvaloniaProperty<T>
                var propertyType = track.Property.GetType();
                var genericArgs = propertyType.BaseType?.GetGenericArguments();
                
                if (genericArgs == null || genericArgs.Length == 0)
                {
                    // Try getting from the property type itself if it's directly generic
                    genericArgs = propertyType.GetGenericArguments();
                }
                
                if (genericArgs == null || genericArgs.Length == 0)
                {
                    // Last resort: use PropertyType from the AvaloniaProperty
                    genericArgs = new[] { track.Property.PropertyType };
                }
                
                var valueType = genericArgs[0];
                
                // Use reflection to call the generic Animate method
                var animateMethod = typeof(SelectorAnimationBuilder)
                    .GetMethods()
                    .First(m => m.Name == "Animate" && m.IsGenericMethod);
                
                var genericMethod = animateMethod.MakeGenericMethod(valueType);
                dynamic propertyBuilder = genericMethod.Invoke(builder, new object[] { track.Property });

                if (propertyBuilder == null)
                    continue;

                try
                {
                    // Now use dynamic to call methods without reflection
                    if (track.From != null)
                        propertyBuilder = propertyBuilder.From(track.From);

                    if (track.To != null)
                    {
                        if (track.Duration.HasValue)
                            propertyBuilder = propertyBuilder.To(track.To, track.Duration.Value);
                        else
                            propertyBuilder = propertyBuilder.To(track.To);
                    }

                    if (track.Easing != null)
                        propertyBuilder = propertyBuilder.WithEasing(track.Easing);

                    if (track.Delay > TimeSpan.Zero)
                        propertyBuilder = propertyBuilder.WithDelay(track.Delay);

                    if (track.Repeat > 1)
                        propertyBuilder = propertyBuilder.Repeat(track.Repeat);

                    if (track.RestoreOriginalValue)
                        propertyBuilder = propertyBuilder.Reset();

                    propertyBuilder.Build();
                }
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to apply animation track for property '{track.Property.Name}'. " +
                        $"Value type mismatch - expected {valueType.Name}.", ex);
                }
            }

            // Apply global settings
            if (Delay > TimeSpan.Zero)
                builder.WithDelay(Delay);

            if (Speed != 1.0)
                builder.WithSpeed(Speed);

            if (Stagger > TimeSpan.Zero)
                builder.Stagger(Stagger);

            if (Repeat != 1)
                builder.Repeat(Repeat);

            if (Yoyo)
                builder.Yoyo();
        }

        /// <summary>
        /// Start the animation on the specified root
        /// </summary>
        public SelectorAnimationBuilder Start(Visual root)
        {
            if (string.IsNullOrEmpty(Selector))
                throw new InvalidOperationException("Animation must have a Selector to start.");

            var builder = Animator.Select(Selector, root);
            Apply(builder);
            return builder;
        }
    }

    /// <summary>
    /// Defines a single property animation track
    /// </summary>
    public class AnimationTrack
    {
        /// <summary>
        /// The property to animate
        /// </summary>
        public AvaloniaProperty Property { get; set; } = null!;

        /// <summary>
        /// Starting value
        /// </summary>
        public object? From { get; set; }

        /// <summary>
        /// Ending value
        /// </summary>
        public object? To { get; set; }

        /// <summary>
        /// Duration of this track
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Easing function
        /// </summary>
        public Easing? Easing { get; set; }

        /// <summary>
        /// Delay before starting this track
        /// </summary>
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Repeat count for this track
        /// </summary>
        public int Repeat { get; set; } = 1;

        /// <summary>
        /// Whether to restore the original value after animation completes
        /// </summary>
        public bool RestoreOriginalValue { get; set; }
    }
}
