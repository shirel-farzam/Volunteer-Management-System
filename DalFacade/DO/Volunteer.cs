
namespace DO;
/// <summary>
/// Volunteer Entity represents a volunteer with all its properties.
/// </summary>
/// <param name="Id">Unique ID of the volunteer</param>
/// <param name="FullName">Full name of the volunteer</param>
/// <param name="PhoneNumber">Phone number of the volunteer</param>
/// <param name="Email">Email address of the volunteer</param>
/// <param name="TypeDistance">Preferred type of distance calculation (Aerial, Walking, Driving)</param>
/// <param name="Job">Role of the volunteer (e.g., Volunteer, Boss)</param>
/// <param name="Active">Indicates whether the volunteer is active</param>
/// <param name="Password">Password for the volunteer's account (optional)</param>
/// <param name="FullAddress">Full address of the volunteer (optional)</param>
/// <param name="Latitude">Latitude of the volunteer's location (optional)</param>
/// <param name="Longitude">Longitude of the volunteer's location (optional)</param>
/// <param name="MaxReading">Maximum reading distance for the volunteer (optional)</param>


public record Volunteer
(

     int Id,
     string FullName,
     string PhoneNumber,
     string Email,
     Distance TypeDistance,
     Role Job,
     bool Active,
     string? Password = null,
     string? FullAddress = null,
     double? Latitude = null,
     double? Longitude = null,
     double? MaxReading = null

)
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Volunteer() : this(0, "", "", "", default(Distance), default(Role), false) { }

}

    