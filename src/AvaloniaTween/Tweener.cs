using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;

namespace AvaloniaTweener
{
    /// <summary>
    /// Attached properties for applying animations in XAML
    /// </summary>
    public static class Tweener
    {
        /// <summary>
        /// Attach an AnimationResource to be played on load
        /// </summary>
        public static readonly AttachedProperty<AnimationResource?> OnLoadResourceProperty =
            AvaloniaProperty.RegisterAttached<Control, AnimationResource?>("OnLoadResource", typeof(Tweener));

        /// <summary>
        /// Play a named animation on load
        /// </summary>
        public static readonly AttachedProperty<string?> OnLoadProperty =
            AvaloniaProperty.RegisterAttached<Control, string?>("OnLoad", typeof(Tweener));

        /// <summary>
        /// Apply a tween string on load (e.g., "opacity:0->1@500ms")
        /// </summary>
        public static readonly AttachedProperty<string?> AnimateProperty =
            AvaloniaProperty.RegisterAttached<Control, string?>("Animate", typeof(Tweener));

        /// <summary>
        /// Apply a tween string on click (e.g., "opacity:0->1@500ms")
        /// </summary>
        public static readonly AttachedProperty<string?> OnClickProperty =
            AvaloniaProperty.RegisterAttached<Control, string?>("OnClick", typeof(Tweener));

        /// <summary>
        /// Attach an event name to trigger animation
        /// </summary>
        public static readonly AttachedProperty<string?> OnProperty =
            AvaloniaProperty.RegisterAttached<Control, string?>("On", typeof(Tweener));

        /// <summary>
        /// The animation resource to use with the On event
        /// </summary>
        public static readonly AttachedProperty<AnimationResource?> AnimationProperty =
            AvaloniaProperty.RegisterAttached<Control, AnimationResource?>("Animation", typeof(Tweener));

        static Tweener()
        {
            OnLoadResourceProperty.Changed.AddClassHandler<Control>(OnLoadResourceChanged);
            OnLoadProperty.Changed.AddClassHandler<Control>(OnLoadChanged);
            AnimateProperty.Changed.AddClassHandler<Control>(OnAnimateChanged);
            OnClickProperty.Changed.AddClassHandler<Control>(OnClickChanged);
            OnProperty.Changed.AddClassHandler<Control>(OnEventChanged);
        }

        // OnLoadResource property accessors
        public static void SetOnLoadResource(Control control, AnimationResource? value) => 
            control.SetValue(OnLoadResourceProperty, value);
        public static AnimationResource? GetOnLoadResource(Control control) => 
            control.GetValue(OnLoadResourceProperty);

        // OnLoad property accessors
        public static void SetOnLoad(Control control, string? value) => 
            control.SetValue(OnLoadProperty, value);
        public static string? GetOnLoad(Control control) => 
            control.GetValue(OnLoadProperty);

        // Animate property accessors
        public static void SetAnimate(Control control, string? value) => 
            control.SetValue(AnimateProperty, value);
        public static string? GetAnimate(Control control) => 
            control.GetValue(AnimateProperty);

        // OnClick property accessors
        public static void SetOnClick(Control control, string? value) => 
            control.SetValue(OnClickProperty, value);
        public static string? GetOnClick(Control control) => 
            control.GetValue(OnClickProperty);

        // On property accessors
        public static void SetOn(Control control, string? value) => 
            control.SetValue(OnProperty, value);
        public static string? GetOn(Control control) => 
            control.GetValue(OnProperty);

        // Animation property accessors
        public static void SetAnimation(Control control, AnimationResource? value) => 
            control.SetValue(AnimationProperty, value);
        public static AnimationResource? GetAnimation(Control control) => 
            control.GetValue(AnimationProperty);

        private static void OnLoadResourceChanged(Control control, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is AnimationResource animation)
            {
                control.Loaded += async (s, args) =>
                {
                    var builder = animation.Start(control);
                    await builder.StartAsync();
                };
            }
        }

        private static void OnLoadChanged(Control control, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is string animationName && !string.IsNullOrEmpty(animationName))
            {
                control.Loaded += async (s, args) =>
                {
                    var builder = Tweener.Select(control.Name ?? control.GetType().Name, control);
                    builder.Play(animationName);
                    await builder.StartAsync();
                };
            }
        }

        private static void OnAnimateChanged(Control control, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is string tween && !string.IsNullOrEmpty(tween))
            {
                control.Loaded += async (s, args) =>
                {
                    var animation = TweenParser.Parse(tween);
                    var builder = animation.Start(control);
                    await builder.StartAsync();
                };
            }
        }

        private static void OnClickChanged(Control control, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is string tween && !string.IsNullOrEmpty(tween))
            {
                control.AddHandler(Button.ClickEvent, async (object? sender, RoutedEventArgs args) =>
                {
                    var animation = TweenParser.Parse(tween);
                    var builder = animation.Start(control);
                    await builder.StartAsync();
                });
            }
        }

        private static void OnEventChanged(Control control, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is string eventName && !string.IsNullOrEmpty(eventName))
            {
                var animation = GetAnimation(control);
                if (animation == null)
                    return;

                // Subscribe to the event
                var eventInfo = control.GetType().GetEvent(eventName);
                if (eventInfo != null)
                {
                    eventInfo.AddEventHandler(control, async (object? sender, RoutedEventArgs args) =>
                    {
                        var builder = animation.Start(control);
                        await builder.StartAsync();
                    });
                }
            }
        }

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
}
