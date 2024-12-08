using DO;
using System.Text.RegularExpressions;

namespace BO;
public class Volunteer
{
    public int Id { get; init; }  // Volunteer ID (Cannot be updated after creation)
    public string FullName { get; set; }  // Full name of the volunteer
    public string PhoneNumber { get; set; }  // Phone number of the volunteer
    public string Email { get; set; }  // Email address of the volunteer
    public Distance TypeDistance { get; set; }  // Type of distance (Air, Walking, Driving)
    public Role Job { get; set; }  // Role of the volunteer (Manager or Volunteer)
    public bool Active { get; set; }  // Whether the volunteer is active (not retired from the organization)
    public string? Password { get; set; }  // Password for the volunteer (can be null, initially set by the manager)
    public string? FullAddress { get; set; }  // Full current address of the volunteer
    public double? Latitude { get; set; }  // Latitude coordinate of the volunteer's location (can be null)
    public double? Longitude { get; set; }  // Longitude coordinate of the volunteer's location (can be null)
    public double? MaxReading { get; set; }  // Maximum distance for receiving calls (can be null)

    public DateTime RegistrationDate { get; init; }  // Registration date of the volunteer (cannot be updated)
                                                     // Method to represent the volunteer as a string
    
   public override string ToString() => $"{FullName} ({Job}) - Active: {Active}, Registered: {RegistrationDate.ToShortDateString()}";
    // Constructor for initial password setup by the manager
    public Volunteer(string fullName, string phoneNumber, string email, string initialPassword)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Email = email;

        // Set initial password by manager, ensure it's strong
        SetPassword(initialPassword);
    }
    // Method to validate a strong password
    private bool IsPasswordStrong(string password)
    {
        // Password strength requirements: at least 8 characters, 1 uppercase, 1 number, 1 special character
        return Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
    }

    // Method to set password (with validation and encryption)
    public void SetPassword(string password)
    {
        if (IsPasswordStrong(password))
        {
            // Encrypt the password before setting it
            Password = EncryptPassword(password);
        }
        else
        {
            throw new ArgumentException("Password is not strong enough.");
        }
    }

    // Simple encryption method for demonstration (use a stronger algorithm in real-world scenarios)
    private string EncryptPassword(string password)
    {
        // Placeholder encryption: in a real-world scenario, use a stronger algorithm like BCrypt or SHA256
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
        // Override ToString method to display volunteer information as a string
        public override string ToString()
        {
            return $"Volunteer {FullName} (ID: {Id}) - Active: {Active}, Calls Handled: {TotalCallsHandled}, Calls Canceled: {TotalCallsCanceled}, Calls Expired: {TotalCallsExpired}, Current Call: {(CurrentCallType == CallType.None ? "None" : CurrentCallType.ToString())}";
        }
    }
}
