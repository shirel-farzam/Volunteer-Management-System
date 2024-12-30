using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    internal class VolunteerInListFieldCollection : IEnumerable
    {
        static readonly IEnumerable<BO.VolunteerInListField> s_enums =
    (Enum.GetValues(typeof(BO.VolunteerInListField)) as IEnumerable<BO.VolunteerInListField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    
    public BO.VolunteerInListField VolIn { get; set; } = BO.VolunteerInListField.None;
       
    }
}
