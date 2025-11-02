using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaAnimate;
using System;
using System.Threading.Tasks;

namespace AvaloniaTween.Demo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AnimateButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var tb = Animator.Select("TextBlock", this)
                .Animate(Canvas.LeftProperty)
                    .From(50.0)
                    .To(200.0, TimeSpan.FromSeconds(1))
                .Animate(RotateTransform.AngleProperty)
                    .From(0.0)
                    .To(300.0, TimeSpan.FromSeconds(3))
                    .Reset();

            _ = tb.StartAsync();
        }

        private void ResetButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

        private void CueButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var tb = Animator.Select("TextBlock", this)
                .Animate(Canvas.LeftProperty)
                    .From(50.0)
                    .To(300.0, TimeSpan.FromSeconds(1), 0.5)
                        .WithEasing(new ElasticEaseOut())  // Very springy! 🎯
                    .To(200.0, TimeSpan.FromSeconds(1))
                        .WithEasing(new BounceEaseOut())   // Bouncy landing!
                .Animate(RotateTransform.AngleProperty)
                    .From(0.0)
                    .To(360.0, TimeSpan.FromSeconds(2))
                        .WithEasing(new BackEaseOut())     // Slight overshoot
                    .Reset();

            _ = tb.StartAsync();
        }
    }
}