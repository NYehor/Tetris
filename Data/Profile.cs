using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class Profile
    {
        public string UserName { get; set; }
        public int Record { get; set; }

        internal Profile() 
        {
            UserName = "";
            Record = 0;        
        }
    }
}
