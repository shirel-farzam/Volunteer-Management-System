namespace BO
{

    /// Enum for specifying the sorting field for volunteers.
    //public enum VolunteerSortField
    //{
    //    ID,             // Sort by volunteer ID (ID of the volunteer)
    //    Name,           // Sort by full name (FullName of the volunteer)
    //    ActivityStatus, // Sort by activity status (whether the volunteer is active or not)
    //    SumCalls,       // Sort by total calls handled by the volunteer (SumCalls)
    //    SumCanceled,    // Sort by total calls canceled by the volunteer (Sumcanceled)
    //    SumExpired,     // Sort by total expired calls of the volunteer (SumExpired)
    //    IdCall,         // Sort by current call ID (IdCall)
    //    CType
    //}


    // Enum for Role of the volunteer (Manager or Regular Volunteer)
    public enum Role
    {
        Volunteer,   // Volunteer
        Manager         // Manager
    }

    // Enum for Distance (used for calculating distances)
    public enum Distance
    {
        Aerial,  // Air distance (straight-line distance)
        Walking,  // Walking distance
        Driving  // Driving distance
    }

    // Enum for Call Type  Combined for all Call Types
    public enum CallType
    {
        None,       // No call is being handled
        Emergency,  // Emergency call
        NonEmergency,  // Non-emergency call
        FollowUp    // Follow-up call
    }

    // Enum for Assignment Completion Type 
    public enum AssignmentCompletionType
    {
        Completed,  // Treatment was completed
        Canceled,   // Treatment was canceled
        Expired,    // Treatment expired (not handled in time)
        AdminCanceled
    }

    // Enum for Call Status (סטטוס הקריאה)
    public enum CallStatus
    {
        Open,           // The call is open (not yet handled)
        InProgress,     // The call is being handled by a volunteer
        Closed,         // The call has been completed
        Expired,        // The call has expired (not handled in time)
        OpenRisk,       // The call is open and at risk of expiration
        InProgressRisk, // The call is in progress and at risk of expiration
        None
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
    public enum CallInListField
    {
        Id,
        CallId,
        Type,
        OpeningTime,
        TimeToFinish,
        LastVolunteerName,
        TreatmentDuration,
        Status,
        TotalAssignments
    }

    public enum ClosedCallInListField
    {
        Id,
        CallType,
        FullAddress,
        OpeningTime,
        EntryTime,
        CompletionTime,
        CompletionType
    }
    public enum OpenCallInListField
    {
        Id,
        CallType,
        Description,
        FullAddress,
        OpeningTime,
        MaxCompletionTime,
        DistanceFromVolunteer
    }
    public enum VolunteerInListField
    {
        Id,
        FullName,
        Active,
        TotalCallsHandled,
        TotalCallsCanceled,
        TotalCallsExpired,
        CurrentCallId,
        CurrentCallType,
        None
    }
    public enum TypeOfCalls
    {
        FoodPreparation,  // Food preparation
        FoodTransport,    // Food transport
        InventoryCheck,   // Inventory check
    }
}
