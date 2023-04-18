using Entity;


namespace TetrisGame
{
    public class GameProcess
    {
        public Action GameOver {  get; set; }

        private GameField GameField;

        public GameProcess(GameField gameField, Action action)
        {
            GameField = gameField;
            GameOver = action;
        }

        public void GameStep()
        { 
            DownElement();
        }

        public void DownElement()
        {
            if (!TryMoveElement(new Vector3(0, 1, 0)))
            {
                if (!GameField.TryAddNextElement())
                {
                    GameOver();
                }
            }
        }

        public void LefElement()
        {
            TryMoveElement(new Vector3(-1, 0, 0));
        }

        public void RightElement()
        {
            TryMoveElement(new Vector3(1, 0, 0));
        }

        public void RotateElement()
        {
            var matrix = GameField.ActiveElemnt.Matrix;
            var rotate = Matrix3x3.Rotate(Angle.Turn90, new Vector3(1,1,1));

            var newMatrix = matrix * rotate;
            if (IsSaveMove(newMatrix))
            {
                GameField.ActiveElemnt.Matrix = newMatrix;
                GameField.ActiveElemnt.SwapWidthAndHeight();
            }
        }

        private bool TryMoveElement(Vector3 vector)
        {
            var matrix = GameField.ActiveElemnt.Matrix;
            var newMatrix = Matrix3x3.Translate(matrix, vector);
            bool isSave = IsSaveMove(newMatrix);

            if (isSave)
            {
                GameField.ActiveElemnt.Matrix = newMatrix;
            }

            return isSave;
        }

        private bool IsSaveMove(Matrix3x3 matrix)
        { 
            bool result = true;

            var cells = GameField.ActiveElemnt.GetCells();
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Location = matrix * cells[i].Location;

                if (cells[i].Column < 0 || cells[i].Column >= GameField.Width ||
                    cells[i].Row < 0 || cells[i].Row >= GameField.Height ||
                    GameField.IsExistCellInOddments(cells[i]))
                { 
                    result = false;
                    break;
                }
            }

            return result;
        }
    }  
} 
