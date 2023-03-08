using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media; 
using Avalonia.Threading;
using SkiaSharp;
using System;
using System.IO;
using System.ComponentModel.Design;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Timers;
using TetrisGame;
using Entity;
using Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using TetrisView;
using static TetrisView.MessageBoxWindow;
using System.Threading.Tasks;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Items { get; }

        const int LengthSide = 25;
        const int GameFieldWidth = 10;
        const int GameFieldHeight = 20;

        private IData data;
        private DispatcherTimer timer;
        private Rectangle[,] rectangles;
        private Rectangle[,] customShapeRec;
        private CellColor[,] frame;
        private CellColor[,] newShape;

        private GameField GameField;
        private GameProcess GameProcess;
        private ShapeFactory ShapeFactory;
        private Profile ActiveProfile;
        private List<string> listOfProfiles;

        public MainWindow()
        {
            Items = new ObservableCollection<string>();

            InitializeComponent();
            myCanvas.Focus();
            myCanvas.Width = 254;
            myCanvas.Height = 504;

            frame = new CellColor[GameFieldWidth, GameFieldHeight];
            rectangles = new Rectangle[GameFieldWidth, GameFieldHeight];
            InitField(rectangles, myCanvas, 1);

            CustomElementCanvas.Focus();
            CustomElementCanvas.Width = 2 * LengthSide * 4 + 4;
            CustomElementCanvas.Height = 2 * LengthSide * 4 + 4;

            newShape = new CellColor[4, 4];
            customShapeRec = new Rectangle[4, 4];
            InitField(customShapeRec, CustomElementCanvas, 2);
            DrawGameField(customShapeRec, newShape);
            CustomElementCanvas.PointerPressed += PointerPressed;

            string pathFolder = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Profiles");
            data = new TetrisGameData(pathFolder);

            ActiveProfile = data.Profiles.GetByName("BaseProfile");

            listOfProfiles = data.Profiles.GetAllName().ToList();
            listOfProfiles.Add("New profile...");
            listOfProfiles.Add("Delete profile...");
            profileComboBox.Items = listOfProfiles;
            profileComboBox.SelectedItem = "BaseProfile";
            profileComboBox.SelectionChanged += ProfileChangedAsync;

            ShapeFactory = new ShapeFactory(ActiveProfile);
            GameField = new GameField(GameFieldWidth, GameFieldHeight, frame, ShapeFactory);
            GameProcess = new GameProcess(GameField, GameOver);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += TimerTick;

            Items.Add(ActiveProfile.Name);
        }

        private async void ProfileChangedAsync(object? sender, SelectionChangedEventArgs args)
        {
            var text = args.AddedItems[0] as string;

            if (text == "New profile...")
            {
                string name  = await AddProfileWindow.ShowAndTryGetName(this);
                if (name != "")
                {
                    Profile newProfile = new Profile();
                    newProfile.Name = name;
                    listOfProfiles.Insert(0, name);

                    profileComboBox.Items = listOfProfiles;
                    profileComboBox.SelectedItem = name;
                    data.Profiles.Insert(newProfile);

                    ActiveProfile = newProfile;
                    GameField = new GameField(GameFieldWidth, GameFieldHeight, frame, ShapeFactory);
                    GameProcess = new GameProcess(GameField, GameOver);
                }

                return;
            }

            if (text == "Delete profile...")
            {
                return;
            }

            Profile profile = data.Profiles.GetByName(text);
            if (profile != null)
            {
                timer.Stop();
                ActiveProfile = profile;
                GameField = new GameField(GameFieldWidth, GameFieldHeight, frame, ShapeFactory);
                GameProcess = new GameProcess(GameField, GameOver);
            }    
        }

        private void PointerPressed(object? sender, PointerPressedEventArgs args)
        {
            var point = args.Pointer.Captured;

            var x = point.Bounds.Top;
            var y = point.Bounds.Left;

            var row = (int)Math.Round((x - 4) / (2 * LengthSide), 0);
            var col = (int)Math.Round((y - 4) / (2 * LengthSide), 0);

            if (newShape[col, row] == CellColor.Base)
            {
                newShape[col, row] = CellColor.Red;
            }
            else
            {
                newShape[col, row] = CellColor.Base;
            } 

            DrawGameField(customShapeRec, newShape);
        }

        private void InitField(Rectangle[,] rectangles, Canvas canvas, int scale)
        {
            int width = rectangles.GetUpperBound(0) + 1;
            int height = rectangles.GetUpperBound(1) + 1;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    rectangles[i, j] = new Rectangle()
                    {
                        Height = scale * LengthSide - 4,
                        Width = scale * LengthSide - 4,
                        Fill = Brushes.Black
                    };

                    Canvas.SetLeft(rectangles[i, j], scale * LengthSide * i + 4);
                    Canvas.SetTop(rectangles[i, j], scale * LengthSide * j + 4);
                    canvas.Children.Add(rectangles[i, j]);
                }
            }
        }

        private void GameOver()
        {
            timer.Stop();
            MessageBoxWindow.Show(this, "Game over!", "Game tetris", MessageBoxButtons.Ok);
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            GameProcess.GameStep();
            GameField.DrawGameField();
            DrawGameField(rectangles, frame);
        }

        private void DrawGameField(Rectangle[,] rectangles, CellColor[,] cells)
        {
            int width = rectangles.GetUpperBound(0) + 1;
            int height = rectangles.GetUpperBound(1) + 1;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    rectangles[i, j].Fill = GetBrushColor(cells[i,j]);
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
            GameField = new GameField(GameFieldWidth, GameFieldHeight, frame, ShapeFactory);
            GameProcess = new GameProcess(GameField, GameOver);
            timer.Start();
        }

        Random rnd = new Random();
        private void OnCreateElementBtn_Click(object sender, RoutedEventArgs e)
        {
            ShapeFactory.AddShape(newShape, (CellColor)rnd.Next(1, 5));
            newShape = new CellColor[4,4];
            DrawGameField(customShapeRec, newShape);
        }

        private void OnCreateAndSaveElementBtn_Click(object sender, RoutedEventArgs e)
        {

            ShapeFactory.AddShape(newShape, (CellColor)rnd.Next(1, 5));
            data.Save();
            newShape = new CellColor[4, 4];
            DrawGameField(customShapeRec, newShape);

        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                case Key.Left:
                    GameProcess.LefElement();
                    DrawGameField(rectangles, frame);
                    return;
                case Key.Right:
                case Key.D:
                    GameProcess.RightElement();
                    DrawGameField(rectangles, frame);
                    return;
                case Key.Up:
                case Key.W:
                    GameProcess.RotateElement();
                    DrawGameField(rectangles, frame);
                    return;
                case Key.Down:
                case Key.S:
                    GameProcess.DownElement();
                    DrawGameField(rectangles, frame);
                    return;
            }
        }

        private void OnKeyUpHandler(object sender, KeyEventArgs e)
        {
        }
    }
}
