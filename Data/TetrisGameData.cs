using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace Data
{
    public class TetrisGameData: IData
    {
        private readonly string FolderPath;

        public ProfileRepository Profiles { get; set; }

        public TetrisGameData(string path)
        {
            FolderPath = path;
            Profiles = new ProfileRepository(FolderPath);
        }

        public void Save()
        {
            Profiles.Save();
        }
    }
}
