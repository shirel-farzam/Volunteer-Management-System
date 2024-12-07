using DalApi;
using System.Diagnostics;
namespace Dal;

/// <summary>
/// Implementation of the IDal interface for managing the DAL layer with XML-based storage.
/// </summary>
sealed internal class DalXml : IDal
{
    public static IDal Instance { get; } = new DalXml();
    private DalXml() { }


    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    /// <summary>
    /// Property to access Call-related operations.
    /// </summary>
    public ICall Call { get; } = new CallImplementation();

    /// <summary>
    /// Property to access Assignment-related operations.
    /// </summary>
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    /// <summary>
    /// Property to access Config-related operations.
    /// </summary>
    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// Resets the database by deleting all data and resetting the configuration to its default values.
    /// </summary>
    public void ResetDB()
    {
        Volunteer.DeleteAll(); // Deletes all Volunteer data
        Call.DeleteAll();      // Deletes all Call data
        Assignment.DeleteAll(); // Deletes all Assignment data
        Config.Reset();        // Resets the configuration
    }
}
