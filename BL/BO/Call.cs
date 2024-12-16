using DO;
using Helpers;

namespace BO;
public class Call
{
    public int Id { get; init; } // Unique identifier for the call 
    public CallType Type { get; set; } // Type of the call (Emergency, Regular, etc.)
    public string? Description { get; set; } // Description of the call (optional)
    public string? FullAddress { get; set; } // Full address of the call (optional)
    public double? Latitude { get; set; } // Latitude of the call (calculated based on address)
    public double? Longitude { get; set; } // Longitude of the call (calculated based on address)
    public DateTime OpenTime { get; set; } // The time the call was opened (cannot be updated)
    public DateTime? MaxEndTime { get; set; } // Maximum time to finish the call (optional)
    public CallStatus Status { get; set; } // Current status of the call (Open, InProgress, etc.)
    public List<CallAssignmentInList>? CallAssignments { get; set; } // List of assignments for the call (past and present)
    public override string ToString() => this.ToStringProperty();
}

