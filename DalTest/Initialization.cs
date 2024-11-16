namespace DalTest;
using DalApi;
using DO;
public static class Initialization
{
    private static IVolunteer? s_Volunteer; //stage 1
    private static ICall? s_Call; //stage 1
    private static IAssignment? s_Assignment; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();

    private static void CreateVolunteer()
    {
        string[] VolunteerName = { "Noa Levy", "Omer Cohen", "Rotem Mizrahi",
                           "Yoav Biton", "Maya Peretz", "Daniel Avrahami", "Tamar Malka",
                           "Yael Adi", "Ron Hazan", "Yonatan Green", "Michal Cohen",
                           "Yuval Levy", "Ofir Dayan", "Alon Goldberg", "Shira Sharabi" };

        string[] PhoneNumber = { "054-1234567", "053-2345678", "054-3456789", "050-4567890",
                         "055-5678901", "054-6789012", "052-7890123", "054-8901234",
                         "054-9012345", "053-0123456", "054-1234568", "050-2345679",
                         "058-3456780", "054-4567891", "055-5678902" };

        string[] Email = { "noa.levy@gmail.com", "omer.cohen@gmail.com", "rotem.mizrahi@gmail.com",
                   "yoav.biton@gmail.com", "maya.peretz@gmail.com", "daniel.avrahami@gmail.com",
                   "tamar.malka@gmail.com", "yael.adi@gmail.com", "ron.hazan@gmail.com",
                   "yonatan.green@gmail.com", "michal.cohen@gmail.com", "yuval.levy@gmail.com",
                   "ofir.dayan@gmail.com", "alon.goldberg@gmail.com", "shira.sharabi@gmail.com" };
        for (int i = 0; i < VolunteerName.Length; i++)
        {
            int id;
            do
                id = s_rand.Next(700000000, 1000000000); // ת.ז אקראית עם 9 ספרות
            while (s_Volunteer!.Read(id) != null); // בדיקת ייחודיות של ת.ז.

            string name = VolunteerName[i];
            string phone = PhoneNumber[i];
            string email = Email[i];
            Distance distanceType = Distance.Aerial; // מרחק ברירת מחדל
            Role role = Role.Volunteer; // ברירת מחדל - מתנדב רגיל
            bool active = true; // המתנדב פעיל כברירת מחדל
            double maxReading = s_rand.Next(5, 100); // מרחק מקסימלי אקראי בין 5 ל-100

            s_Volunteer!.Create(new Volunteer(id, name, phone, email, distanceType, role, active, null, null, null, null, maxReading));
        }

        // הוספת מנהל אחד לפחות
        int managerId;
        do
            managerId = s_rand.Next(100000000, 1000000000);
        while (s_Volunteer!.Read(managerId) != null);

        s_Volunteer!.Create(new Volunteer(managerId, "Admin Manager", "050-1111111", "admin@gamil.com", Distance.Aerial, Role.Boss, true, "Password246"));

    }
    private static void CreateCalls()
    {
       
        DateTime start = new DateTime(s_dalConfig.Clock.Year , s_dalConfig.Clock.Month, s_dalConfig.Clock.Day-7); //stage 1
        int range = (s_dalConfig.Clock - start).Minutes; //stage 1
        DateTime bdt = start.AddMinutes(s_rand.Next(range));

    }
    private static void createCalls()
    {
        // מאגר של כתובות עם קווי אורך ורוחב אמיתיים
        var locations = new (string Address, double Latitude, double Longitude)[]
        {
        ("123 Main St, City", 32.109333, 34.855499),
        ("456 Elm St, City", 32.105699, 34.851502),
        ("789 Oak St, City", 32.095485, 34.832389),
        ("101 Pine St, City", 32.077098, 34.791331),
        ("202 Maple St, City", 32.054198, 34.774587),
        ("303 Birch St, City", 32.072294, 34.817534)
        };

        // מאגר של תיאורי קריאות וסוגים
        string[] descriptions = { "Electric issue", "Water leakage", "Fire alarm", "Noise complaint", "Gas leak", "Lost item" };
        CallType[] callTypes = { CallType.Emergency, CallType.Technical, CallType.Service, CallType.General };

        foreach (var description in descriptions)
        {
            // קבלת מזהה ייחודי מתוך הקונפיגורציה
            int id = s_dalConfig.GetNextCallId();

            // בחירת סוג קריאה וכתובת אקראית
            CallType type = callTypes[s_rand.Next(callTypes.Length)];
            var location = locations[s_rand.Next(locations.Length)];

            // זמן פתיחת הקריאה רנדומלי, לפני הזמן הנוכחי
            DateTime timeOpened = s_dalConfig.Clock.AddMinutes(-s_rand.Next(10, 1440)); // עד יום אחורה

            // זמן סיום רנדומלי או null
            DateTime? maxTimeToClose = s_rand.NextDouble() < 0.7
                ? timeOpened.AddHours(s_rand.Next(1, 24))
                : (DateTime?)null;

            // יצירת אובייקט Call והוספתו לרשימה
            s_dalCall!.Create(new Call
            {
                Id = id,
                Type = type,
                Description = description,
                FullAddress = location.Address,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                TimeOpened = timeOpened,
                MaxTimeToClose = maxTimeToClose
            });
        }
    }
    private static void createAssignments()
    {
        // משתנה המייצג סיכוי טיפול
        double chanceOfAssignment = 0.8;

        foreach (var call in s_dalCall!.ReadAll())
        {
            // מגרילים אם תהיה הקצאה לקריאה זו
            if (s_rand.NextDouble() > chanceOfAssignment) continue;

            // מזהה הקצאה מתוך הקונפיגורציה
            int assignmentId = s_dalConfig.NextAssignmentId();

            // הגרלת מתנדב קיים לפי ת.ז
            int volunteerId;
            do
                volunteerId = s_rand.Next(MIN_ID, MAX_ID);
            while (s_dalVolunteer!.Read(volunteerId) == null); // בדיקה שהמתנדב קיים

            // קביעת זמן כניסה לטיפול לאחר זמן פתיחה
            DateTime startTreatment = call.TimeOpened.AddMinutes(s_rand.Next(1, 120)); // עד שעתיים אחרי פתיחה

            // זמן סיום טיפול (עשוי להיות אחרי זמן מקסימלי או null)
            DateTime? endTreatment = null;
            if (call.MaxTimeToClose.HasValue)
            {
                endTreatment = startTreatment.AddMinutes(s_rand.Next(1, 180)); // עד 3 שעות אחרי התחלה
                if (endTreatment > call.MaxTimeToClose)
                {
                    endTreatment = null; // זמן סיום שלא תואם את הזמן המקסימלי של הקריאה
                }
            }

            // סוג סיום טיפול
            string closeType;
            if (endTreatment.HasValue)
            {
                closeType = endTreatment <= call.MaxTimeToClose ? "טופלה" : "ביטול מנהל";
            }
            else
            {
                closeType = "בטיפול";
            }

            // יצירת אובייקט Assignment והוספתו לרשימה
            s_dalAssignment!.Create(new Assignment
            {
                Id = assignmentId,
                CallId = call.Id,
                VolunteerId = volunteerId,
                StartTreatment = startTreatment,
                EndTreatment = endTreatment,
                CloseType = closeType
            });
        }
    }





}
