using System.Runtime.CompilerServices;

namespace Dal;

/// <summary>
/// Static class for managing configuration settings and constants for the DAL layer.
/// </summary>
internal static class Config
{
    /// <summary>
    /// File path for the data configuration XML file.
    /// </summary>
    internal const string s_data_config_xml = "data-config.xml";

    /// <summary>
    /// File path for the volunteers XML file.
    /// </summary>
    internal const string s_volunteers_xml = "volunteers.xml";

    /// <summary>
    /// File path for the calls XML file.
    /// </summary>
    internal const string s_calls_xml = "calls.xml";

    /// <summary>
    /// File path for the assignments XML file.
    /// </summary>
    internal const string s_assignments_xml = "assignments.xml";

    /// <summary>
    /// Property to get and increase the next call ID from the configuration file.
    /// </summary>
    internal static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");

        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    /// <summary>
    /// Property to get and increase the next assignment ID from the configuration file.
    /// </summary>
    internal static int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentID");

        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentID", value);
    }

    internal const int startVolunteerId = 1000;

    internal const int startAssignmentId = 1000;

    /// <summary>
    /// Property to get and set the system clock value.
    /// </summary>
    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");

        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    /// <summary>
    /// Property to get and set the risk range value.
    /// </summary>
    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
        
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7 
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
    }

    /// <summary>
    /// Resets the configuration to default values.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        // Reset IDs
        NextCallId = startVolunteerId;
        NextAssignmentId = startAssignmentId;

        // Reset Clock to the current time
        Clock = DateTime.Now;

        // Reset RiskRange to a default value (e.g., 0)
        RiskRange = TimeSpan.FromHours(1);
    }
}
