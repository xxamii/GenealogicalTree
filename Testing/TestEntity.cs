using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Testing
{
    internal class TestEntity : Entity
    {
        public int Number { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public List<int> Values { get; set; } = new List<int>();
        public TestEnum EnumValue { get; set; }
    }
}
