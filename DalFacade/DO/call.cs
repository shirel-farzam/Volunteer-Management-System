

namespace DO;

public class Call
{
    public int Id { get; set; }                  // מזהה ייחודי לקריאה
    public CallType Type { get; set; }            // סוג הקריאה
    public string Description { get; set; }       // תיאור מילולי של הקריאה
    public string FullAddress { get; set; }       // כתובת מלאה של הקריאה
    public double Latitude { get; set; }          // קו רוחב של מיקום הקריאה
    public double Longitude { get; set; }         // קו אורך של מיקום הקריאה
    public DateTime TimeOpened { get; set; }      // זמן פתיחת הקריאה
    public DateTime? MaxTimeToClose { get; set; } // זמן מקסימלי לסיום הקריאה (אם יש)

    
}