namespace Dal;

internal static class Config
{
    // הגדרת מזהה רץ לישות קריאה
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId++; }

    // הגדרת מזהה רץ לישות מתנדב
    internal const int startVolunteerId = 5000;
    private static int nextVolunteerId = startVolunteerId;
    internal static int NextVolunteerId { get => nextVolunteerId++; }

    // הגדרת מזהה רץ לישות הקצאה
    internal const int startAssignmentId = 2000;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }

    // משתנים נוספים בהתאם למשתני התצורה של המערכת
    internal static DateTime Clock { get; set; } = DateTime.Now;

    // "זמן סיכון" עבור קריאות מתקרבות לזמן סיום
    internal static TimeSpan RiskTimeSpan { get; set; } = TimeSpan.FromHours(1);

    // פונקציה לאיפוס הערכים להתחלתיים
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextVolunteerId = startVolunteerId;
        nextAssignmentId = startAssignmentId;

        // משתני תצורה נוספים לאיפוס
        Clock = DateTime.Now;
        RiskTimeSpan = TimeSpan.FromHours(1);
    }
}
