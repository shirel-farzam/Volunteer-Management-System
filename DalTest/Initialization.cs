namespace DalTest;
using DalApi;
using Dal;
using DO;
using System.Diagnostics;

public static class Initialization
{
    //private static IVolunteer? s_Volunteer; //stage 1 - Interface for handling volunteer operations
    //private static ICall? s_Call; //stage 1 - Interface for handling call operations
    //private static IAssignment? s_Assignment; //stage 1 - Interface for handling assignment operations
    //private static IConfig? s_dal.Config; //stage 1 - Interface for configuration data
    //private static readonly Random s_rand = new(); // Random instance for generating random values

    

    private static IDal? s_dal; //stage 2
    private static readonly Random s_rand = new();
    private const int MIN_ID = 200000000;
    private const int MAX_ID = 400000000;

    // Method to create volunteer data
    private static void CreateVolunteer()
    {

        int[] Id = { 214429151, 214432767, 214485302, 214545303, 214554958,
                     214616104, 214631426, 214664260, 214675688, 214736456,
                     214736688, 214763252, 214773020, 214776742, 214786972 };

        // Array of volunteer names
        string[] VolunteerName = { "Noa Levy", "Omer Cohen", "Rotem Mizrahi",
                           "Yoav Biton", "Maya Peretz", "Daniel Avrahami", "Tamar Malka",
                           "Yael Adi", "Ron Hazan", "Yonatan Green", "Michal Cohen",
                           "Yuval Levy", "Ofir Dayan", "Alon Goldberg", "Shira Sharabi" };

        // Array of phone numbers corresponding to the volunteer names
        string[] PhoneNumber = { "0541234567", "0532345678", "0543456789",
                                 "0504567890", "0555678901", "0546789012", "0527890123",
                                 "0548901234", "0549012345", "0530123456", "0541234568",
                                 "0502345679", "0583456780", "0544567891", "0555678902" };

        // Array of email addresses corresponding to the volunteer names
        string[] Email = { "noa.levy@example.com", "omer.cohen@example.com", "rotem.mizrahi@example.com",
                   "yoav.biton@example.com", "maya.peretz@example.com", "daniel.avrahami@example.com",
                   "tamar.malka@example.com", "yael.adi@example.com", "ron.hazan@example.com",
                   "yonatan.green@example.com", "michal.cohen@example.com", "yuval.levy@example.com",
                   "ofir.dayan@example.com", "alon.goldberg@example.com", "shira.sharabi@example.com" };


        // Array of volunteer addresses (strings)
        string[] addresses =
        {
            "Rothschild Boulevard 1, Tel Aviv, Israel",
"King George 10, Haifa, Israel",
"Herzl Street 5, Netanya, Israel",
"Jabotinsky Street 20, Petah Tikva, Israel",
"Ben Gurion Boulevard 50, Ashdod, Israel",
"Yigal Alon Street 30, Tel Aviv, Israel",
"Independence Square 8, Hadera, Israel",
"David HaMelech Street 12, Eilat, Israel",
"Hanegev Boulevard 3, Beersheba, Israel",
"Emek Refaim 43, Jerusalem, Israel",
"HaAtzmaut Boulevard 2, Bat Yam, Israel",
"Allenby Street 35, Tel Aviv, Israel",
"Namal Street 4, Ashkelon, Israel",
"Sokolov Street 22, Rishon LeZion, Israel",
"Hertzel Boulevard 9, Holon, Israel"

        };

        // Array of longitude coordinates
        double[] Longitudes = new double[]
        {
          34.7688262, 34.995336,35.125519,34.770817,34.651074
           , 34.811118,34.91956,34.94821,34.785262,35.2179409,
           34.779417,34.772842,34.4091,34.776051, 34.780119
        };

        // Array of latitude coordinates
        double[] Latitudes = new double[]
        {
           32.0631559,32.792761,32.303342,32.089587,31.791658,
            32.109166,32.43645, 29.55805,31.257023,31.7619324,
            32.096611, 32.062991,31.437793,32.062593, 32.008484
        };
        string[] passwords =
{
    "W1nter2024",
    "M@rk3tX9",
    "Go!4ItNow",
    "H@ppyD@y1s",
    "S#1lverKey",
    "GooD1234",
    "Tricky5",
    "R3liable",
    "C0d3!2024",
    "J@vaC0d3r",
    "Adm!n$2024",
    "S@lute1",
    "3asY_p@ss",
    "BlueSky9",
    "Zaq12wsx"
};

        // Loop to create volunteer data and add it to the data source
        for (int i = 0; i < 15; i++)
        {
            int Currid = Id[i];
            string name = VolunteerName[i];
            string phone = PhoneNumber[i];
            string email = Email[i];
            string Address= addresses[i];
            string password= passwords[i];
            double NLatitude = Latitudes[i];
            double NLongitude = Longitudes[i];
            Distance distanceType = Distance.Aerial; // Default distance type
            Role role = Role.Volunteer; // Default role - regular volunteer
            bool active = true; // Volunteer is active by default
            double maxReading = s_rand.Next(10, 100); // Generate a random max reading between 5 and 100

            //Create a new Volunteer object and add it to the data source
            s_dal!.Volunteer.Create(new Volunteer(Currid, name, phone, email, distanceType, role, active, password, Address, NLatitude, NLongitude, maxReading));
        }

       

        // Create a new manager and add to the data source
        s_dal!.Volunteer.Create(new Volunteer(326047701, "Ayelet Benisty", "0556882236", "admin@example.com", Distance.Aerial, Role.Manager, true, "Zaq12wsx","Ein Gedi 37,Beer sheva, Israel", 31.2509036, 34.7668511, 50));
    }

