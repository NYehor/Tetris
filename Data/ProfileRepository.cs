using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entity;

namespace Data
{
    public class ProfileRepository: IProfileRepository
    {
        const string FileExtention = ".txt";

        const string RecordKeyWord = "";
        const string MatrixKeyWord = "Matrix:{";
        const string CellsKeyWord = " Cells:{";
        const string WidthKeyWord = "Width:{";
        const string HeightKeyWord = "Height:{";
        const string SymbolOfEnd = "}";
        const string RowKeyWord = "Row:";
        const string ColumnKeyWord = "Column:";
        const string ColorKeyWord = "Color:";

        readonly string FolderPath;
        
        private List<Profile> Profiles;

        public ProfileRepository(string folderPath)
        {
            FolderPath = folderPath;
            Profiles = new List<Profile>();
            Init();
        }
        
        /// <summary>
        ///     Load profiles from the folder
        /// </summary>
        private void Init()
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }

                var txtFiles = Directory.GetFiles(FolderPath, "*" + FileExtention);
                foreach (var file in txtFiles)
                {
                    string name = file.Substring(FolderPath.Length + 1);
                    var profile = ReadProfileFromFile(Path.Combine(FolderPath, name));
                    profile.Name = name.Replace(FileExtention, "");

                    Profiles.Add(profile);
                }

