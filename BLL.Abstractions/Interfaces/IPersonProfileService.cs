using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Core.ViewModels;

namespace BLL.Abstractions.Interfaces
{
    public interface IPersonProfileService
    {
        public Task<List<PersonProfile>> GetPersonProfilesAsync(Func<PersonProfile, bool> predicate = null);
        public Task<PersonRelatives?> GetPersonRelativesAsync(PersonProfile personProfile);
        public Task<(PersonProfile?, List<string>)> CreatePersonProfileAsync(PersonProfile personProfile,
            List<int>? parents = null, List<int>? children = null);
        public Task<(bool, List<string>)> DeletePersonProfileAsync(int id);
        public Task<(PersonProfile?, List<string>)> UpdatePersonProfileAsync(PersonProfile personProfile,
            List<int>? parents = null, List<int>? children = null);
    }
}
