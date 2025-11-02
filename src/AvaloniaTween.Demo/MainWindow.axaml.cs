using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaTweener;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Interactivity;

namespace AvaloniaTweener.Demo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Register a named animation
            Animator.Register("fadeIn", builder =>
            {
                builder.Animate(Visual.OpacityProperty)
                    .FromTo(0.0, 1.0, TimeSpan.FromSeconds(0.5))
                    .WithEasing(new CubicEaseOut());
            });
        }

        private void AnimateButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var tb = Animator.Select("TextBlock", this)
                .Animate(Canvas.LeftProperty)
                    .From(50.0)
                        .WithDelay(TimeSpan.FromMilliseconds(200))
                    .To(200.0, TimeSpan.FromSeconds(1))
                .Animate(RotateTransform.AngleProperty)
                    .From(0.0)
                    .To(360.0, TimeSpan.FromSeconds(1))
                        .Repeat(2)
                    // .OnUpdate(progress => Debug.WriteLine($"Progress: {progress:P0}"))
                    .Reset();

            _ = tb.StartAsync();
        }

        private void ResetButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Play a named animation
            var tb = Animator.Select("TextBlock", this);
            tb.Play("fadeIn");
            _ = tb.StartAsync();
        }

        private void CueButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Use timeline for sequenced animations
            var timeline = Animator.CreateTimeline()
                .Add(
                    Animator.Select("TextBlock", this)
                        .Animate(Canvas.LeftProperty)
                            .FromTo(50.0, 360.0, TimeSpan.FromSeconds(1))
                            .WithEasing(new ElasticEaseOut())
                            .Hold()
                            .Build()
                )
                .Add(
                    Animator.Select("TextBlock", this)
                        .Animate(Canvas.LeftProperty)
                            .FromTo(360.0, 200.0, TimeSpan.FromSeconds(1))
                            .WithEasing(new BounceEaseOut())
                            .Hold()
                            .Build()
                );

            _ = timeline.StartAsync();
        }

        private void OnCodeExamplesClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var window = new CodeExamples();
            window.Show();
        }

        private void OnTweenExamplesClick(object? sender, RoutedEventArgs e)
        {
            var window = new TweenExamples();
            window.Show();
        }
    }
}