namespace DO;
public record Assignment
(
    int Id,           // מזהה ייחודי להקצאה
    int CallId,        // מזהה הקריאה שהמתנדב בחר לטפל בה
    int VolunteerId,   // ת.ז של המתנדב שבחר לטפל בקריאה
    DateTime TimeStart, /// זמן כניסה לטיפול (תאריך ושעה)
   DateTime? TimeEnd = null, // זמן סיום הטיפול בפועל (תאריך ושעה)
   TypeEnd? TypeEndTreat = null// סוג סיום הטיפול (טופלה, ביטול עצמי, ביטול מנהל, ביטול פג תוקף)
)
{ public Assignment() : this(0, 0, 0, default(DateTime)) { } }





