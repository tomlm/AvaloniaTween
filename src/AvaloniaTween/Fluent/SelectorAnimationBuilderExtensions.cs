using Avalonia;
using System;
using System.Collections.Generic;

namespace AvaloniaTweener.Fluent
{
   
    // Extension method for playing named animations
    public static class SelectorAnimationBuilderExtensions
    {
        public static SelectorAnimationBuilder Play(this SelectorAnimationBuilder builder, string name)
        {
            if (AnimatorRegistry.TryGet(name, out var configure))
            {
                configure?.Invoke(builder);
                return builder;
            }
            
            throw new InvalidOperationException($"Animation '{name}' is not registered. Use Tweener.Register() first.");
        }
    }
}