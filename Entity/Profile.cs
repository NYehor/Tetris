using System.Collections.Generic;

namespace Entity
{
    public class Profile
    {
        public string Name { get; set; }
        public int Record { get; set; }
        public List<IShape> Elements { get; set; }

        public Profile() 
        {
            Elements = new List<IShape>();
            Name = "new profile";
        }
    }
}
