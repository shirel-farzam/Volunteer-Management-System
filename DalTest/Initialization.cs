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

}
