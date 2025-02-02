using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PL
{
    
    internal class VolunteerInListFieldCollection : IEnumerable
    {
        static readonly IEnumerable<BO.VolunteerInListField> s_enums =
    (Enum.GetValues(typeof(BO.VolunteerInListField)) as IEnumerable<BO.VolunteerInListField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();

        public BO.VolunteerInListField VolIn { get; set; } = BO.VolunteerInListField.None;

    }
    internal class CallStatusCollection : IEnumerable
    {
        static readonly IEnumerable<BO.CallStatus> s_enums =
    (Enum.GetValues(typeof(BO.CallStatus)) as IEnumerable<BO.CallStatus>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class OpenCallInListCollection : IEnumerable
    {
        static readonly IEnumerable<BO.OpenCallInListField> s_enums =
    (Enum.GetValues(typeof(BO.OpenCallInListField)) as IEnumerable<BO.OpenCallInListField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class CallTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.CallType> s_enums =
    (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class ClosedCallInListFieldCollection : IEnumerable
    {
        static readonly IEnumerable<BO.ClosedCallInListField> s_enums =
    (Enum.GetValues(typeof(BO.ClosedCallInListField)) as IEnumerable<BO.ClosedCallInListField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class DistanceCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Distance> s_enums =
    (Enum.GetValues(typeof(BO.Distance)) as IEnumerable<BO.Distance>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class RoleCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Role> s_enums =
    (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class CallInListFieldCollection : IEnumerable
    {
        static readonly IEnumerable<BO.CallInListField> s_enums =
    (Enum.GetValues(typeof(BO.CallInListField)) as IEnumerable<BO.CallInListField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

}
