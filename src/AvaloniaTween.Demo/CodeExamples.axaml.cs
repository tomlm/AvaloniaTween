using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Threading.Tasks;

namespace AvaloniaTweener.Demo
{
    public partial class CodeExamples : Window
    {
        public CodeExamples()
        {
            InitializeComponent();
        }

        private async void OnSlideButtonClick(object? sender, RoutedEventArgs e)
        {
            var button = (Button)sender!;
            await Animator.Select(button)
                .Animate(Button.MarginProperty)
                    .To(new Thickness(100, 0, 0, 0), TimeSpan.FromMilliseconds(1000))
                        .WithEasing(new BackEaseOut())
                    .Reset()
                .StartAsync();
        }

        private async void OnBounceButtonClick(object? sender, RoutedEventArgs e)
        {
            var button = (Button)sender!;
            await Animator.Select(button)
                .Animate(TranslateTransform.YProperty)
                    .To(-100.0, TimeSpan.FromMilliseconds(700))
                        .WithEasing(new QuadraticEaseOut())
                    .To(0.0, TimeSpan.FromMilliseconds(700))
                        .WithEasing(new BounceEaseOut())
                    .Reset()
                .StartAsync();
        }

        private async void OnColorButtonClick(object? sender, RoutedEventArgs e)
        {
            var button = (Button)sender!;
            await Animator.Select(button)
                .Animate(Button.BackgroundProperty)
                    .WithDelay(TimeSpan.FromSeconds(1))
                    .To(new SolidColorBrush(Colors.Purple), TimeSpan.FromMilliseconds(500))
                    .To(new SolidColorBrush(Colors.Orange), TimeSpan.FromMilliseconds(500))
                    .To(new SolidColorBrush(Colors.Teal), TimeSpan.FromMilliseconds(500))
                .StartAsync();
        }

        private async void OnScaleButtonClick(object? sender, RoutedEventArgs e)
        {
            var button = (Button)sender!;
            await Animator.Select(button)
                .Animate(ScaleTransform.ScaleXProperty)
                    .To(2.0, TimeSpan.FromMilliseconds(1000))
                        .WithEasing(new BackEaseOut())
                    .To(1.0, TimeSpan.FromMilliseconds(1000))
                        .WithEasing(new ElasticEaseOut())
                    .Reset()
                .StartAsync();
        }

        private async void OnRotateButtonClick(object? sender, RoutedEventArgs e)
        {
            var button = (Button)sender!;
            await Animator.Select(button)
                .Animate(RotateTransform.AngleProperty)
                    .To(360.0, TimeSpan.FromMilliseconds(1000))
                        .WithEasing(new CubicEaseInOut())
                    .Reset()
                .StartAsync();
        }

        private async void OnMultiButtonClick(object? sender, RoutedEventArgs e)
        {
            var button = (Button)sender!;
            
            await Animator.Select(button)
                .Animate(ScaleTransform.ScaleXProperty)
                    .To(2.0, TimeSpan.FromSeconds(1))
                    .To(1.0, TimeSpan.FromSeconds(1))
                .Animate(Button.BackgroundProperty)
                    .To(new SolidColorBrush(Colors.Crimson), TimeSpan.FromMilliseconds(2000))
                .StartAsync();
        }
    }
}