    static int p = 0;
        static int T = 0;
       static int I = 0;
    private static void CreateCalls()
    {
        // Array of descriptions for different types of calls
        string[] DescriptionsP = {
    "Family needs a hot meal in the city center.",
    "Patient waiting for a meal arranged near the hospital.",
    "Family in a difficult situation, urgently needs a hot meal.",
    "Family requested an immediate meal to be delivered to a nearby location.",
    "Patient in late-stage illness, meal urgently needed near the clinic.",
    "Family requesting a hot meal for lunch in their residential area.",
    "Family needs a hot meal after medical treatment at the hospital.",
    "Meal needed for a patient having difficulty moving, near a busy street.",
    "Hot meal requested for a family in an area without cooking facilities.",
    "Family needs a hot meal with a special diet option.",
    "Patient living in an assisted living facility, requires meals in the evening.",
    "Family expected a hot meal near the military base.",
    "Family requested support with meals for a week due to chronic illness.",
    "Hot meal needed for a patient waiting to be discharged from the hospital.",
    "Family requests a hot meal for the whole day due to intensive treatment.",
    "Family needs a hot meal in the suburban area.",
    "Family needs a meal for a patient in a therapy pool.",
    "Family needs a hot meal in the evening for a patient after surgery.",
    "Hot meal requested for the parents of a patient waiting for medical tests.",
    "Family requests a meal immediately for the afternoon hours."
};
        // Additional descriptions for food preparation assistance calls
        string[] DescriptionsL = {
    "Family locked out of the kitchen after patient left for the hospital.",
    "Patient alone at home, needs assistance with preparing a meal.",
    "Family needs a holiday meal but has no access to the kitchen.",
    "Patient in serious condition, family urgently needs a hot meal.",
    "Family unable to prepare food after leaving the clinic.",
    "Patient in serious condition, meal must be tailored to a specific diet.",
    "Family unable to prepare food and needs immediate help.",
    "Assistance needed in preparing food for a family member at the hospital.",
    "Patient at home in treatment, requires a hot meal urgently.",
    "Family unable to cook and needs immediate help with a meal.",
    "Patient isolated at home, urgently needs a hot meal.",
    "Family requests help preparing a meal at night.",
    "Patient in serious condition and requesting help preparing food at home."
};
        // Additional descriptions for call types related to meal preparation support
        string[] DescriptionsC = {
    "Patient needs a hot meal after finishing medical treatment at the hospital.",
    "Family needs help preparing meals due to difficulties in the kitchen.",
    "The patient requires food support after a long period of medical treatment.",
    "Family unable to prepare food after a difficult surgery.",
    "Patient needs a hot meal after a prolonged clinic treatment.",
    "Family requests a hot meal for someone who is hospitalized for several days.",
    "Family unable to prepare meals after the patient was hospitalized in critical condition.",
    "Family needs assistance in preparing meals for a patient at home.",
    "Patient in critical condition needs a hot meal immediately after medical treatment.",
    "Family requests help with preparing food for a patient requiring daily support.",
    "Family needs help with preparing a meal for a patient in long-term hospitalization.",
    "Patient unable to prepare food on their own, needs a hot meal at home.",
    "Family requests help preparing an evening meal for an elderly patient.",
    "Patient and family need assistance with meal preparation for an extended period.",
    "Family requests support with preparing a hot meal for a patient with daily struggles.",
    "Patient requires an immediate meal after a complex treatment at the clinic.",
    "Family needs food assistance for a patient waiting at home for medical tests.",
    "Patient needs a hot meal after an extended hospitalization.",
    "Family requests help preparing meals for a patient during home medical care."
};
        // arry of addreses
        string[] addresses = new string[]
        {
    "King David St 12, Jerusalem, Israel",
    "Keren Hayesod St 15, Jerusalem, Israel",
    "HaRav Kook St 20, Jerusalem, Israel",
    "Strauss St 5, Jerusalem, Israel",
    "Radak St 10, Jerusalem, Israel",
    "Bezalel St 35, Jerusalem, Israel",
    "Shmuel HaNagid St 18, Jerusalem, Israel",
    "David HaMelech St 6, Jerusalem, Israel",
    "Shivtei Israel St 10, Jerusalem, Israel",
    "HaMesila Park, Jerusalem, Israel",
    "Rabbi Akiva St 8, Jerusalem, Israel",
    "Haneviim St 50, Jerusalem, Israel",
    "Yemin Moshe St 8, Jerusalem, Israel",
    "Yoel Moshe Salomon St 22, Jerusalem, Israel",
    "HaOren St 7, Jerusalem, Israel",
    "Beit Hakerem St 25, Jerusalem, Israel",
    "Givat Shaul St 10, Jerusalem, Israel",
    "Shlomo Zalman Shragai St 5, Jerusalem, Israel",
    "Emek Refaim St 12, Jerusalem, Israel",
    "Azza St 40, Jerusalem, Israel",
    "Derech Har HaTsofim 20, Jerusalem, Israel",
    "Mount Scopus Campus, Jerusalem, Israel",
    "Nablus Rd 10, Jerusalem, Israel",
    "Hebron Rd 50, Jerusalem, Israel",
    "HaPalmach St 20, Jerusalem, Israel",
    "Lincoln St 5, Jerusalem, Israel",
    "Duvdevani St 10, Jerusalem, Israel",
    "Diskin St 12, Jerusalem, Israel",
    "Alkalai St 15, Jerusalem, Israel",
    "Ramban St 20, Jerusalem, Israel",
    "Mordechai Ben Hillel St 12, Jerusalem, Israel",
    "HaRav Herzog St 60, Jerusalem, Israel",
    "Gershon Agron St 12, Jerusalem, Israel",
    "Givon St 5, Jerusalem, Israel",
    "Golda Meir Blvd 80, Jerusalem, Israel",
    "Lev Ram Blvd 5, Jerusalem, Israel",
    "Harav Shach St 10, Jerusalem, Israel",
    "Kiryat HaLeom, Jerusalem, Israel",
    "Shaarei Tsedek St 3, Jerusalem, Israel",
    "Givat Mordechai St 10, Jerusalem, Israel",
    "Bayit Vegan St 10, Jerusalem, Israel",
    "Sanhedria St 12, Jerusalem, Israel",
    "Bar Ilan St 20, Jerusalem, Israel",
    "Shmuel Hanavi St 45, Jerusalem, Israel",
    "Malha Rd 5, Jerusalem, Israel",
    "Pisgat Ze'ev Blvd 10, Jerusalem, Israel",
    "Teddy Stadium, Jerusalem, Israel",
    "Zahal St 7, Jerusalem, Israel",
    "Ha'Arazim Blvd 5, Jerusalem, Israel",
    "Ramot Forest, Jerusalem, Israel",
    "Eliyahu Bashan St 8, Jerusalem, Israel"
        };

        // arry of longitudes
        double[] longitudes = new double[]
        {
    35.2134, 35.2153, 35.2135, 35.2110, 35.2123,
    35.2201, 35.2120, 35.2145, 35.2159, 35.2206,
    35.2221, 35.2113, 35.2176, 35.2138, 35.2109,
    35.2118, 35.2082, 35.2047, 35.2165, 35.2147,
    35.2202, 35.2423, 35.2349, 35.2128, 35.2131,
    35.2169, 35.2074, 35.2117, 35.2105, 35.2139,
    35.2208, 35.2126, 35.2204, 35.2133, 35.2146,
    35.2041, 35.2039, 35.2034, 35.2239, 35.2144,
    35.2223, 35.2174, 35.2300, 35.2332, 35.2122,
    35.2305, 35.2249, 35.2251, 35.2168, 35.2214
        };

        // arry of latitudes
        double[] latitudes = new double[]
        {
    31.7769, 31.7747, 31.7830, 31.7739, 31.7762,
    31.7743, 31.7748, 31.7796, 31.7740, 31.7711,
    31.7741, 31.7764, 31.7810, 31.7768, 31.7819,
    31.7795, 31.7780, 31.7825, 31.7814, 31.7817,
    31.7822, 31.7789, 31.7758, 31.7726, 31.7703,
    31.7831, 31.7813, 31.7792, 31.7764, 31.7805,
    31.7753, 31.7755, 31.7793, 31.7761, 31.7708,
    31.7716, 31.7777, 31.7722, 31.7803, 31.7801,
    31.7796, 31.7748, 31.7794, 31.7759, 31.7816,
    31.7809, 31.7730, 31.7792, 31.7749, 31.7731
        };
        // create call
        for (int i = 0; i < 50; i++)
        {
            // Declare the variable once
            CallType calltype;
            string ndescription;
          

            // Assign a call type and description based on the value of i
            if (i % 3 == 0)
            {
                calltype = CallType.FoodPreparation;
                if (p >= DescriptionsP.Length)
                    p = 0;
                ndescription = DescriptionsP[p];
                p++;
            }
            else if (i % 4 == 0)
            {
                calltype = CallType.FoodTransport;
                if (T >= DescriptionsL.Length)
                    T = 0;
                ndescription = DescriptionsL[T];
                T++;
            }
            else
            {
                calltype = CallType.InventoryCheck;
                if (I >= DescriptionsC.Length)
                    I = 0;
                ndescription = DescriptionsC[I];
                I++;
            }

            // Set the start time to one day before the current clock time
            DateTime start = s_dal!.Config.Clock.AddDays(-1);

            // Calculate the total minutes from the start time to the current time
            int totalMinutesInLastDay = (int)(s_dal.Config.Clock - start).TotalMinutes;

            // Generate a random start time within the last 24 hours
            DateTime RndomStart = start.AddMinutes(s_rand.Next(0, totalMinutesInLastDay));

            // Optional end time
            DateTime? RandomEnd = null;

            // Randomly decide end time logic
            if (i % 10 == 0)
            {
                // Ensure this call might be expired
                int maxRange = (int)(s_dal.Config.Clock - RndomStart).TotalMinutes;
                if (maxRange > 0)
                {
                    RandomEnd = RndomStart.AddMinutes(s_rand.Next(1, maxRange + 1));
                }
            }
            else
            {
                // Randomly decide to assign an end time
                if (s_rand.Next(2) == 1)
                {
                    int maxDurationMinutes = s_rand.Next(1, 1441);
                    RandomEnd = RndomStart.AddMinutes(maxDurationMinutes);

                    // Make approximately 5 calls expire
                    if (i < 5)
                    {
                        RandomEnd = s_dal.Config.Clock.AddMinutes(-s_rand.Next(1, 60)); // Expired time
                    }
                }
            }

            // Create the call object
            s_dal.Call.Create(new Call(0, calltype, ndescription, addresses[i], latitudes[i], longitudes[i], RndomStart, RandomEnd));
        }
    }

