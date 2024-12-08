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
    public DateTime RegistrationDate { get; init; }
    public override string ToString() => this.ToStringProperty();


    //// Method to represent the volunteer as a string
    //public override string ToString() =>
    //    $"{FullName} ({Job}) - Active: {Active}, Registered: {RegistrationDate.ToShortDateString()}";

    //// Constructor for initial password setup by the manager
    //public Volunteer(string fullName, string phoneNumber, string email, string initialPassword)
    //{
    //    FullName = fullName;
    //    PhoneNumber = phoneNumber;
    //    Email = email;

    //    // Set initial password by manager, ensure it's strong
    //    SetPassword(initialPassword);
    //}
    //// Method to validate a strong password
    //private bool IsPasswordStrong(string password)
    //{
    //    // Password strength requirements: at least 8 characters, 1 uppercase, 1 number, 1 special character
    //    return Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
    //}

    //// Method to set password (with validation and encryption)
    //public void SetPassword(string password)
    //{
    //    if (IsPasswordStrong(password))
    //    {
    //        // Encrypt the password before setting it
    //        Password = EncryptPassword(password);
    //    }
    //    else
    //    {
    //        throw new ArgumentException("Password is not strong enough.");
    //    }
    //}

    //// Simple encryption method for demonstration (use a stronger algorithm in real-world scenarios)
    //private string EncryptPassword(string password)
    //{
    //    // Placeholder encryption: in a real-world scenario, use a stronger algorithm like BCrypt or SHA256
    //    return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    //}

}

