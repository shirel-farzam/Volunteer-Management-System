
namespace BlApi;

public interface IAdmin
{
    DateTime GetClock();
    // Method declaration for advancing the system clock by a specific time unit
     void AdvanceClock(BO.TimeUnit timeUnit);

    // Method declaration for getting the risk time range configuration
     TimeSpan GetRiskTimeRange();

    // Method declaration for setting the risk time range configuration
     void SetRiskTimeRange(TimeSpan riskTimeRange);

    // Method declaration for resetting the database to its initial state
     void ResetDatabase();

    // Method declaration for initializing the database with default values
     void InitializeDatabase();
}

