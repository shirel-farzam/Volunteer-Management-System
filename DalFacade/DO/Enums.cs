

namespace DO;

public enum Role
{
    Volunteer,   // מתנדב
    Boss         // מנהל
}

// הגדרת Enum עבור סוג המרחק
public enum Distance
{
    Aerial,      // מרחק אווירי
    Walking,     // מרחק הליכה
    Driving      // מרחק נסיעה
}
public enum CallType
{
    FoodPreparation,  // הכנת אוכל
    FoodTransport,    // שינוע אוכל
                      // ניתן להוסיף סוגים נוספים לפי הצורך
}
public enum TypeEnd
{
    Treated,          // טופלה
    SelfCancel,       // ביטול עצמי
    ManagerCancel,    // ביטול מנהל
    ExpiredCancel     // ביטול פג תוקף
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
           
        }
    }

public void Create(Course item)
{
    //for entities with auto id
    int id = DataSource.Config.NextCourseId;
    Course copy = item with { Id = id };
    DataSource.Courses.Add(copy);


    NotFiniteNumberException

        





  
}

    
     