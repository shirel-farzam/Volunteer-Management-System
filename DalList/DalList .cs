namespace Dal;

using DalApi;

/// <summary>
/// Implementation of the IDal interface for managing the data access layer (DAL).
/// This class provides access to the different entities (Volunteer, Call, Assignment, Config) 
/// and allows for resetting the database (removing all data and resetting configurations).
/// </summary>
sealed internal class DalList : IDal
{
    public static IDal Instance { get; } = new DalList();
    private DalList() { }


    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    /// <summary>
    /// Provides access to call management operations.
    /// </summary>
    public ICall Call { get; } = new CallImplementation();

    /// <summary>
    /// Provides access to assignment management operations.
    /// </summary>
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    /// <summary>
    /// Provides access to configuration management operations.
    /// </summary>
    public IConfig Config { get; } = new ConfigImplementation();

    
    /// Resets the entire database by deleting all entries and resetting the configurations.

    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}
