namespace BO;

public class CallInProgress
{
    // The unique identifier of the assignment entity (cannot be changed after initialization)
    public int Id { get; init; }

    // The unique identifier of the call entity (cannot be changed after initialization)
    public int CallId { get; init; }

    // The type of the call (ENUM, cannot be changed after initialization)
    public CallType CallType { get; init; }

    // A descriptive text about the call (mutable)
    public string? Description { get; set; }

    // The full address of the call (cannot be changed after initialization)
    public string FullAddress { get; init; }

    // The time the call was opened (cannot be changed after initialization)
    public DateTime OpeningTime { get; init; }

    // The maximum time to complete the call (nullable, mutable)
    public DateTime? MaxCompletionTime { get; set; }

    // The time the volunteer started handling the call (cannot be changed after initialization)
    public DateTime EntryTime { get; init; }

    // The distance of the call from the current location of the handling volunteer (mutable, may change dynamically)
    public double DistanceFromVolunteer { get; set; }

    // The status of the call (ENUM, mutable, can change as the status updates)
    public CallStatus Status { get; set; }
    public override string ToString() => this.ToStringProperty();

}
