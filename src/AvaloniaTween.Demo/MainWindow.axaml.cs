using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaAnimate;
using System;

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
                    .To(300.0, TimeSpan.FromSeconds(1), 0.5)    // Move to 300 over 1 second at 50%
                    .To(200.0, TimeSpan.FromSeconds(1))         // Move to 200 at 100%
                .Animate(RotateTransform.AngleProperty)
                    .From(0.0)
                    .To(300.0, TimeSpan.FromSeconds(3))
                    .Reset();

            _ = tb.StartAsync();
        }
    }
}