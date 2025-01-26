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
        //_dal.ResetDB(); // Reset the database to its initial state
        //Initialization.Do(); // Perform initial setup
        //AdminManager.UpdateClock(AdminManager.Now); // Update the system clock to the current time
        AdminManager.MaxRange = time;
    }

    // Method to move the system clock forward by a specified time unit
    public void ForwardClock(BO.TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7

        AdminManager.UpdateClock(unit switch
        {
            BO.TimeUnit.MINUTE => AdminManager.Now.AddMinutes(1), // Add one minute
            BO.TimeUnit.HOUR => AdminManager.Now.AddHours(1), // Add one hour
            BO.TimeUnit.DAY => AdminManager.Now.AddDays(1), // Add one day
            BO.TimeUnit.MONTH => AdminManager.Now.AddMonths(1), // Add one month
            BO.TimeUnit.YEAR => AdminManager.Now.AddYears(1), // Add one year
            _ => DateTime.MinValue // Default value in case of an invalid unit
        });
    }

    // Method to get the current clock time
    public DateTime GetClock() => AdminManager.Now;
    // Method to retrieve the maximum allowed range for a configuration parameter
    public TimeSpan GetMaxRange()
    {

        return AdminManager.MaxRange;
        //  return _dal.Config.RiskRange;
    }
   // public void SetMaxRange(TimeSpan maxRange) => AdminManager.MaxRange = maxRange;
    
    // Method to reinitialize the system
    public void initialization()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        DalTest.Initialization.Do();
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.MaxRange = AdminManager.MaxRange;

       
    }

    // Method to reset the system
    public void Reset()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        lock (AdminManager.BlMutex) // stage 7  lock (AdminManager.BlMutex) // stage 7
            _dal.ResetDB(); // Reset the database
        AdminManager.UpdateClock(AdminManager.Now); // Update the clock to the current time
        AdminManager.MaxRange = AdminManager.MaxRange;

    }
    public void SetClock(DateTime newTime)
    {
        AdminManager.UpdateClock (newTime);
        
    }
    #region Stage 5

    public void AddClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers += clockObserver;

    public void RemoveClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers -= clockObserver;

    public void AddConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers += configObserver;

    public void RemoveConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers -= configObserver;

    #endregion Stage 5
    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
    public void StopSimulator()
        => AdminManager.Stop(); //stage 7

}
