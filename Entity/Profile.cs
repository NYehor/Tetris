using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
