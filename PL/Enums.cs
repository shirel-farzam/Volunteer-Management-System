using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PL
{
    //internal class VolunteerFilterCollection : IEnumerable
    //{
    //    // Collection of VolunteerInListField enum values
    //    private static readonly IEnumerable<BO.VolunteerInListField> s_enums =
    //        Enum.GetValues(typeof(BO.VolunteerInListField)).Cast<BO.VolunteerInListField>();

    //    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    //}

    //internal class DistanceTypes : IEnumerable
    //{
    //    // Collection of Distance enum values
    //    private static readonly IEnumerable<BO.Distance> distanceTypes =
    //        Enum.GetValues(typeof(BO.Distance)).Cast<BO.Distance>();

    //    public IEnumerator GetEnumerator() => distanceTypes.GetEnumerator();
    //}

    //internal class Roles : IEnumerable
    //{
    //    // Collection of Role enum values
    //    private static readonly IEnumerable<BO.Role> roles =
    //        Enum.GetValues(typeof(BO.Role)).Cast<BO.Role>();

    //    public IEnumerator GetEnumerator() => roles.GetEnumerator();
    //}

    //internal class CallType : IEnumerable
    //{
    //    // Collection of CallType enum values
    //    private static readonly IEnumerable<BO.CallType> callTypes =
    //        Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType>();

    //    public IEnumerator GetEnumerator() => callTypes.GetEnumerator();
    //}

    //internal class VolunteerInListFieldCollection : IEnumerable
    //{
    //    static readonly IEnumerable<BO.VolunteerInListField> s_enums =
    //(Enum.GetValues(typeof(BO.VolunteerInListField)) as IEnumerable<BO.VolunteerInListField>)!;

    //    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();

    //    public BO.VolunteerInListField VolIn { get; set; } = BO.VolunteerInListField.None;

    //}
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
}
