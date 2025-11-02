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
                .To(new Thickness(200, 0, 0, 0), TimeSpan.FromMilliseconds(800))
                .WithEasing(new BackEaseOut())
                .StartAsync();
        }

        private async void OnBounceButtonClick(object? sender, RoutedEventArgs e)
        {
            var button = (Button)sender!;
            await Animator.Select(button)
                .Animate(TranslateTransform.YProperty)
                .To(-50.0, TimeSpan.FromMilliseconds(400))
                .WithEasing(new QuadraticEaseOut())
                .To(0.0, TimeSpan.FromMilliseconds(400))
                .WithEasing(new BounceEaseOut())
                .StartAsync();
        }

        private async void OnColorButtonClick(object? sender, RoutedEventArgs e)
        {
            var button = (Button)sender!;
            await Animator.Select(button)
                .Animate(Button.BackgroundProperty)
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
                .To(1.5, TimeSpan.FromMilliseconds(300))
                .WithEasing(new BackEaseOut())
                .To(1.0, TimeSpan.FromMilliseconds(300))
                .WithEasing(new ElasticEaseOut())
                .StartAsync();
        }

        private async void OnRotateButtonClick(object? sender, RoutedEventArgs e)
        {
            var button = (Button)sender!;
            await Animator.Select(button)
                .Animate(RotateTransform.AngleProperty)
                .To(360.0, TimeSpan.FromMilliseconds(1000))
                .WithEasing(new CubicEaseInOut())
                .StartAsync();
        }

        private async void OnMultiButtonClick(object? sender, RoutedEventArgs e)
        {
            var button = (Button)sender!;
            
            var scaleTask = Animator.Select(button)
                .Animate(ScaleTransform.ScaleXProperty)
                .To(1.2, TimeSpan.FromMilliseconds(600))
                .To(1.0, TimeSpan.FromMilliseconds(400))
                .StartAsync();

            var colorTask = Animator.Select(button)
                .Animate(Button.BackgroundProperty)
                .To(new SolidColorBrush(Colors.Crimson), TimeSpan.FromMilliseconds(1000))
                .StartAsync();

            await Task.WhenAll(scaleTask, colorTask);
        }
    }
}