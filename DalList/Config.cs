namespace Dal;
using DO;
public static class Config
{
    // Define a starting ID for the Call entity
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    public static int NextCallId { get => nextCallId++; }

    // Define a starting ID for the Volunteer entity
    internal const int startVolunteerId = 5000;

    // Define a starting ID for the Assignment entity
    internal const int startAssignmentId = 2000;
    private static int nextAssignmentId = startAssignmentId;
    public static int NextAssignmentId { get => nextAssignmentId++; }

    // Additional variables based on the system configuration
    internal static DateTime Clock { get; set; } = DateTime.Now;

    // "Risk time" for calls approaching their end time
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);

    // Function to reset values to their initial states
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;
        // Additional configuration variables to reset
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }
}