    private static void CreateAssignment()
    {
        for (int i = 0; i < 60; i++)
        {
            //Assigning a volunteer to a task
            int randVolunteer = s_rand.Next(s_dal!.Volunteer.ReadAll().Count());
            Volunteer volunteerToAssign = s_dal.Volunteer.ReadAll().OrderBy(v => s_rand.Next()).First();
            //call number ID
            int randCAll = s_rand.Next(s_dal.Call!.ReadAll().Count() - 15);
            Call callToAssig = s_dal.Call.ReadAll().OrderBy(v => s_rand.Next()).First();
            while (callToAssig.TimeOpened > s_dal!.Config!.Clock)
            {
                randCAll = s_rand.Next(s_dal.Call!.ReadAll().Count() - 15);
                callToAssig = s_dal.Call.ReadAll().OrderBy(v => s_rand.Next()).First();
            }
            TypeEnd? finish = null;
            DateTime? finishTime = null;
            if (callToAssig.MaxTimeToClose != null && callToAssig.MaxTimeToClose >= s_dal!.Config?.Clock)
            {
                finish = TypeEnd.ExpiredCancel;
                finishTime = s_dal.Config.Clock;
            }
            else
            {
                int randFinish = s_rand.Next(0, 4);
                switch (randFinish)
                {
                    case 0:
                        finish = TypeEnd.Treated;
                        finishTime = s_dal!.Config!.Clock;
                        break;
                    case 1:
                        finish = TypeEnd.SelfCancel;
                        finishTime = s_dal.Config.Clock;
                        break;
                    case 2:
                        finish = TypeEnd.ManagerCancel;
                        finishTime = s_dal.Config.Clock; break;
                }
            }
            s_dal.Assignment?.Create(new Assignment(0, callToAssig.Id, volunteerToAssign.Id, s_dal!.Config!.Clock, finishTime, finish));
        }
    }


