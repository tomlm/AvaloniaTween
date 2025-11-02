using Avalonia;
using System;
using System.Collections.Generic;

namespace AvaloniaTweener
{
    public static class Animator
    {
        public static SelectorAnimationBuilder Select(string selector, Visual root)
        {
            var targets = SelectorResolver.Resolve(selector, root);
            return new SelectorAnimationBuilder(targets);
        }

        public static SelectorAnimationBuilder Select(Visual target)
        {
            var targets = new List<Visual> { target };
            return new SelectorAnimationBuilder(targets);
        }

        public static void Register(string name, Action<SelectorAnimationBuilder> configure)
        {
            AnimatorRegistry.Register(name, configure);
        }

        public static void Unregister(string name)
        {
            AnimatorRegistry.Unregister(name);
        }

        public static Timeline CreateTimeline()
        {
            return new Timeline();
        }
    }

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
            
            throw new InvalidOperationException($"Animation '{name}' is not registered. Use Animator.Register() first.");
        }
    }
}