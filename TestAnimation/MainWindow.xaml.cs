using System;
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
        private const double _height = 300;
        private const double _width = 600;
        public class Ball
        {
            public Ellipse Shape { get; set; }
            public Point Position { get; set; }
            public double Radius { get; set; }
            public bool IsDeleted { get; set; }
            public SolidColorBrush BallColor =  Brushes.DodgerBlue;
        }
        public class BallMovement
        {
            public Ball Ball { get; set; }
            public double Speed { get; set; }
        }
        private BallMovement[] balls;

        public MainWindow() {
            InitializeComponent();
            //InitializeBalls();
            //StartTimer();
            
            const int count = 19;
            MyCanvas.Height = _height;
            MyCanvas.Width = _width;
            MyCanvas.Background = Brushes.Transparent;
            CanvasBorder.BorderBrush = Brushes.Black;
            CanvasBorder.Height = _height;
            CanvasBorder.Width = _width;
            // CanvasBorder.BorderThickness = new Thickness(1);

            System.Windows.Shapes.Rectangle rect;
            rect = new System.Windows.Shapes.Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Black);
            rect.Fill = Brushes.Transparent;
            rect.Width = 30;
            rect.Height = _height;
            Canvas.SetLeft(rect, MyCanvas.Width);
           // Canvas.SetTop(rect,MyCanvas);
            
            int countOfRows = (int)_height / 35;
            
            // Canvas.SetLeft(rect, 20);
            // Canvas.SetTop(rect,10);
            // MyCanvas.Children.Add(rect);
            // channel = new LambdaCollection<Rectangle>(1)
            //     .WithPropertyRect(WidthProperty, i => i * 100.0)
            //     .WithPropertyRect(HeightProperty, i => i * 50.0)
            //     .WithPropertyRect(Shape.FillProperty, i => new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)));
            //
            // circles = new LambdaCollection<Ellipse>(count)
            //     .WithProperty(WidthProperty, i => 20.0)
            //     .WithProperty(HeightProperty, i => 20.0)
            //     .WithProperty(Shape.FillProperty, i => new SolidColorBrush(Color.FromArgb(255, 135, 206, 250)))
            //     .WithXY(x => x * 25.0,
            //             y => y * 0.0 - 23.0);
            //
            // foreach (var ellipse in circles) {
            //     MyCanvas.Children.Add(ellipse);
            // }

            
            
            balls = new BallMovement[count * countOfRows];
            int k = 0;
            for (int j = 0; j < countOfRows; j++) {
                circles = new LambdaCollection<Ellipse>(count)
                    .WithProperty(WidthProperty, i => 20.0)
                    .WithProperty(HeightProperty, i => 20.0)
                    .WithProperty(Shape.FillProperty, i => new SolidColorBrush(Color.FromArgb(255, 135, 206, 250)))
                    .WithXY(x => x * 25.0,
                        y => y * 0.0 + j * 35.0 - 23.0);
                
                
                int widthBetween = -50;
                foreach (var ellipse in circles) {
                    //MyCanvas.Children.Add(ellipse);
     
                    balls[k] = new BallMovement()
                    {
                        Ball = new Ball() { Shape = ellipse, Position = new Point(widthBetween, j * 35.0 - 16.0), Radius = 10 },
                        /*Speed = 1 + j,*/
                        Speed = 1 + j,
                        //Angle = 45 // задаем направление движения шарика в градусах
                    };
                    MyCanvas.Children.Add(balls[k].Ball.Shape);
                    widthBetween += 25;
                    k++;
                }
                

            }
            
        }
        private void StartTimer()
        {
            Timer timer = new Timer(UpdateBalls, null, 0, 20);
        }

        private void RemoveBall(BallMovement ball)
        {
            MyCanvas.Children.Remove(ball.Ball.Shape);
            //balls.Remove(ball);
        }
        private void UpdateBalls(object state)
        {
            // foreach (BallMovement ball in balls)
            // {
            //     //if (!ball.IsMoving) continue; // пропускаем шарики, которые не двигаются
            //
            //     Dispatcher.Invoke(() =>
            //     {
            //         Canvas.SetLeft(ball.Ball.Shape, ball.Ball.Position.X - ball.Ball.Radius);
            //         Canvas.SetTop(ball.Ball.Shape, ball.Ball.Position.Y - ball.Ball.Radius); // обновляем позицию шарика на Canvas
            //     });
            // }
            
            foreach (BallMovement ball in balls)
            {
                //double radians = ball.Angle * Math.PI / 180;
                // if (ball.Ball.Position.X >= 300) {
                //     ball.Ball.BallColor = Brushes.Transparent;
                //     MyCanvas.Children.Remove(ball.Ball.Shape);
                // }
                
                Dispatcher.Invoke(() =>
                {
                    Canvas.SetLeft(ball.Ball.Shape, ball.Ball.Position.X - ball.Ball.Radius);
                    Canvas.SetTop(ball.Ball.Shape, ball.Ball.Position.Y - ball.Ball.Radius);
                });
 
            }
        }
        private void ButtonDo_Click(object sender, RoutedEventArgs e) {
            // StartTimer();
            // // var c = new LambdaDoubleAnimationCollection(
            // //     circles.Count,
            // //     i => i * 25.0,
            // //     i => CanvasBorder.Width,
            // //     i => new Duration(TimeSpan.FromSeconds(2)),
            // //     i => j => 100.0 );
            // // c.BeginApplyAnimation(circles.Cast<UIElement>().ToArray(), Canvas.LeftProperty);
            StartTimer();
            
            for (int i = 0; i < 152; i++) // добавляем анимацию движения трем рядам
            {
                // int rowIndex = i * 5;
                // double speed = 50 + i * 25; // задаем скорость движения шариков
                // double duration = 8 - i; // задаем продолжительность анимации
                //
                // for (int j = 0; j < 20; j++)
                // {
                    //balls[rowIndex + j].IsMoving = true; // устанавливаем флаг, что шарик двигается
                    DoubleAnimation animation = new DoubleAnimation();
                    animation.From = balls[i].Ball.Position.X;
                    animation.To = MyCanvas.Width - 100;
                    //animation.Completed += (s, ev) => { balls[i].Ball.BallColor = Brushes.Transparent; }; 
                    //animation.DecelerationRatio = 1;
                    //animation.SpeedRatio = 0;
                    //animation.Duration = TimeSpan.FromSeconds(5);
                    
                    Storyboard.SetTarget(animation, balls[i].Ball.Shape);
                    Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            
                    Storyboard storyboard = new Storyboard();
                    storyboard.Children.Add(animation);
                    storyboard.SpeedRatio = balls[i].Speed / 20;
                    //storyboard.Completed += (s, ev) => { MyCanvas.Children.Remove(balls[i].Ball.Shape); };
                    //storyboard.Completed += (s, ev) => { balls[rowIndex + j].IsMoving = false; }; // сбрасываем флаг, что шарик двигается
                   
                    storyboard.Begin();
                    
                    //storyboard.Completed += (s, ev) => { MyCanvas.Children.Remove(balls[i].Ball.Shape); };
                    Dispatcher.Invoke(() =>
 
                    { 
                        
                       // animation.Completed += (s, ev) => { MyCanvas.Children.Remove(balls[i].Ball.Shape); };
                        // if (balls[i].Ball.Position.X >= MyCanvas.Width - 100) {
                        //     balls[i].Ball.BallColor = Brushes.Transparent;
                        //     MyCanvas.Children.Remove(balls[i].Ball.Shape);
                        // }
      
                    });
            
                    // storyboard.Completed += (s, ev) => {
                    //     foreach (var ball in balls) {
                    //         ball.Ball.BallColor = Brushes.Transparent;
                    //     }
                    // };
                    // balls[i].Ball.IsDeleted = true; // устанавливаем флаг для удаления шарика из коллекции BallMovement
                    // balls = balls.Where(b => !b.Ball.IsDeleted).ToArray(); // удаляем шарики из BallMovement, которые достигли конца холста
            }
            
            //
            // tvar t = new tvar();
            // t.ShowDialog();



        }   



        private void InitializeBalls()
        {
            balls = new BallMovement[20]; // создаем 3 шарика
            double speed = 5;
            double radius = 10;

            for (int i = 0; i < balls.Length; i++)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = radius * 2;
                ellipse.Height = radius * 2;
                ellipse.Fill = new SolidColorBrush(Colors.DodgerBlue); // задаем цвет шарика
                //MyCanvas.Children.Add(ellipse); // добавляем шарик на Canvas

                Point position = new Point(50 + i * 100, 50); // задаем начальную позицию шарика

                balls[i] = new BallMovement()
                {
                    Ball = new Ball() { Shape = ellipse, Position = position, Radius = radius },
                    Speed = speed,
                    //Angle = 45 // задаем направление движения шарика в градусах
                };
               
            }
            
        }
        


    }
}