    // public static void Do(IVolunteer? dalVolunteer, ICall? dalCall, IAssignment? dalAssignment, IConfig? dalConfig)
    //public static void Do(IDal dal) //stage 2
    public static void Do() //stage 4
    {
        // Null checks for the parameters to ensure that none of them are null
        //s_Volunteer = dalVolunteer ?? throw new NullReferenceException("Volunteer DAL object cannot be null!");
        //s_Call = dalCall ?? throw new NullReferenceException("Call DAL object cannot be null!");
        //s_Assignment = dalAssignment ?? throw new NullReferenceException("Assignment DAL object cannot be null!");
        //s_dal.Config = dalConfig ?? throw new NullReferenceException("Config DAL object cannot be null!");

        // s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2
        s_dal = DalApi.Factory.Get; //stage 4

        Console.WriteLine("Resetting configuration values and clearing all lists...");

        s_dal.ResetDB();//stage 2


        Console.WriteLine("Initializing volunteers...");
        // Calling method to initialize volunteers
        CreateVolunteer();

        Console.WriteLine("Initializing calls...");
        // Calling method to initialize calls
        CreateCalls();

        Console.WriteLine("Initializing assignments...");
        // Calling method to initialize assignments
        CreateAssignment();

        Console.WriteLine("Initialization complete.");
    }
}                                                      
