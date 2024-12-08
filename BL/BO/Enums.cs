namespace BO
{

    /// Enum for specifying the sorting field for volunteers.
    public enum VolunteerSortField
    {
        ID,           // Sort by volunteer ID
        Name,         // Sort by volunteer name
        JoinDate,     // Sort by the date the volunteer joined
        ActivityStatus // Sort by the activity status (active/inactive)
    }
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
    // Enum for specifying the field to filter or sort by for calls
    public enum CallField
    {
        CallId,          // Filter or sort by the unique ID of the call
        Description,     // Filter or sort by the description of the call
        Status,          // Filter or sort by the current status of the call
        AssignmentDate   // Filter or sort by the assignment date of the call
    }
    public enum TimeUnit
    {
        MINUTE,  // Minute unit for time advancement
        HOUR,    // Hour unit for time advancement
        DAY,     // Day unit for time advancement
        MONTH,   // Month unit for time advancement
        YEAR     // Year unit for time advancement
    }
}
