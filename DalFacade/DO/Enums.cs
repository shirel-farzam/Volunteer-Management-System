

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


    
     