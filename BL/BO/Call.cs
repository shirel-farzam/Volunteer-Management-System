
namespace BO;

internal class Call
{
    public int Id { get; init; } // Unique identifier for the call (לקוח מ-DO.Call)
    public CallType Type { get; set; } // Type of the call (Emergency, Regular, etc.)
    public string? Description { get; set; } // Description of the call (optional)
    public string? FullAddress { get; set; } // Full address of the call (optional)
    public double? Latitude { get; set; } // Latitude of the call (calculated based on address)
    public double? Longitude { get; set; } // Longitude of the call (calculated based on address)
    public DateTime OpenTime { get; set; } // The time the call was opened (cannot be updated)
    public DateTime? MaxEndTime { get; set; } // Maximum time to finish the call (optional)
    public CallStatus Status { get; set; } // Current status of the call (Open, InProgress, etc.)
    public List<CallAssignInList>? Assignments { get; set; } // List of assignments for the call (past and present)

}
public class CallAssignInList
{
    // Volunteer ID (from DO.Assignment)
    public int VolunteerId { get; set; }

    // Volunteer Name (from DO.Volunteer)
    public string VolunteerName { get; set; }

    // Start time when the volunteer begins treatment for the call
    public DateTime StartTime { get; set; }

    // Actual end time when the volunteer finishes treatment (if completed)
    public DateTime? EndTime { get; set; }

    // Completion type of the treatment (e.g., completed, canceled, expired)
    public AssignmentCompletionType? CompletionType { get; set; }

    // Constructor to create a new assignment for the call
    public CallAssignInList(int volunteerId, string volunteerName, DateTime startTime, DateTime? endTime = null, AssignmentCompletionType? completionType = null)
    {
        VolunteerId = volunteerId;  // Initialize volunteer ID
        VolunteerName = volunteerName;  // Initialize volunteer name
        StartTime = startTime;  // Initialize the start time of the treatment
        EndTime = endTime;  // Initialize end time (optional)
        CompletionType = completionType;  // Initialize the completion status (optional)
    }

    // Override ToString method to display assignment information as a string
    public override string ToString()
    {
        return $"Volunteer {VolunteerName} (ID: {VolunteerId}) - Start: {StartTime}, End: {(EndTime.HasValue ? EndTime.Value.ToShortDateString() : "Not completed")}, Completion Type: {CompletionType?.ToString() ?? "Not assigned"}";
    }
}
