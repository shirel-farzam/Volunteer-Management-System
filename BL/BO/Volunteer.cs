namespace BO;
using Helpers;
public class Volunteer
{
    // ID of the volunteer (cannot be updated after creation)
    public int Id { get; init; }

    // Full name of the volunteer
    public string FullName { get; set; }

    // Phone number of the volunteer
    public string PhoneNumber { get; set; }

    // Email address of the volunteer
    public string Email { get; set; }

    // Password of the volunteer (can be null, initially set by the manager)
    public string? Password { get; set; }

    // Full current address of the volunteer
    public string? FullAddress { get; set; }

    // Latitude coordinate (can be null)
    public double? Latitude { get; set; }

    // Longitude coordinate (can be null)
    public double? Longitude { get; set; }

    // Role of the volunteer (Manager or Volunteer)
    public Role Job { get; set; }

    // Whether the volunteer is active or not
    public bool Active { get; set; }

    // Maximum distance for receiving calls (can be null)
    public double? MaxReading { get; set; }

    // Type of distance (Air, Walking, Driving)
    public Distance TypeDistance { get; set; }

    // Total calls handled by the volunteer
    public int TotalHandledCalls { get; set; }

    // Total canceled calls by the volunteer
    public int TotalCanceledCalls { get; set; }

    // Total expired calls by the volunteer
    public int TotalExpiredCalls { get; set; }

    // Current call in progress (nullable)
    public CallInProgress? CurrentCall { get; set; }

    // Registration date (immutable)
    public override string ToString() => this.ToStringProperty();
}

