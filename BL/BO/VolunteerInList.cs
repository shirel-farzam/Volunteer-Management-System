using DO;
using System.Text.RegularExpressions;

namespace BO;

public class VolunteerInList
{
    // Volunteer ID (from DO.Volunteer)
    public int Id { get; set; }

    // Full Name of the volunteer (from DO.Volunteer)
    public string FullName { get; set; }

    // Whether the volunteer is active (from DO.Volunteer)
    public bool Active { get; set; }

    // Total number of calls handled by the volunteer (based on successful completion)
    public int TotalCallsHandled { get; set; }

    // Total number of calls canceled by the volunteer
    public int TotalCallsCanceled { get; set; }

    // Total number of calls that expired (not handled in time) by the volunteer
    public int TotalCallsExpired { get; set; }

    // The ID of the current call the volunteer is handling (if any)
    public int? CurrentCallId { get; set; }

    // The type of the current call being handled by the volunteer (None if no active call)
    public CallType CurrentCallType { get; set; }

    // Constructor to initialize VolunteerInList
    public VolunteerInList(int id, string fullName, bool active, int totalCallsHandled, int totalCallsCanceled, int totalCallsExpired, int? currentCallId, CallType currentCallType)
    {
        Id = id;
        FullName = fullName;
        Active = active;
        TotalCallsHandled = totalCallsHandled;
        TotalCallsCanceled = totalCallsCanceled;
        TotalCallsExpired = totalCallsExpired;
        CurrentCallId = currentCallId;
        CurrentCallType = currentCallType;
    }
