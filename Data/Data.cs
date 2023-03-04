using System;

namespace Data
{
    public class Data
    {
        private readonly string Path;
        public Profile Profile { get; }

        public Data(string path) 
        {
            Path = path;
        }
    }
}
