using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ParentChild : Entity
    {
        public int ParentId { get; set; }
        public int ChildId { get; set; }
    }
}
