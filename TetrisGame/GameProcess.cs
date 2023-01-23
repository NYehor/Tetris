using TetrisGame.Shapes;
using System;

namespace TetrisGame
{
    public class GameProcess
    {
        readonly int FieldWidth;
        readonly int FieldHeight;

        public int Score { get; private set; }
        private CellColor[][] GameField;
        private CellColor[][] GameFieldWithElement;
        
        
        private Shape ActiveElement;
        private Random random;
        private Type[] Elements;

        public event Action GameOver;
        private System.Timers.Timer timer;
        public GameProcess(int fieldWidth, int fieldHeight)
        {
            Score = 0;
            FieldWidth = fieldWidth;
            FieldHeight = fieldHeight;

            random = new Random();
            Elements = new Type[] { typeof(LShape), typeof(JShape), typeof(TShape), typeof(ZShape), typeof(IShape)  };
            ActiveElement = GenereteElement();
            GameField = new CellColor[FieldHeight][];
            for (int i = 0; i < FieldHeight; i++)
            {
                GameField[i] = new CellColor[FieldWidth];
            }

            //  GameOver += GameOverMethod;

            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Elapsed += GameStep;
        }

        public void GameStep(object? sender, EventArgs e)
        {
            
            if (ActiveElement.PositionRow < FieldHeight - ActiveElement.ArrayOfCells.Length &&
                IsElementHaveSpaceToMoveDown())
            {
                DownStepMove();
            }
            else
            {
                GameField = GameFieldWithElement;
                ActiveElement = GenereteElement();
            }

            for (int i = 0; i < GameField.Length; i++)
            {
                if (IsRowCompleted(GameField[i]))
                {
                    Array.Clear(GameField[i]);
                    CellColor[][] tmpArr = new CellColor[i][];

                    Array.Copy(GameField, 0, tmpArr, 0, tmpArr.Length);
                    for (int j = 0; j < tmpArr.Length; j++)
                    { 
                        GameField[j + 1] = tmpArr[j];
                    }
                }
            }
        }

        private bool IsElementHaveSpaceToInit(Shape element)
        {
            for (int i = 0; i < ActiveElement.ArrayOfCells.Length; i++)
            {
                for (int j = 0; j < ActiveElement.ArrayOfCells[0].Length; j++)
                {
                    if (GameField[ActiveElement.PositionRow + i][j + ActiveElement.PositionCol] != CellColor.Base &&
                            element.ArrayOfCells[i][j] != CellColor.Base)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsElementHaveSpaceToMoveDown()
        {
            for (int i = 0; i < ActiveElement.ArrayOfCells.Length; i++)
            {
                for (int j = 0; j < ActiveElement.ArrayOfCells[0].Length; j++)
                {
                    if (GameField[ActiveElement.PositionRow + i + 1][j + ActiveElement.PositionCol] != CellColor.Base &&
                        ActiveElement.ArrayOfCells[i][j] != CellColor.Base)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        private bool IsRowCompleted(CellColor[] row)
        {
            foreach (var cell in row)
            {
                if (cell == CellColor.Base)
                    return false;
            }

            Score += 10;
            return true;
        }

        private bool IsRowClear(CellColor[] row)
        {
            foreach (var cell in row)
            {
                if (cell != CellColor.Base)
                    return false;
            }

            return true;
        }

        public CellColor[][] GetGameFieldWithElement()
        {
            CellColor[][] Frame = new CellColor[GameField.Length][];

            for (int i = 0; i < FieldHeight; i++)
            {
                Frame[i] = new CellColor[FieldWidth];
                for (int j = 0; j < FieldWidth; j++)
                {
                    Frame[i][j] = GameField[i][j];
                }
            }

            for (int i = 0; i < ActiveElement.ArrayOfCells.Length; i++)
            {
                for (int j = 0; j < ActiveElement.ArrayOfCells[0].Length; j++)
                {
                    if(ActiveElement.ArrayOfCells[i][j] != CellColor.Base)
                        Frame[ActiveElement.PositionRow + i][j + ActiveElement.PositionCol] = ActiveElement.ArrayOfCells[i][j];
                }
            }

            GameFieldWithElement = Frame;
            return Frame;
        }

        private Shape GenereteElement()
        {
            return (Shape)Activator.CreateInstance(Elements[random.Next(0, Elements.Length)]);
        }

        public void LeftStepMove()
        {
            if (ActiveElement.TryMoveLeft()) return;

            if (ActiveElement.PositionCol > 0)
            {
                ActiveElement.PositionCol--;
            }
        }

        public void RightStepMove()
        { 
            if (ActiveElement.TryMoveRight()) return;

            if (ActiveElement.PositionCol < FieldWidth - ActiveElement.ArrayOfCells[0].Length)
            {
                ActiveElement.PositionCol++;
            }
        }

        public void DownStepMove()
        {
            if (ActiveElement.TryMoveDown()) return;

            if (ActiveElement.PositionRow < FieldHeight - ActiveElement.ArrayOfCells.Length)
            {
                ActiveElement.PositionRow++;
            }
        }

        public void RotateMove() => ActiveElement.Rotate();

        public void Start()
        {
            timer.Enabled = true;
        }

        public void GameOverMethod()
        {
            timer.Enabled = false;
        }
    }
}
