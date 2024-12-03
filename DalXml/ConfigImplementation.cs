using DalApi;
namespace Dal;

/// <summary>
/// Implementation of the IConfig interface for managing configuration settings in the DAL.
/// </summary>
internal class ConfigImplementation : IConfig
{
    /// <summary>
    /// Gets or sets the system clock value.
    /// </summary>
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    /// <summary>
    /// Gets or sets the risk range value.
    /// </summary>
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    /// <summary>
    /// Resets the configuration to its default values.
    /// </summary>
    public void Reset()
    {
        Config.Reset();
    }
}
