using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Abstractions;
using Core.Models;

namespace DAL
{
    public class DependencyFactory
    {
        public static GenericRepository<T> GetGenericRepository<T>()
            where T : Entity
        {
            string DBPath = Path.Combine(Directory.GetCurrentDirectory(), "db.json");
            return new GenericRepository<T>(DBPath);
        }
    }
}
