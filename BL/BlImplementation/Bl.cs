namespace BlImplementation;
using BlApi;

// Main BL class implementing IBl
internal class Bl : IBl
{
    public IVolunteer Volunteer { get; } = new VolunteerImplementation(); // Handles volunteer operations
    public ICall Call { get; } = new CallImplementation(); // Handles call operations
    public IAdmin Admin { get; } = new AdminImplementation(); // Handles admin operations
}
