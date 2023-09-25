using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.ViewModels
{
    public class PersonRelatives
    {
        public PersonProfile PersonProfile { get; set; }
        public List<PersonRelatives> Parents { get; set; } = new List<PersonRelatives>();
        public List<PersonRelatives> Children { get; set; } = new List<PersonRelatives>();

        public PersonRelatives(PersonProfile personProfile)
        {
            PersonProfile = personProfile;
        }
    }
}
