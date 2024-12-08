namespace BO;
public class ClosedCallInList
{
    // Unique identifier of the closed call (from DO.Call) - Cannot be null
    public int Id { get; init; }

    // The type of the call (ENUM) - Cannot be null
    public CallType CallType { get; init; }

    // The full address of the call (from DO.Call) - Cannot be null
    public string FullAddress { get; init; }

    // The time the call was opened (from DO.Call) - Cannot be null
    public DateTime OpeningTime { get; init; }

    // The time the volunteer started handling the call (from DO.Assignment) - Cannot be null
    public DateTime EntryTime { get; init; }

    // The actual time the treatment ended (from DO.Assignment) - Can be null
    public DateTime? CompletionTime { get; set; }

    // The type of the call completion (ENUM) - Can be null
    public AssignmentCompletionType? CompletionType { get; set; }
    public override string ToString() => this.ToStringProperty();

}
