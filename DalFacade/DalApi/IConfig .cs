namespace DalApi;

// Interface defining configuration settings and operations for the system
public interface IConfig
{
    // Property representing the system clock
    DateTime Clock { get; set; }

    // Property representing the risk range as a time span
    TimeSpan RiskRange { get; set; }

    // Method to reset the configuration to its default values
    void Reset();
}

