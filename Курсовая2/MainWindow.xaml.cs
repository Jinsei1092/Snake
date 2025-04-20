using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Text.Json;
using Microsoft.Win32;
using System.Text.Json.Serialization;
using System.IO;

namespace Snake
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int CellSize = 20;
        const int GridSize = 25;
        const int Speed = 250;

        CObstacles obstacles;
        CSnake snake;
        CFood food;
        DispatcherTimer timer;
        СDirection currentDirection;

        public MainWindow()
        {
            InitializeComponent();

            cbdifficulties.Items.Add("Easy");
            cbdifficulties.Items.Add("Hard");

            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Speed) };
            timer.Tick += Timer_Tick;
        }

        private void DrawInitialState()
        {
            foreach (CPoint position in snake.Body)
            {
                DrawCell(position.X, position.Y, Brushes.Green);
            }

            if (obstacles != null)
            {
                foreach (CPoint position in obstacles.Wall)
                {
                    DrawCell(position.X, position.Y, Brushes.Black);
                }
            }

            DrawCell(food.X, food.Y, Brushes.Red);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateGameState();
            RedrawGameState();
        }

        private void UpdateGameState()
        {
            snake.Move(currentDirection);

            if (snake.IsSelfCollision() || IsOutOfBounds() || IsKnockedObstacle())
            {
                GameOver();
            }

            if (HasEatenFood())
            {
                snake.Grow();
                
                food = new CFood(FoodPos()[0], FoodPos()[1]);
			}
        }

        private void RedrawGameState()
        {
            gameGrid.Children.Clear();

            for (int i = 0; i < snake.Body.Count(); i++)
            {
                DrawCell(snake.Body[i].X, snake.Body[i].Y, Brushes.Green);
            }

            if (cbdifficulties.SelectedIndex == 1)
            {
                for (int i = 0; i < obstacles.Wall.Count(); i++)
                {
                    DrawCell(obstacles.Wall[i].X, obstacles.Wall[i].Y, Brushes.Black);
                }
            }

            DrawCell(food.X, food.Y, Brushes.Red);
        }

        private void DrawCell(int x, int y, Brush brush)
        {
            Rectangle rect = new Rectangle
            {
                Width = CellSize,
                Height = CellSize,
                Fill = brush
            };

            Canvas.SetLeft(rect, x * CellSize);
            Canvas.SetTop(rect, y * CellSize);

            gameGrid.Children.Add(rect);
        }

        private bool HasEatenFood()
        {
            return snake.Head.X == food.X && snake.Head.Y == food.Y;
        }

        private int GetRandomPosition()
        {
            Random r = new Random();
            return r.Next(GridSize);
        }

        private bool IsOutOfBounds()
        {
            return snake.Head.X < 0 || snake.Head.X >= GridSize ||
                   snake.Head.Y < 0 || snake.Head.Y >= GridSize;
        }

        private bool IsKnockedObstacle()
        {
            bool a = false;

            if (cbdifficulties.SelectedIndex == 1)
            {
                foreach (CPoint position in obstacles.Wall)
                {
                    if (snake.Head.X == position.X && snake.Head.Y == position.Y)
                    {
                        a = true; break;
                    }
                }
            }
            return a;
        }

        private bool IsEmpty(int x, int y, bool b)
        {
            bool a = true;
            if (b == true && cbdifficulties.SelectedIndex == 1)
            {
                foreach (CPoint position in obstacles.Wall)
                {
                    if (x == position.X && position.Y == y)
                    {
                        a = false;
                    }
                }
            }

            foreach (CPoint position in snake.Body)
            {
                if (x == position.X && position.Y == y)
                {
                    a = false;
                }
            }

            return a;
        }

        private void GameOver()
        {
            cbdifficulties.IsEnabled = true;
            
            LbScore.Items.Add(cbdifficulties.SelectedItem.ToString() + ": Score - " + snake.Body.Count());

            timer.Stop();
            MessageBox.Show("Игра окончена! Ваш счёт - " + snake.Body.Count(), "Змейка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private int[] FoodPos()
        {
            int[] a = new int[2];
            while (true)
            {
                bool b = false;
                a[0] = GetRandomPosition();
                a[1] = GetRandomPosition();
                for (int i = 0; i < snake.Body.Count(); i++)
                {
                    if (IsEmpty(a[0], a[1], true) == false)
                    {
                        b = true;
                    }
                }
                if (b == false)
                {
                    break;
                }
            }
            return a;
        }

        private int[] ObstaclePos(bool c)
        {
            int[] a = new int[2];
            while (true)
            {
                bool b = false;
                a[0] = GetRandomPosition();
                a[1] = GetRandomPosition();
                if (IsEmpty(a[0], a[1], c) == false || (a[0] >= snake.Head.X && a[1] == snake.Head.Y))
                {
                    b = true;
                }
                if (b == false)
                {
                    break;
                }
            }
            return a;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btStart.IsEnabled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up when currentDirection != СDirection.Down:
                    currentDirection = СDirection.Up;
                    break;
                case Key.Down when currentDirection != СDirection.Up:
                    currentDirection = СDirection.Down;
                    break;
                case Key.Left when currentDirection != СDirection.Right:
                    currentDirection = СDirection.Left;
                    break;
                case Key.Right when currentDirection != СDirection.Left:
                    currentDirection = СDirection.Right;
                    break;
            }
        }

        private void CreateObstacles()
        {
            for (int i = 0; i < 10; i++)
            {
                obstacles.CountUp(ObstaclePos(true)[0], ObstaclePos(true)[1]);
            }
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            snake = new CSnake(GridSize / 4, GridSize / 4);

            if (cbdifficulties.SelectedIndex == 1)
            {
                obstacles = new CObstacles(ObstaclePos(false)[0], ObstaclePos(false)[1]);
                CreateObstacles();
            }

            food = new CFood(FoodPos()[0], FoodPos()[1]);

            currentDirection = СDirection.Right;

            timer.Start();

            cbdifficulties.IsEnabled = false;

            DrawInitialState();
        }

        public void btSaveJson_Click(object sender, RoutedEventArgs e)
        {
            string jsonString = JsonSerializer.Serialize(LbScore.Items);
            // Сохранение JSON в файл
            File.WriteAllText(@"C:\\Users\\an232\\Desktop\Score.json", jsonString);
        }
    }
}
