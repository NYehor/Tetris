using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    internal interface IProfileRepository
    {
        IEnumerable<Profile> GetAll();
        IEnumerable<string> GetAllName();
        Profile GetByName(string name);
        void Update(Profile profile);
        void Insert(Profile profile);
        void Delete(Profile profile);
        void Save();
    }
}
