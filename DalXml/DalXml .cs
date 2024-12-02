using DalApi;
namespace Dal;

public class DalXml : IDal
{
    // Property for the Volunteer implementation
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    // Property for the Call implementation
    public ICall Call { get; } = new CallImplementation();

    // Property for the Assignment implementation
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    // Property for the Config implementation
    public IConfig Config { get; } = new ConfigImplementation();

    // Method to reset the database by deleting all data and resetting the configuration
    public void ResetDB()
    {
        // Delete all volunteers from the XML storage
        Volunteer.DeleteAll();

        // Delete all calls from the XML storage
        Call.DeleteAll();

        // Delete all assignments from the XML storage
        Assignment.DeleteAll();

        // Reset the configuration to its default values
        Config.Reset();
    }
}
