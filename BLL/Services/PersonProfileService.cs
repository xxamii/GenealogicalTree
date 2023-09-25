using BLL.Abstractions.Interfaces;
using Core;
using Core.Models;
using Core.ViewModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class PersonProfileService : IPersonProfileService
    {
        private readonly IGenericRepository<PersonProfile> _personProfileRepository;
        private readonly IGenericRepository<ParentChild> _parentChildRepository;
        private readonly ISession _session;

        public PersonProfileService(ISession session, IGenericRepository<PersonProfile> personProfileRepository,
            IGenericRepository<ParentChild> parentChildRepository)
        {
            _personProfileRepository = personProfileRepository;
            _session = session;
            _parentChildRepository = parentChildRepository;
        }

        public async Task<List<PersonProfile>> GetPersonProfilesAsync(Func<PersonProfile, bool> predicate = null)
        {
            List<PersonProfile> personProfiles = await _personProfileRepository.DeserializeAsync(predicate);
            return personProfiles;
        }

        public async Task<PersonRelatives?> GetPersonRelativesAsync(PersonProfile personProfile)
        {
            if ((await GetPersonProfileByIdAsync(personProfile.Id)) == null)
            {
                return null;
            }

            PersonRelatives personRelatives = new PersonRelatives (personProfile) {
                Parents = new List<PersonRelatives>(),
                Children = new List<PersonRelatives>()
            };

            personRelatives.Parents = await GetAllPersonParentsAsync(personRelatives);
            personRelatives.Children = await GetAllPersonChildrenAsync(personRelatives);

            return personRelatives;
        }

        public async Task<(PersonProfile?, List<string>)> CreatePersonProfileAsync(PersonProfile personProfile,
            List<int>? parentsIds = null, List<int>? childrenIds = null)
        {
            (PersonProfile?, List<string>) result = (null, new List<string>());
            
            if (_session.IsUserLoggedIn && _session.CurrentUser != null && _session.CurrentUser.Role == Role.Administrator)
            {
                parentsIds = parentsIds ?? new List<int>();
                childrenIds = childrenIds ?? new List<int>();

                bool canCreate = true;

                if (personProfile.Name.Trim().Length < 1)
                {
                    result.Item2.Add("Person name can not be empty");
                    canCreate = false;
                }

                if (personProfile.Country.Trim().Length < 1)
                {
                    result.Item2.Add("Person country can not be empty");
                    canCreate = false;
                }

                if (personProfile.City.Trim().Length < 1)
                {
                    result.Item2.Add("Person city can not be empty");
                    canCreate = false;
                }

                if (canCreate)
                {
                    PersonProfile saved = await _personProfileRepository.SerializeAsync(personProfile);
                    result.Item1 = saved;
                    (bool, List<string>) parentsResult = await UpdateOrCreatePersonParentsAsync(saved, parentsIds);
                    (bool, List<string>) childrenResult = await UpdateOrCreatePersonChildrenAsync(saved, childrenIds);
                    result.Item2.AddRange(parentsResult.Item2);
                    result.Item2.AddRange(childrenResult.Item2);
                }
            }
            else
            {
                result.Item2.Add("Unauthorized access");
            }

            return result;
        }

        public async Task<(bool, List<string>)> DeletePersonProfileAsync(int id)
        {
            (bool, List<string>) result = (false, new List<string>());

            if (_session.IsUserLoggedIn && _session.CurrentUser.Role == Role.Administrator)
            {
                PersonProfile? personProfile = await GetPersonProfileByIdAsync(id);

                if (personProfile != null)
                {
                    await _personProfileRepository.DeleteAsync(personProfile);
                    await DeletePersonParentsAsync(personProfile);
                    await DeletePersonChildrenAsync(personProfile);
                    result.Item1 = true;
                }
                else
                {
                    result.Item2.Add($"Could not find a person with id {id}");
                }
            }
            else
            {
                result.Item2.Add("Unauthorized access");
            }

            return result;
        }

        public async Task<(PersonProfile?, List<string>)> UpdatePersonProfileAsync(PersonProfile toUpdate,
            List<int>? parents = null, List<int>? children = null)
        {
            PersonProfile? personProfile = await GetPersonProfileByIdAsync(toUpdate.Id);
            (PersonProfile?, List<string>) result = (null, new List<string>());

            if (personProfile != null)
            {
                if (toUpdate.Name.Trim().Length < 1)
                {
                    toUpdate.Name = personProfile.Name;
                }

                if (personProfile.Country.Trim().Length < 1)
                {
                    toUpdate.Country = personProfile.Country;
                }

                if (personProfile.City.Trim().Length < 1)
                {
                    toUpdate.City = personProfile.City;
                }

                result.Item1 = await _personProfileRepository.UpdateAsync(toUpdate);

                if (parents != null)
                {
                    (bool, List<string>) parentsResult = await UpdateOrCreatePersonParentsAsync(toUpdate, parents);
                    result.Item2.AddRange(parentsResult.Item2);
                }

                if (children != null)
                {
                    (bool, List<string>) childrenResult = await UpdateOrCreatePersonChildrenAsync(toUpdate, children);
                    result.Item2.AddRange(childrenResult.Item2);
                }
            }
            else
            {
                result.Item2.Add($"Could not find a person with id {toUpdate.Id}");
            }

            return result;
        }

        private async Task<(bool, List<string>)> UpdateOrCreatePersonParentsAsync(PersonProfile personProfile, List<int> parentsIds)
        {
            (bool, List<string>) result = (true, new List<string>());

            if (parentsIds.Count() > 2)
            {
                result.Item1 = false;
                result.Item2.Add("Could not update parents");
                result.Item2.Add("A person can not have more than 2 parents");
                return result;
            }

            List<PersonProfile> parents = new List<PersonProfile>();

            foreach (int id in parentsIds)
            {
                PersonProfile? parent = await GetPersonProfileByIdAsync(id);

                if (parent != null)
                {
                    if (await CheckParentIsNotDescendant(personProfile, parent))
                    {
                        parents.Add(parent);
                    }
                    else
                    {
                        result.Item1 = false;
                        result.Item2.Add($"Could not add a parent with id {id}. The parent is a descendant");
                    }
                }
                else
                {
                    result.Item1 = false;
                    result.Item2.Add($"Could not find a person with id {id}");
                }
            }

            if (result.Item1)
            {
                List<ParentChild> parentChildren = await _parentChildRepository.DeserializeAsync(a => a.ChildId == personProfile.Id);
                foreach (ParentChild child in parentChildren)
                {
                    await _parentChildRepository.DeleteAsync(child);
                }

                foreach (PersonProfile parent in parents)
                {
                    await _parentChildRepository.SerializeAsync(new ParentChild { ParentId = parent.Id, ChildId = personProfile.Id });
                }
            }
            else
            {
                result.Item2.Insert(0, "Could not update parents");
            }

            return result;
        }

        private async Task<(bool, List<string>)> UpdateOrCreatePersonChildrenAsync(PersonProfile personProfile, List<int> childrenIds)
        {
            (bool, List<string>) result = (true, new List<string>());

            List<PersonProfile> children = new List<PersonProfile>();

            foreach (int id in childrenIds)
            {
                PersonProfile? child = await GetPersonProfileByIdAsync(id);

                if (child != null)
                {
                    if (await CheckChildIsNotParent(personProfile, child))
                    {
                        children.Add(child);
                    }
                    else
                    {
                        result.Item1 = false;
                        result.Item2.Add($"Could not add a child with id {id}. The child is an ancestor");
                    }
                }
                else
                {
                    result.Item1 = false;
                    result.Item2.Add($"Could not find a person with id {id}");
                }
            }

            if (result.Item1)
            {
                List<ParentChild> parentChildren = await _parentChildRepository.DeserializeAsync(a => a.ParentId == personProfile.Id);
                foreach (ParentChild child in parentChildren)
                {
                    await _parentChildRepository.DeleteAsync(child);
                }

                foreach (PersonProfile child in children)
                {
                    await _parentChildRepository.SerializeAsync(new ParentChild { ParentId = personProfile.Id, ChildId = child.Id });
                }
            }
            else
            {
                result.Item2.Insert(0, "Could not update children");
            }

            return result;
        }
        
        private async Task<PersonProfile?> GetPersonProfileByIdAsync(int id)
        {
            List<PersonProfile> personProfiles = await _personProfileRepository.DeserializeAsync(a => a.Id == id);
            return personProfiles.FirstOrDefault();
        }

        private async Task<List<PersonRelatives>> GetAllPersonParentsAsync(PersonRelatives person)
        {
            List<PersonRelatives> result = new List<PersonRelatives>();
            List<ParentChild> parentChildren = await _parentChildRepository.DeserializeAsync(a => a.ChildId == person.PersonProfile.Id);

            foreach (ParentChild parentChild in parentChildren)
            {
                PersonProfile parentProfile = await GetPersonProfileByIdAsync(parentChild.ParentId);
                PersonRelatives parent = new PersonRelatives(parentProfile);

                parent.Parents = await GetAllPersonParentsAsync(parent);

                result.Add(parent);
            }

            return result;
        }

        private async Task<List<PersonRelatives>> GetAllPersonChildrenAsync(PersonRelatives person)
        {
            List<PersonRelatives> result = new List<PersonRelatives>();
            List<ParentChild> parentChildren = await _parentChildRepository.DeserializeAsync(a => a.ParentId == person.PersonProfile.Id);

            foreach (ParentChild parentChild in parentChildren)
            {
                PersonProfile childProfile = await GetPersonProfileByIdAsync(parentChild.ChildId);
                PersonRelatives child = new PersonRelatives(childProfile);

                child.Children = await GetAllPersonChildrenAsync(child);

                result.Add(child);
            }

            return result;
        }

        private async Task DeletePersonParentsAsync(PersonProfile personProfile)
        {
            List<ParentChild> parentChildren = await _parentChildRepository.DeserializeAsync(a => a.ChildId == personProfile.Id);

            foreach(ParentChild parentChild in parentChildren)
            {
                await _parentChildRepository.DeleteAsync(parentChild);
            }
        }

        private async Task DeletePersonChildrenAsync(PersonProfile personProfile)
        {
            List<ParentChild> parentChildren = await _parentChildRepository.DeserializeAsync(a => a.ParentId == personProfile.Id);

            foreach (ParentChild parentChild in parentChildren)
            {
                await _parentChildRepository.DeleteAsync(parentChild);
            }
        }

        private async Task<bool> CheckParentIsNotDescendant(PersonProfile person, PersonProfile parent)
        {
            List<ParentChild> children = await _parentChildRepository.DeserializeAsync(a => a.ParentId == person.Id);
            foreach (ParentChild child in children)
            {
                if (child.ChildId == parent.Id)
                {
                    return false;
                }

                PersonProfile? childProfile = (await _personProfileRepository.DeserializeAsync(a => a.Id == child.ChildId)).FirstOrDefault();
                return await CheckParentIsNotDescendant(childProfile, parent);
            }

            return true;
        }

        private async Task<bool> CheckChildIsNotParent(PersonProfile person, PersonProfile child)
        {
            List<ParentChild> parents = await _parentChildRepository.DeserializeAsync(a => a.ChildId == person.Id);
            foreach (ParentChild parent in parents)
            {
                if (parent.ParentId == child.Id)
                {
                    return false;
                }

                PersonProfile? parentProfile = (await _personProfileRepository.DeserializeAsync(a => a.Id == parent.ParentId)).FirstOrDefault();
                return await CheckChildIsNotParent(parentProfile, child);
            }

            return true;
        }
    }
}
