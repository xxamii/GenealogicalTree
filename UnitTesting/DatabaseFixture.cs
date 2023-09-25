using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace UnitTesting
{
    public class DatabaseFixture : IDisposable
    {
        public readonly string DatabasePath;
        private readonly string _initialDatabaseData;

        public DatabaseFixture()
        {
            DatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "test.json");

            _initialDatabaseData = File.ReadAllText(DatabasePath);
        }

        public void Dispose()
        {
            File.WriteAllText(DatabasePath, _initialDatabaseData);
        }
    }
}