                if (Profiles.Count == 0)
                {
                    Profiles.Add(GetBaseProfile());
                    Save();
                }
            }
            catch (FileLoadException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private Profile ReadProfileFromFile(string path)
        {
            Profile profile = new Profile();
            string[] lines = File.ReadAllLines(path);

            int sInd = lines[0].IndexOf(RecordKeyWord) + RecordKeyWord.Length;
            int length = lines[0].Length - sInd;
            profile.Record = Convert.ToInt32(lines[0].Substring(sInd, length).Trim(' '));

            for (int i = 1; i < lines.Length; i++)
            {
                int sMatrixIndx = lines[i].IndexOf(MatrixKeyWord) + MatrixKeyWord.Length;  
                int fMatrixIndx = lines[i].IndexOf(SymbolOfEnd, sMatrixIndx);

                string matrixStr = lines[i].Substring(sMatrixIndx, fMatrixIndx - sMatrixIndx);
                var numbStr = matrixStr.Split(',');

                var matrix = new Matrix3x3();
                int numbIndex = 0;
                for (short i1 = 0; i1 < 3; i1++)
                {
                    for (short j = 0; j < 3; j++)
                    {
                        matrix[i1, j] = Convert.ToInt32(numbStr[numbIndex]);
                        numbIndex++;
                    }
                }

                int sCellsIndex = lines[i].IndexOf("Cells:{[") + 8;
                int fCellsIndex = lines[i].IndexOf("] }", sCellsIndex);
                string[] cellsStr = lines[i].Substring(sCellsIndex, fCellsIndex - sCellsIndex).Split("] [");

                Cell[] cells = new Cell[cellsStr.Length];
                for (int j = 0; j < cellsStr.Length; j++)
                {
                    int rowInd = cellsStr[j].IndexOf(RowKeyWord) + RowKeyWord.Length;
                    int columnInd = cellsStr[j].IndexOf(ColumnKeyWord) + ColumnKeyWord.Length;
                    int colorInd = cellsStr[j].IndexOf(ColorKeyWord) + ColorKeyWord.Length;

                    short row = Convert.ToInt16(cellsStr[j].Substring(rowInd, columnInd - rowInd - 7).Trim(' '));
                    short column = Convert.ToInt16(cellsStr[j].Substring(columnInd, colorInd - columnInd - 6).Trim(' '));
                    string colorStr = cellsStr[j].Substring(colorInd, cellsStr[j].Length - colorInd).Trim(' ');

                    cells[j] = new Cell(row, column, (CellColor)Enum.Parse(typeof(CellColor), colorStr));
                }

                int widthInd = lines[i].IndexOf(WidthKeyWord) + WidthKeyWord.Length;
                string widthStr = lines[i].Substring(widthInd, lines[i].IndexOf(SymbolOfEnd, widthInd) - widthInd).Trim(' ');
                int width = Convert.ToInt32(widthStr);

                int heightInd = lines[i].IndexOf(HeightKeyWord) + HeightKeyWord.Length;
                string heightStr = lines[i].Substring(heightInd, lines[i].IndexOf(SymbolOfEnd, heightInd) - heightInd).Trim(' ');
                int height = Convert.ToInt32(heightStr);

                profile.Elements.Add(new BaseShape(matrix, cells, width, height));
            }

            return profile;
        }

        /// <summary>
        /// Save all state of profiles in file.
        /// </summary>
        public void Save()
        {
            foreach (var profile in Profiles)
            {
                SaveProfile(profile);
            }
        }

        private void SaveProfile(Profile profile)
        {
            string path = Path.Combine(FolderPath, profile.Name + FileExtention);

            bool isExsistFile = File.Exists(path);
            using StreamWriter file = new StreamWriter(path, !isExsistFile);
            file.WriteLine(RecordKeyWord + profile.Record);

            foreach (var shape in profile.Elements)
            {
                file.WriteLine(WriteShapeToStr(shape));
            }
        }

        private string WriteShapeToStr(IShape shape)
        {
            string text = MatrixKeyWord;
            for (short i = 0; i < 3; i++)
            {
                for (short j = 0; j < 3 && !(j == 2 && i == 2); j++)
                {
                    text += shape.Matrix[i, j] + ",";
                }
            }

            text += shape.Matrix[2, 2].ToString();
            text += SymbolOfEnd;

            text += CellsKeyWord;
            var cells = shape.GetCells();

            foreach (var cell in cells)
            {
                text += "["+ RowKeyWord + cell.Row.ToString() + " " + ColumnKeyWord 
                            + cell.Column.ToString() + " " + ColorKeyWord + cell.Color + "] ";
            }

            text += SymbolOfEnd;
            text += WidthKeyWord + shape.Width + SymbolOfEnd;
            text += HeightKeyWord + shape.Height + SymbolOfEnd;

            return text;
        }

        private Profile GetBaseProfile()
        {
            var lShape = new Cell[] { 
                new Cell(0, 0, CellColor.Orange), 
                new Cell(1, 0, CellColor.Orange), 
                new Cell(2, 0, CellColor.Orange), 
                new Cell(2, 1, CellColor.Orange) 
            };

            var jShape = new Cell[] {
                new Cell(0, 1, CellColor.Purple),
                new Cell(1, 1, CellColor.Purple),
                new Cell(2, 1, CellColor.Purple),
                new Cell(2, 0, CellColor.Purple)
            };

            var zShape = new Cell[] {
                new Cell(0, 0, CellColor.Red),
                new Cell(1, 0, CellColor.Red),
                new Cell(1, 1, CellColor.Red),
                new Cell(2, 1, CellColor.Red)
            };

            var tShape = new Cell[] {
                new Cell(1, 0, CellColor.Green),
                new Cell(0, 1, CellColor.Green),
                new Cell(1, 1, CellColor.Green),
                new Cell(2, 1, CellColor.Green)
            };

            var newBaseProfile = new Profile()
            {
                Name = "BaseProfile",
                Elements = new List<IShape>()
                {
                    new BaseShape(new Matrix3x3(), lShape, 2, 3),
                    new BaseShape(new Matrix3x3(), jShape, 2, 3),
                    new BaseShape(new Matrix3x3(), zShape, 4, 2),
                    new BaseShape(new Matrix3x3(), tShape, 4, 2)
                }
            };

            return newBaseProfile;
        }

        public IEnumerable<Profile> GetAll()
        {
            return Profiles;
        }

        public Profile GetByName(string name)
        {
            return Profiles.First(x => x.Name == name);
        }

        public void Update(Profile profile)
        {
            var currentProfile = Profiles.Find(p => p.Name == profile.Name);
            currentProfile = profile;
        }

        public void Insert(Profile profile)
        {
            Profiles.Add(profile);
        }

        public void Delete(Profile profile)
        { 
            Profiles.Remove(profile);

            string path = Path.Combine(FolderPath, profile.Name + FileExtention);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public IEnumerable<string> GetAllName()
        {
            return Profiles.Select(x => x.Name);
        }
    }
}
