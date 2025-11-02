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
                    .From(50.0).To(200.0).Duration(TimeSpan.FromSeconds(1))
                .Animate(RotateTransform.AngleProperty)
                    .From(0.0).To(300.0).Duration(TimeSpan.FromSeconds(1))
                    .Reset();
            
            _ = tb.StartAsync();
        }

        private void ResetButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }
    }
}