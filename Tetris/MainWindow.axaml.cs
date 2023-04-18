using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media; 
using Avalonia.Threading;
using System;
using System.IO;
using System.Linq;
using TetrisGame;
using Entity;
using Data;
using System.Collections.ObjectModel;
using TetrisView;
using static TetrisView.MessageBoxWindow;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        const string ScoreText = "Score: ";
        const string RecordText = "Record: ";
        const string NameOfFolder = "Profiles";
        const string NameOfBaseProfile = "BaseProfile";
        const string NewProfile_ItemOfComboBox = "New profile...";

        public ObservableCollection<string> Items { get; }

        const int LengthSide = 25;
        const int GameFieldWidth = 10;
        const int GameFieldHeight = 20;

        private bool isKeyDown = false;
        private IData data;
        private DispatcherTimer timer;
        private Rectangle[,] rectangles;
        private Rectangle[,] customShapeRec;
        private CellColor[,] frame;
        private CellColor[,] newShape;
        private ObservableCollection<string> listOfProfiles;
        private int indexOfCurrentElement;

        private GameField GameField;
        private GameProcess GameProcess;
        private ShapeFactory ShapeFactory;
        private Profile ActiveProfile;

        public MainWindow()
        {
            Items = new ObservableCollection<string>();
            indexOfCurrentElement = 0;

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
            CustomElementCanvas.PointerPressed += PointerPressedOnCustomElenementCanvas;

            string pathFolder = System.IO.Path.Combine(Directory.GetCurrentDirectory(), NameOfFolder);
            data = new TetrisGameData(pathFolder);

            ActiveProfile = data.Profiles.GetByName(NameOfBaseProfile);

            listOfProfiles = new ObservableCollection<string>();
            data.Profiles.GetAllName().ToList().ForEach(listOfProfiles.Add);

            listOfProfiles.Add(NewProfile_ItemOfComboBox);
            profileComboBox.Items = listOfProfiles;
            profileComboBox.SelectedItem = NameOfBaseProfile;
            profileComboBox.SelectionChanged += ProfileChangedAsync;

            ShapeFactory = new ShapeFactory(ref ActiveProfile);
            Score_textBlock.Text = ScoreText + "0";
            Record_textBlock.Text = RecordText + Convert.ToString(ActiveProfile.Record);
            GameField = new GameField(GameFieldWidth, GameFieldHeight, frame, ShapeFactory);
            GameProcess = new GameProcess(GameField, GameOver);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += TimerTick;

            Items.Add(ActiveProfile.Name);
        }

        
        private bool TrySetupNewGame(Profile profile)
        {
            bool result = false;
            ActiveProfile = profile;
            Score_textBlock.Text = ScoreText + "0";
            Record_textBlock.Text = RecordText + Convert.ToString(ActiveProfile.Record);

            if (profile.Elements.Count != 0)
            {
                ShapeFactory = new ShapeFactory(ref ActiveProfile);
                GameField = new GameField(GameFieldWidth, GameFieldHeight, frame, ShapeFactory);
                GameProcess = new GameProcess(GameField, GameOver);
                result = true;
            }
            else
            {
                MessageBoxWindow.Show(
                    this,
                    "Add elements to your new game!",
                    "Warning",
                    MessageBoxButtons.Ok
                );
            }

            return result;
        }

        private async void ProfileChangedAsync(object? sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count == 0) return;

            string? selected = args.AddedItems[0] as string;
            if (selected == null) return;
            
            Profile profile = ActiveProfile;
            timer.Stop();
            if (selected == NewProfile_ItemOfComboBox)
            {
                string name = await AddProfileWindow.ShowAndTryGetName(this);
                if (name != null)
                {
                    profile = new Profile() { Name = name };
                    data.Profiles.Insert(profile);
                    listOfProfiles.Insert(0, name);
                    profileComboBox.SelectedItem = name;
                }
                else
                {
                    profileComboBox.SelectedItem = profile.Name;
                }
            }
            else
            {
                var currentProfile = data.Profiles.GetByName(selected);
                if (currentProfile != null)
                {
                    profile = currentProfile;
                }
            }

            if (profile != ActiveProfile)
            {
                TrySetupNewGame(profile);
            }
        }

        private void PointerPressedOnCustomElenementCanvas(object? sender, PointerPressedEventArgs args)
        {
            var point = args.Pointer.Captured;

            if (point != null)
            {
                double x = point.Bounds.Top;
                double y = point.Bounds.Left;

                var row = (int)Math.Round(x / (CustomElementCanvas.Height / 4), 0);
                var col = (int)Math.Round(y / (CustomElementCanvas.Width / 4), 0);


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
            if (GameField.Score > ActiveProfile.Record)
            {
                ActiveProfile.Record = GameField.Score;
                Record_textBlock.Text = RecordText + Convert.ToString(ActiveProfile.Record);
            }

            MessageBoxWindow.Show(this, "Game over!", "Game tetris", MessageBoxButtons.Ok);
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            GameProcess.GameStep();
            Score_textBlock.Text = ScoreText + Convert.ToString(GameField.Score);
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

        private void DrawCustomElement(IShape element)
        {
            DrawGameField(customShapeRec, new CellColor[4,4]);

            var cells = element.GetCells();
            foreach (var cell in cells)
            {
                customShapeRec[cell.Column, cell.Row].Fill = GetBrushColor(cell.Color);
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
                case CellColor.Yellow:
                    return Brushes.Yellow;
                case CellColor.Pink:
                    return Brushes.Pink;
            }

            return Brushes.Black;
        }

        private void OnPreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveProfile.Elements.Count > 0)
            {
                if (indexOfCurrentElement > 0)
                {
                    indexOfCurrentElement--;
                }
                else
                {
                    indexOfCurrentElement = ActiveProfile.Elements.Count - 1;
                }

                DrawCustomElement(ActiveProfile.Elements[indexOfCurrentElement]);
            }
        }

        private void OnNextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveProfile.Elements.Count > 0)
            {
                if (indexOfCurrentElement < ActiveProfile.Elements.Count - 1)
                {
                    indexOfCurrentElement++;
                }
                else
                {
                    indexOfCurrentElement = 0;
                }

                DrawCustomElement(ActiveProfile.Elements[indexOfCurrentElement]);
            }
        }

        private async void OnDeleteCurrentElementBtn_Click(object sender, RoutedEventArgs e)
        {
            var res = await MessageBoxWindow.Show(
                this,
                "Are you sure to delete current element?",
                "Warning",
                MessageBoxButtons.YesNo
                );

            if (res == MessageBoxResult.Yes)
            {
                ActiveProfile.Elements.Remove(ActiveProfile.Elements[indexOfCurrentElement]);
                indexOfCurrentElement++;

                if (indexOfCurrentElement > ActiveProfile.Elements.Count - 1)
                {
                    indexOfCurrentElement = 0;
                }

                DrawCustomElement(ActiveProfile.Elements[indexOfCurrentElement]);
            }
        }

        private void OnCreateElementBtn_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            ShapeFactory.AddShape(newShape, (CellColor)rnd.Next(1, 5));
            newShape = new CellColor[4, 4];
            DrawGameField(customShapeRec, newShape);
        }

        private void OnNewElementBtn_Click(object sender, RoutedEventArgs e)
        {
            newShape = new CellColor[4, 4];
            DrawGameField(customShapeRec, newShape);
        }

        private void OnSaveChangesBtn_Click(object sender, RoutedEventArgs e)
        {
            data.Profiles.Save(); 
        }

        private async void OnDeleteProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (listOfProfiles.Count > 2)
            {
                var selectedItem = profileComboBox.SelectedItem;

                if (selectedItem != null)
                {
                    string? selected = selectedItem.ToString();

                    var res = await MessageBoxWindow.Show(
                        this,
                        "Are you sure to delete " + selected +  " ?",
                        "Warning",
                        MessageBoxButtons.YesNo
                        );

                    if (res == MessageBoxResult.Yes && selected != null && selected != NewProfile_ItemOfComboBox)
                    {
                        listOfProfiles.Remove(selected);
                        profileComboBox.SelectedIndex = 0;
                        data.Profiles.Delete(data.Profiles.GetByName(selected));
                        DrawGameField(customShapeRec, newShape);
                    }
                }
                
            }
            else
            {
                await MessageBoxWindow.Show(
                    this,
                    "You can not delete the last profile!", 
                    "Warning", 
                    MessageBoxButtons.Ok
                    );
            }
        }

        private void OnStartGameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TrySetupNewGame(ActiveProfile))
            {
                timer.Start();
            }
        }



        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (isKeyDown) return;
            isKeyDown = true;

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
            isKeyDown = false;
        }
    }
}
