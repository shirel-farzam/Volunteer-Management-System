namespace BO;

public class CallInList
{
    // Assignment ID (nullable if no assignment has been made)
    public int? Id { get; init; }

    // Unique ID of the call
    public int CallId { get; init; }

    // Type of the call
    public CallType Type { get; init; }

    // Opening time of the call
    public DateTime OpeningTime { get; init; }

    // Remaining time until the call expires
    public TimeSpan? TimeToFinish { get; init; }

    // Name of the last volunteer who handled the call
    public string? LastVolunteerName { get; init; }

    // Total time taken to complete the treatment (if applicable)
    public TimeSpan? TreatmentDuration { get; init; }

    // Current status of the call
    public CallStatus Status { get; init; }

    // Total number of assignments related to the call
    public int TotalAssignments { get; init; }
    public override string ToString() => this.ToStringProperty();


}
                                   