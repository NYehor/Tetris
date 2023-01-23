using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using SkiaSharp;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Threading;
using System.Timers;
using TetrisGame;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        const int LengthSide = 25;
        const int GameFieldWidth = 10;
        const int GameFieldHeight = 20;

        
        private DispatcherTimer timer;
        private GameProcess gameProcess;
        private Rectangle[][] rectangles;

        public int Score { get => gameProcess.Score; }
        public MainWindow()
        {
            InitializeComponent();
            myCanvas.Focus();
            myCanvas.Width = 254;
            myCanvas.Height = 504;

            gameProcess = new GameProcess(GameFieldWidth, GameFieldHeight);
            rectangles = new Rectangle[GameFieldHeight][];

            for (int i = 0; i < GameFieldHeight; i++)
            {
                rectangles[i] = new Rectangle[GameFieldWidth];
                for (int j = 0; j < GameFieldWidth; j++)
                {
                    rectangles[i][j] = new Rectangle()
                    {
                        Height = LengthSide - 4,
                        Width = LengthSide - 4,
                        Fill = Brushes.Black
                    };

                    Canvas.SetLeft(rectangles[i][j], LengthSide * j + 4);
                    Canvas.SetTop(rectangles[i][j], LengthSide * i + 4);
                    myCanvas.Children.Add(rectangles[i][j]);
                }
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += TimerTick;

           // gameProcess.GameOver += GameOver;
        }

        private void GameOver()
        { 
        
        }
        private void TimerTick(object? sender, EventArgs e)
        {
            DrawGameField(gameProcess.GetGameFieldWithElement());
            Score_textBox.Text = "Score: " + gameProcess.Score;
        }


        private void DrawGameField(CellColor[][] frame)
        {
            for (int i = 0; i < GameFieldHeight; i++)
            {
                for (int j = 0; j < GameFieldWidth; j++)
                {
                    rectangles[i][j].Fill = GetBrushColor(frame[i][j]);
                }
            }
        }

        private IBrush GetBrushColor(CellColor cellColor)
        {
            switch (cellColor)
            {
                case CellColor.Base:
                    return Brushes.Gray;
                case CellColor.Red:
                    return Brushes.Red;
                case CellColor.Orange:
                    return Brushes.Orange;
                case CellColor.Green:
                    return Brushes.Green;
                case CellColor.Purple:
                    return Brushes.Purple;
                case CellColor.Blue:
                    return Brushes.Blue;
            }

            return Brushes.Black;
        }

        private void OnStartGameBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            gameProcess.Start();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                case Key.Left:
                    gameProcess.LeftStepMove();
                    return;
                case Key.Right:
                case Key.D:
                    gameProcess.RightStepMove();
                    return;
                case Key.Up:
                case Key.W:
                    gameProcess.RotateMove();
                    return;
                case Key.Down:
                case Key.S:
                    gameProcess.DownStepMove();
                    return;
            }
        }

        private void OnKeyUpHandler(object sender, KeyEventArgs e)
        {

        }
    }
}
