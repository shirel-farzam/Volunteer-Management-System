

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
public void Create(T item)
{
    // בדיקה אם כבר קיים מתנדב עם אותו מזהה (למניעת כפילות)
    if (volunteers.Any(existingVolunteer => existingVolunteer.Id == volunteer.Id && volunteer.Id != 0))
    {
        throw new InvalidOperationException($"מתנדב עם תעודת זהות {volunteer.Id} כבר קיים במערכת.");
    }

    // אם המזהה הוא ברירת מחדל (0), יוצרים מזהה רץ חדש
    if (volunteer.Id == 0)
    {
        // יצירת מזהה חדש בעזרת המונה הבא מ-DataSource.Config
        int newId = DataSource.Config.GetNextId();

        // יצירת עותק של האובייקט עם המזהה החדש
        Volunteer newVolunteer = volunteer with { Id = newId };

        // הוספת האובייקט החדש לרשימת המתנדבים
        volunteers.Add(newVolunteer);

        //return newId; // החזרת המזהה החדש?????????????..
    }
    else
    {
        // הוספת האובייקט כפי שהוא לרשימת המתנדבים (עם מזהה ייחודי שהוגדר מראש)
        volunteers.Add(volunteer);
        //????????????????????????????return volunteer.Id; // החזרת המזהה הקיים
    }
}




