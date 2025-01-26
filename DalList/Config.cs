namespace Dal;
using DO;
using System.Runtime.CompilerServices;

public static class Config
{
    // Define a starting ID for the Call entity
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    
    public static int NextCallId {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => nextCallId++; }

    // Define a starting ID for the Volunteer entity
    internal const int startVolunteerId = 1000;

    // Define a starting ID for the Assignment entity
    internal const int startAssignmentId = 1000;
    private static int nextAssignmentId = startAssignmentId;
    public static int NextAssignmentId {

        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => nextAssignmentId++; }

    // Additional variables based on the system configuration
    internal static DateTime Clock {
        [MethodImpl(MethodImplOptions.Synchronized)]  get;
        [MethodImpl(MethodImplOptions.Synchronized)] set; } = DateTime.Now;

    // "Risk time" for calls approaching their end time
    internal static TimeSpan RiskRange {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get; [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set; } = TimeSpan.FromHours(1);

    // Function to reset values to their initial states
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;
        // Additional configuration variables to reset
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }
}
