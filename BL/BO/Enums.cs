namespace BO
{
    // Enum for Role of the volunteer (Manager or Regular Volunteer)
    public enum Role
    {
        Manager,  // The volunteer is a manager
        Volunteer  // The volunteer is a regular volunteer
    }

    // Enum for Distance (used for calculating distances)
    public enum Distance
    {
        Distance,  // Air distance (straight-line distance)
        Walking,  // Walking distance
        Driving  // Driving distance
    }

    // Enum for Call Type (קריאה) - Combined for all Call Types
    public enum CallType
    {
        None,       // No call is being handled
        Emergency,  // Emergency call
        NonEmergency,  // Non-emergency call
        FollowUp    // Follow-up call
    }

    // Enum for Assignment Completion Type (סוג סיום הטיפול)
    public enum AssignmentCompletionType
    {
        Completed,  // Treatment was completed
        Canceled,   // Treatment was canceled
        Expired     // Treatment expired (not handled in time)
    }

    // Enum for Call Status (סטטוס הקריאה)
    public enum CallStatus
    {
        Open,           // The call is open (not yet handled)
        InProgress,     // The call is being handled by a volunteer
        Closed,         // The call has been completed
        Expired,        // The call has expired (not handled in time)
        OpenRisk,       // The call is open and at risk of expiration
        InProgressRisk // The call is in progress and at risk of expiration
    }

    // Class to represent the current call being handled by the volunteer
    public class CallInProgress
    {
        public int CallId { get; set; }  // Unique ID for the call
        public string? Description { get; set; }  // Description of the call
        public DateTime StartTime { get; set; }  // Time when the call started
        // Additional properties as needed
    }
}
