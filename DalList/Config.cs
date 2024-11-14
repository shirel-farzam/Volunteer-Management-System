namespace Dal;

internal static class Config
{
    // הגדרת מזהה רץ לישות קריאה
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId++; }

    // הגדרת מזהה רץ לישות מתנדב
    internal const int startVolunteerId = 5000;

    // הגדרת מזהה רץ לישות הקצאה
    internal const int startAssignmentId = 2000;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }

    // משתנים נוספים בהתאם למשתני התצורה של המערכת
    internal static DateTime Clock { get; set; } = DateTime.Now;

    // "זמן סיכון" עבור קריאות מתקרבות לזמן סיום
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);

    // פונקציה לאיפוס הערכים להתחלתיים
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;

        // משתני תצורה נוספים לאיפוס
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }
}
