using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;
using Entity;

namespace Data
{
    public class ProfileRepository: IProfileRepository
    {
        readonly string FolderPath;
        
        private List<Profile> Profiles;

        public ProfileRepository(string folderPath)
        {
            FolderPath = folderPath;
            Profiles = new List<Profile>();
            Init();
        }
        
        private void Init()
        {
            try
            {
                if (Directory.Exists(FolderPath))
                {
                    var txtFiles = Directory.EnumerateFiles(FolderPath, "*.txt");
                    foreach (var file in txtFiles)
                    {
                        string name = file.Substring(FolderPath.Length + 1);
                        var profile = ReadProfileFromFile(Path.Combine(FolderPath, name));
                        profile.Name = name.Replace(".txt", "");

                        Profiles.Add(profile);
                    }

                    if (Profiles.Count == 0)
                    {
                        Profiles.Add(GetBaseProfile());
                        Save();
                    }
                }
                else
                {
                    Directory.CreateDirectory(FolderPath);
                    Profiles.Add(GetBaseProfile());
                    Save();
                }                
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

            int sInd = lines[0].IndexOf("Record:") + 7;
            int length = lines[0].Length - sInd;
            profile.Record = Convert.ToInt32(lines[0].Substring(sInd, length).Trim(' '));

            for (int i = 1; i < lines.Length; i++)
            {
                int sMatrixIndx = lines[i].IndexOf("Matrix:{") + 8;  
                int fMatrixIndx = lines[i].IndexOf("}", sMatrixIndx);

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
                    int rowInd = cellsStr[j].IndexOf("Row:") + 4;
                    int columnInd = cellsStr[j].IndexOf("Column:") + 7;
                    int colorInd = cellsStr[j].IndexOf("Color:") + 6;

                    short row = Convert.ToInt16(cellsStr[j].Substring(rowInd, columnInd - rowInd - 7).Trim(' '));
                    short column = Convert.ToInt16(cellsStr[j].Substring(columnInd, colorInd - columnInd - 6).Trim(' '));
                    string colorStr = cellsStr[j].Substring(colorInd, cellsStr[j].Length - colorInd).Trim(' ');

                    cells[j] = new Cell(row, column, (CellColor)Enum.Parse(typeof(CellColor), colorStr));
                }

                int widthInd = lines[i].IndexOf("Width:{") + 7;
                string widthStr = lines[i].Substring(widthInd, lines[i].IndexOf("}", widthInd) - widthInd).Trim(' ');
                int width = Convert.ToInt32(widthStr);

                int heightInd = lines[i].IndexOf("Height:{") + 8;
                string heightStr = lines[i].Substring(heightInd, lines[i].IndexOf("}", heightInd) - heightInd).Trim(' ');
                int height = Convert.ToInt32(heightStr);

                profile.Elements.Add(new BaseShape(matrix, cells, width, height));
            }

            return profile;
        }

        public void Save()
        {
            foreach (var profile in Profiles)
            {
                SaveProfile(profile);
            }
        }

        public void SaveProfile(Profile profile)
        {
            string path = Path.Combine(FolderPath, profile.Name + ".txt");
            bool flag = false;

            if (File.Exists(path))
            {
                flag = false;
            }
            else
            {
                flag = true;
            }

            using StreamWriter file = new StreamWriter(path, flag);
            file.WriteLine("Record:" + profile.Record);

            foreach (var shape in profile.Elements)
            {
                file.WriteLine(WriteShapeToStr(shape));
            }
        }

        private string WriteShapeToStr(IShape shape)
        {
            string text = "Matrix:{";
            for (short i = 0; i < 3; i++)
            {
                for (short j = 0; j < 3 && !(j == 2 && i == 2); j++)
                {
                    text += shape.Matrix[i, j] + ",";
                }
            }

            text += shape.Matrix[2, 2].ToString();
            text += "}";

            text += " Cells:{";
            var cells = shape.GetCells();

            foreach (var cell in cells)
            {
                text += "[Row:" + cell.Row.ToString() + " Column:" + cell.Column.ToString() + " Color:" + cell.Color + "] ";
            }

            text += "} ";
            text += "Width:{" + shape.Width + "} ";
            text += "Height:{" + shape.Height + "}";

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
            Profiles.Find(p => p.Name == profile.Name);
        }

        public void Insert(Profile profile)
        {
            Profiles.Add(profile);
        }

        public void Delete(Profile profile)
        { 
            Profiles.Remove(profile);
        }

        public IEnumerable<string> GetAllName()
        {
            return Profiles.Select(x => x.Name);
        }
    }
}
