using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace UnitTesting
{
    internal class TestEntity : Entity
    {
        public int Number { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public TestEnum EnumValue { get; set; }
    }
}
