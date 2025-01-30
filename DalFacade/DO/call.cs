namespace DO;

/// <summary>
/// The Call entity represents an event or request managed in the system.
/// </summary>
/// <param name="Id">Represents the unique identifier for the call.</param>
/// <param name="Type">The type of the call.</param>
/// <param name="Description">A detailed description of the call.</param>
/// <param name="FullAddress">The full address where the intervention is required.</param>
/// <param name="Latitude">Latitude - the geographical location of the call.</param>
/// <param name="Longitude">Longitude - the geographical location of the call.</param>
/// <param name="TimeOpened">The time the call was opened (date and time by the manager).</param>
/// <param name="MaxTimeToClose">The maximum time allowed to close the call (default: null).</param>
public record Call
(
    int Id,
    CallType Type,
    string Description,
    string FullAddress,
    double ?Latitude,
    double ?Longitude,
    DateTime TimeOpened,
    DateTime? MaxTimeToClose = null
)
{
    /// <summary>
    /// Default constructor for creating a call with default values.
    /// </summary>
    public Call() : this(0, default(CallType), "", "", 0, 0, DateTime.MinValue) { }
}
