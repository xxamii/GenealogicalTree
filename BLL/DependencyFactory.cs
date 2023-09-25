using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Models;

namespace BLL
{
    public class DependencyFactory
    {
        private static ISession _session;

        public static IUserService GetUserService()
        {
            return new UserService(DAL.DependencyFactory.GetGenericRepository<User>());
        }

        public static IPersonProfileService GetPersonProfileService()
        {
            return new PersonProfileService(GetSession(), DAL.DependencyFactory.GetGenericRepository<PersonProfile>(),
                DAL.DependencyFactory.GetGenericRepository<ParentChild>());
        }

        public static ISession GetSession()
        {
            if (_session == null)
            {
                _session = new Session(GetUserService());
            }

            return _session;
        }
    }
}
