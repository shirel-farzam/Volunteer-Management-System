using DalApi;

namespace Dal;

internal class ConfigImplementation : IConfig
{
    // Property to get and set the system clock from Config
    public DateTime Clock { get => Config.Clock; set => Config.Clock = value; }

    // Property to get and set the risk range from Config
    public TimeSpan RiskRange { get => Config.RiskRange; set => Config.RiskRange = value; }

    // Method to reset the configuration values to their initial states
    public void Reset()
    {
        Config.Reset(); // Calls the Reset method from Config to reset the values
    }
}
