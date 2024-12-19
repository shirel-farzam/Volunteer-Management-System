using DalApi;

namespace Dal;

// Implementation of the IConfig interface for managing configuration settings.
internal class ConfigImplementation : IConfig
{
    // Property for getting and setting the clock from the Config.
    public DateTime Clock { get => Config.Clock; set => Config.Clock = value; }

    // Property for getting and setting the risk range from the Config.
    public TimeSpan RiskRange { get => Config.RiskRange; set => Config.RiskRange = value; }

    // Method to reset the configuration to its initial state.
    public void Reset()
    {
        Config.Reset();
    }
}
