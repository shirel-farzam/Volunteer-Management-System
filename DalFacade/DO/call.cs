

namespace DO;


public record Call
(

        int Id,
        CallType Type,
        string Description,
        string FullAddress,
        double Latitude,
        double Longitude,
        DateTime TimeOpened,
        DateTime? MaxTimeToClose = null
   )
{
    /// <summary>
    /// Default constructor for Call with default values
    /// </summary>
    public Call() : this(0, default(CallType), "", "", 0, 0, DateTime.MinValue) { }
}
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
