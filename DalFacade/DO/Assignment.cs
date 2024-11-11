

namespace DO;
public class Assignment
{
    public int Id { get; set; }             // מזהה ייחודי להקצאה
    public int CallId { get; set; }         // מזהה הקריאה שהמתנדב בחר לטפל בה
    public int VolunteerId { get; set; }    // ת.ז של המתנדב שבחר לטפל בקריאה
    public DateTime TimeStart { get; set; } // זמן כניסה לטיפול (תאריך ושעה)
    public DateTime? TimeEnd { get; set; }  // זמן סיום הטיפול בפועל (תאריך ושעה)
    public TypeEnd TypeEndTreat { get; set; } // סוג סיום הטיפול (טופלה, ביטול עצמי, ביטול מנהל, ביטול פג תוקף)

}
