

namespace DO;


public record Call
(

        int Id,
        CallType Type,
        string Description,
        string FullAddress,
        double Latitude,
        double Longitude,
        DateTime TimeOpened,
        DateTime? MaxTimeToClose = null
   )
{
    /// <summary>
    /// Default constructor for Call with default values
    /// </summary>
    public Call() : this(0, default(CallType), "", "", 0, 0, DateTime.MinValue) { }
}