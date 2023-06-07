using System;
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
using System.Windows.Shapes;

namespace TestAnimation {

    public partial class tvar : Window {
        public class Ball
        {
            public Ellipse Shape { get; set; }
            public Point Position { get; set; }
            public double Radius { get; set; }
            public bool IsDeleted { get; set; }
            public SolidColorBrush BallColor =  Brushes.DodgerBlue;
        }
        public class Row
        {
            public ObservableCollection<Ball> Balls { get; set; }
            public double Speed { get; set; }
           // public double OffsetY { get; set; }
        }
        
        public class BallMovement
        {
            public Ball Ball { get; set; }
            public double Speed { get; set; }
        }
        private BallMovement[] balls;
        private BallMovement[] ballsNew;
        private const double _height = 300;
        private const double _width = 600;
        ObservableCollection<Row> rows = new ObservableCollection<Row>();
      
        public tvar() {
            InitializeComponent();
            const int count = 19;
            MyCanvas.Height = _height;
            MyCanvas.Width = _width;
            MyCanvas.Background = Brushes.Transparent;
            CanvasBorder.BorderBrush = Brushes.Black;
            CanvasBorder.Height = _height;
            CanvasBorder.Width = _width;
            int countOfRows = (int)_height / 35;
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 20;
            ellipse.Height = 20;
            ellipse.Fill = new SolidColorBrush(Colors.DodgerBlue);

            for (int j = 0; j < countOfRows; j++) {
                Row row = new Row() {
                    Speed = j + 1,
                    //OffsetY = r.Next(0, (int)canvas.ActualHeight / 2),
                    Balls = new ObservableCollection<Ball>()
                };
                int widthBetween = -50;
                for (int i = 0; i < count; i++) {
                    Ball ball = new Ball() {
                        Shape = ellipse, Position = new Point(widthBetween, j * 35.0 - 16.0), Radius = 10
                    };
                    widthBetween += 25;
                    row.Balls.Add(ball);
                   // MyCanvas.Children.Add(ball.Shape);
                }

                rows.Add(row);
            }

            DrawCircles();




        }
       
        private void DrawCircles()
        {
            MyCanvas.Children.Clear();

            foreach (Row row in rows)
            {
                foreach (Ball circle in row.Balls)
                {

                    MyCanvas.Children.Add(circle.Shape);
                }
            }
        }
        private void OnLoaded(object sender, RoutedEventArgs e) 
        { 
        } 
        
    }
        
    
}
    
    