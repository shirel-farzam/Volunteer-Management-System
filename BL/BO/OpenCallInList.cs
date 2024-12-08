using DO;

namespace BO
{
    public class OpenCallInList
    {
        // Unique identifier of the open call (from DO.Call) - Cannot be null
        public int Id { get; init; }

        // The type of the call (ENUM) - Cannot be null
        public CallType CallType { get; init; }

        // A descriptive text about the call (from DO.Call) - Can be null
        public string? Description { get; init; }

        // The full address of the call (from DO.Call) - Cannot be null
        public string FullAddress { get; init; }

        // The time the call was opened (from DO.Call) - Cannot be null
        public DateTime OpeningTime { get; init; }

        // The maximum time to complete the call (from DO.Call) - Can be null
        public DateTime? MaxCompletionTime { get; init; }

        // The distance of the call from the current volunteer (calculated in the business logic layer) - Cannot be null
        public double DistanceFromVolunteer { get; set; }
        public override string ToString() => this.ToStringProperty();

    }
}