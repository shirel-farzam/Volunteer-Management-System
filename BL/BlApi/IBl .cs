

namespace BlApi;

internal interface IBl
{
    IVolunteer Volunteer { get; }
    ICall Call { get; }
    IAdmin Admin { get; }
}
