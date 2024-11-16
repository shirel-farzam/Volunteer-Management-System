namespace DO;

/// <summary>
/// The Assignment entity represents a volunteer's handling of a specific call.
/// </summary>
/// <param name="Id">A unique sequential identifier for the assignment entity.</param>
/// <param name="CallId">The identifier of the call that the volunteer chose to handle.</param>
/// <param name="VolunteerId">The ID number of the volunteer who chose to handle the call.</param>
/// <param name="TimeStart">The start time of handling (date and time).</param>
/// <param name="TimeEnd">The actual end time of handling (date and time, default: null).</param>
/// <param name="TypeEndTreat">The type of treatment conclusion (completed, self-cancelled, admin-cancelled, expired).</param>
public record Assignment
(
    int Id,           // A unique sequential identifier for the assignment entity
    int CallId,        // The identifier of the call that the volunteer chose to handle
    int VolunteerId,   // The ID number of the volunteer who chose to handle the call
    DateTime TimeStart, /// The start time of handling (date and time)
    DateTime? TimeEnd = null, // The actual end time of handling (date and time)
    TypeEnd? TypeEndTreat = null // The type of treatment conclusion (completed, self-cancelled, admin-cancelled, expired)
)
{
    /// <summary>
    /// Default constructor for creating an assignment entity with basic values.
    /// </summary>
    public Assignment() : this(0, 0, 0, default(DateTime)) { }
}
