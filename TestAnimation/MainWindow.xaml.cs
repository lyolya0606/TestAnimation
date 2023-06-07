﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TestAnimation {
    public class LambdaCollection<T> : Collection<T>
        where T : DependencyObject, new() {
        public LambdaCollection(int count) {
            while (count-- > 0) {
                Add(new T());
            } 
        }

        public LambdaCollection<T> WithProperty<U>(DependencyProperty property, Func<int, U> generator) {
            for (int i = 1; i < Count; i++) {
                this[i].SetValue(property, generator(i));
            }

            return this;
        }

        public LambdaCollection<T> WithPropertyRect<U>(DependencyProperty property, Func<int, U> generator) {
            this[0].SetValue(property, generator(0));
            return this;
        }

        public LambdaCollection<T> WithXY<U>(Func<int, U> xGen, Func<int, U> yGen) {
            for (int i = 0; i < Count; i++) {
                this[i].SetValue(Canvas.LeftProperty,  xGen(i - 2));
                this[i].SetValue(Canvas.TopProperty, yGen(i));
            }

            return this;
        }
        
    }

    public class LambdaDoubleAnimation : DoubleAnimation {
        public Func<double, double> ValueGenerator { get; set; }

        // protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue,
        //     AnimationClock animationClock) {
        //     return ValueGenerator(base.GetCurrentValueCore(defaultOriginValue, defaultDestinationValue, animationClock));
        // }
    }

    public class LambdaDoubleAnimationCollection : Collection<LambdaDoubleAnimation> {
        public LambdaDoubleAnimationCollection(int count, Func<int, double> from, Func<int, double> to, Func<int, 
            Duration> duration, Func<int, Func<double, double>> valueGenerator) {
            for (int i = 0; i < count; i++) {
                var lda = new LambdaDoubleAnimation {
                    From = from(i),
                    To = to(i),
                    Duration = duration(i),
                    //ValueGenerator = valueGenerator(i)
                };
                Add(lda);
            }
        }

        public void BeginApplyAnimation(UIElement[] targets, DependencyProperty property) {
            for (int i = 0; i < Count; i++) {
                targets[i].BeginAnimation(property, Items[i]);
            }
        }
    }
    


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private LambdaCollection<Ellipse> circles;
        private LambdaCollection<Rectangle> channel;
        private const double _height = 210;
        private const double _width = 510;
        private double _coverVelocity = 1.5;
        private const int _count = (int)(_width / 18.5);
        private int _countOfRows = (int)_height / 21;
        
        public class Ball {
            public Ellipse Shape { get; set; }
            public Point Position { get; set; }
            public double Radius { get; set; }
            public SolidColorBrush BallColor =  Brushes.DodgerBlue;
        }
        public class BallMovement {
            public Ball Ball { get; set; }
            public double Speed { get; set; }
        }
        
        private BallMovement[] balls;
        private BallMovement[] ballsNew;

        public MainWindow() {
            InitializeComponent();


            MyCanvas.Height = _height;
            MyCanvas.Width = _width;
            MyCanvas.Background = Brushes.Transparent;
            CanvasBorder.BorderBrush = Brushes.Black;
            CanvasBorder.Height = _height;
            CanvasBorder.Width = _width;
            
            
            balls = new BallMovement[_count * _countOfRows];
            int k = 0;
            for (int j = 0; j < _countOfRows; j++) {
                circles = new LambdaCollection<Ellipse>(_count)
                    .WithProperty(WidthProperty, i => 10.0)
                    .WithProperty(HeightProperty, i => 10.0)
                    .WithProperty(Shape.FillProperty, i => new SolidColorBrush(Color.FromArgb(255, 135, 206, 250)))
                    .WithXY(x => x * 20.0,
                        y => y * 0.0 + j * 20.0 - 23.0);
                
                
                int widthBetween = -60;
                foreach (var ellipse in circles) {
                    //MyCanvas.Children.Add(ellipse);
     
                    balls[k] = new BallMovement() {
                        Ball = new Ball() { Shape = ellipse, Position = new Point(widthBetween, j * 20.0 - 16.0), Radius = 5 },
                        /*Speed = 1 + j,*/
                        Speed = _coverVelocity / _height * (_height - j * 20),
                        //Angle = 45 // задаем направление движения шарика в градусах
                    };
                    Canvas.SetLeft(balls[k].Ball.Shape, widthBetween);
                    MyCanvas.Children.Add(balls[k].Ball.Shape);
                    widthBetween += 20;
                    k++;
                }
            }
        }
        
        
        private void MakeAnimation(BallMovement ball, bool isStarted = false) {
            var startPositionX = isStarted ? ball.Ball.Position.X : -58;
            var myDoubleAnimation = new DoubleAnimation {
                
                From = startPositionX,
                To = MyCanvas.Width - 50,
                FillBehavior = FillBehavior.Stop,
                Duration = new Duration(
                    new TimeSpan((long)((MyCanvas.Width - 50 - startPositionX) * 30000))) // какое-то число для нормальных тиков
            };
            myDoubleAnimation.Completed += (o, args) =>  MyStoryboardOnCompleted(ball, myDoubleAnimation);
            Storyboard.SetTarget(myDoubleAnimation, ball.Ball.Shape);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.LeftProperty));
            var myStoryboard = new Storyboard {
                SpeedRatio = ball.Speed / 10
            };
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Begin();
        }

        private void MyStoryboardOnCompleted(BallMovement ball, DependencyObject myDoubleAnimation) {
            MakeAnimation(ball);
        }
        
        private void ButtonDo_Click(object sender, RoutedEventArgs e) {
            for (int i = 0; i < _count * (_countOfRows - 1); i++) {
                MakeAnimation(balls[i], true);
            }
            // foreach (var ball in balls) {
            //     MakeAnimation(ball, true);
            // }

        }   

       


    }
}
