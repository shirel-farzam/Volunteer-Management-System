using BlApi; // Using the BL (Business Logic) interfaces
using DalTest; // For testing the Data Access Layer (DAL)
using Helpers; // Helper utilities

namespace BlImplementation;

// Implementation of the Admin interface
internal class AdminImplementation : IAdmin
{
    // Creating a DAL instance using the Factory pattern
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    // Method to define and initialize the system
    public void Definition(TimeSpan time)
    {
        _dal.ResetDB(); // Reset the database to its initial state
        Initialization.Do(); // Perform initial setup
        ClockManager.UpdateClock(ClockManager.Now); // Update the system clock to the current time
    }

    // Method to move the system clock forward by a specified time unit
    public void ForwardClock(BO.TimeUnit unit) => ClockManager.UpdateClock(unit switch
    {
        BO.TimeUnit.MINUTE => ClockManager.Now.AddMinutes(1), // Add one minute
        BO.TimeUnit.HOUR => ClockManager.Now.AddHours(1), // Add one hour
        BO.TimeUnit.DAY => ClockManager.Now.AddDays(1), // Add one day
        BO.TimeUnit.MONTH => ClockManager.Now.AddMonths(1), // Add one month
        BO.TimeUnit.YEAR => ClockManager.Now.AddYears(1), // Add one year
        _ => DateTime.MinValue // Default value in case of an invalid unit
    });

    // Method to get the current clock time
    public DateTime GetClock() => _dal.Config.Clock;

    // Method to retrieve the maximum allowed range for a configuration parameter
    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange;
    }

    // Method to reinitialize the system
    public void initialization()
    {
        DalTest.Initialization.Do(); // Reinitialize the DAL for testing purposes
        ClockManager.UpdateClock(ClockManager.Now); // Update the clock to the current time
    }

    // Method to reset the system
    public void Reset()
    {
        _dal.ResetDB(); // Reset the database
        ClockManager.UpdateClock(ClockManager.Now); // Update the clock to the current time
    }
}
