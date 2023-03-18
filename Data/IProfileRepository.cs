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
        /// <summary>
        ///     Get all profiles of the game
        /// </summary>
        /// <returns></returns>
        IEnumerable<Profile> GetAll();

        /// <summary>
        ///     Get all names of profiles of the game
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetAllName();

        /// <summary>
        ///     Get the profile by the name
        /// </summary>
        /// <param name="name">The name of the profile to be found</param>
        /// <returns>Profile</returns>
        Profile GetByName(string name);

        /// <summary>
        ///     Update the current profile
        /// </summary>
        /// <param name="profile">The current profile going to update</param>
        void Update(Profile profile);

        /// <summary>
        ///     Insert new profile in profiles of the game
        /// </summary>
        void Insert(Profile profile);

        /// <summary>
        ///    Delete the profile from the game
        /// </summary>
        /// <param name="profile">The current profile going to delete</param>
        void Delete(Profile profile);

        /// <summary>
        /// Save current state in profiles
        /// </summary>
        void Save();
    }
}
