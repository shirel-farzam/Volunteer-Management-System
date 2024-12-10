using DO;
using System.Text.RegularExpressions;

namespace BO;

public class VolunteerInList
{
    // Volunteer ID (from DO.Volunteer) - cannot be changed after initialization
    public int Id { get; init; }

    // Full Name of the volunteer (from DO.Volunteer) - cannot be changed after initialization
    public string FullName { get; init; }

    // Whether the volunteer is active (from DO.Volunteer) - cannot be changed after initialization
    public bool Active { get; init; }

    // Total number of calls handled by the volunteer (based on successful completion) - mutable, can change over time
    public int TotalCallsHandled { get; set; }

    // Total number of calls canceled by the volunteer - mutable, can change over time
    public int TotalCallsCanceled { get; set; }

    // Total number of calls that expired (not handled in time) by the volunteer - mutable, can change over time
    public int TotalCallsExpired { get; set; }

    // The ID of the current call the volunteer is handling (if any) - nullable, mutable
    public int? CurrentCallId { get; set; }
  
    public override string ToString() => this.ToStringProperty();


}
