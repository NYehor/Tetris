using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IData
    {
        ProfileRepository Profiles { get; set; }

        /// <summary>
        /// Save all state of the repositories 
        /// </summary>
        void Save();
    }
}
