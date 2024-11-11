

namespace DO;

public record Volunteer
{

    public int Id { get; set; }                  // מזהה ייחודי של המתנדב
    public string FullName { get; set; }         // שם מלא (שם פרטי ושם משפחה)
    public string PhoneNumber { get; set; }      // מספר טלפון
    public string Email { get; set; }            // כתובת דוא"ל
    public string? Password { get; set; }        // סיסמה (אם רלוונטי)
    public string? FullAddress { get; set; }     // כתובת מלאה
    public double? Latitude { get; set; }        // קו רוחב
    public double? Longitude { get; set; }       // קו אורך
    public Role Job { get; set; }                // תפקיד (מתנדב או מנהל)
    public bool Active { get; set; }             // האם המשתמש פעיל
    public double? MaxReading { get; set; }      // מרחק מקסימלי לפעילות
    public Distance TypeDistance { get; set; }   // סוג המרחק (אווירי, הליכה, נסיעה)

    // פונקציה לעדכון מיקום המתנדב
    public void UpdateLocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}

