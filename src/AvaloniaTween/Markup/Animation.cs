using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;
using AvaloniaTweener.Fluent;
using AvaloniaTweener.Markup;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaTweener.Markup
{
    /// <summary>
    /// Represents a complete animation definition that can be used as a static resource.
    /// Contains one or more Transform elements that define property animations.
    /// </summary>
    public class Animation
    {
        public Animation()
        {
            Transforms = new AvaloniaList<Transform>();
        }

        /// <summary>
        /// Collection of property transforms (animations)
        /// </summary>
        [Content]
        public AvaloniaList<Transform> Transforms { get; set; }

        /// <summary>
        /// Starts the animation on the specified target visual
        /// </summary>
        /// <param name="target">The visual element to animate</param>
        /// <returns>A SelectorAnimationBuilder that can be awaited</returns>
        public SelectorAnimationBuilder Start(Visual target)
        {
            var builder = Tweener.Select(target);
            Apply(builder);
            return builder;
        }

        /// <summary>
        /// Applies this animation configuration to an existing builder
        /// </summary>
        internal void Apply(SelectorAnimationBuilder builder)
        {
            foreach (var transform in Transforms)
            {
                transform.Apply(builder);
            }
        }
    }
}
