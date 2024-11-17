using Dal;
using DalApi;
using DO;


namespace DalTest
{
    public class Program
    {
        private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        private static ICall? s_dalCall = new CallImplementation();
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
        private static IConfig? s_dalConfig = new ConfigImplementation();

        // Enum representing the options available in the Main Menu
        public enum MainMenuOption
        {
            Exit,                  // Exit the program
            VolunteerMenu,         // Go to the Volunteer Menu
            CallMenu,              // Go to the Call Menu
            AssignmentMenu,        // Go to the Assignment Menu
            ConfigMenu,            // Go to the Config Menu
            InitializeData,        // Initialize data for the application
            ShowAllData,           // Show all data stored
            ResetDatabase          // Reset the database
        }

        // Enum representing the options available in the Volunteer Menu
        public enum VolunteerMenuOption
        {
            Exit,                  // Exit the Volunteer Menu
            Create,                // Create a new Volunteer
            Read,                  // Read a specific Volunteer
            ReadAll,               // Read all Volunteers
            Update,                // Update a Volunteer
            Delete,                // Delete a specific Volunteer
            DeleteAll             // Delete all Volunteers
        }

        // Enum representing the options available in the Call Menu
        public enum CallMenuOption
        {
            Exit,                  // Exit the Call Menu
            Create,                // Create a new Call
            Read,                  // Read a specific Call
            ReadAll,               // Read all Calls
            Update,                // Update a Call
            Delete,                // Delete a specific Call
            DeleteAll             // Delete all Calls
        }

        // Enum representing the options available in the Assignment Menu
        public enum AssignmentMenuOption
        {
            Exit,                  // Exit the Assignment Menu
            Create,                // Create a new Assignment
            Read,                  // Read a specific Assignment
            ReadAll,               // Read all Assignments
            Update,                // Update an Assignment
            Delete,                // Delete a specific Assignment
            DeleteAll             // Delete all Assignments
        }

        // Enum representing the options available in the Config Menu
        public enum ConfigMenuOption
        {
            Exit,                  // Exit the Config Menu
            AdvanceSystemClock,    // Advance the system clock
            ShowSystemClock,       // Show the current system clock
            SetConfigVariable,     // Set a specific configuration variable
            ShowConfigVariable,    // Show a specific configuration variable
            ResetConfig            // Reset the configuration settings
        }

        // Main entry point of the application
        static void Main(string[] args)
        {
            try
            {
                // Initialize data access layers for different entities
                s_dalVolunteer = new VolunteerImplementation();  // Create Volunteer implementation
                s_dalCall = new CallImplementation();            // Create Call implementation
                s_dalAssignment = new AssignmentImplementation(); // Create Assignment implementation
                s_dalConfig = new ConfigImplementation();        // Create Config implementation

                bool exit = false;  // Flag to control the exit condition of the main loop
                while (!exit)
                {
                    // Display Main Menu
                    Console.WriteLine("\nMain Menu:");
                    Console.WriteLine("1. Volunteer Menu");
                    Console.WriteLine("2. Call Menu");
                    Console.WriteLine("3. Assignment Menu");
                    Console.WriteLine("4. Config Menu");
                    Console.WriteLine("5. Initialize Data");
                    Console.WriteLine("6. Show All Data");
                    Console.WriteLine("7. Reset Database");
                    Console.WriteLine("0. Exit");

                    // Read the user's choice from the console and cast it to the MainMenuOption enum
                    MainMenuOption choice = (MainMenuOption)int.Parse(Console.ReadLine() ?? "0");

                    // Process the selected menu option
                    switch (choice)
                    {
                        case MainMenuOption.VolunteerMenu:
                            VolunteerMenu();  // Open the Volunteer Menu
                            break;
                        case MainMenuOption.CallMenu:
                            CallMenu();  // Open the Call Menu
                            break;
                        case MainMenuOption.AssignmentMenu:
                            AssignmentMenu();  // Open the Assignment Menu
                            break;
                        case MainMenuOption.ConfigMenu:
                            ConfigMenu();  // Open the Config Menu
                            break;
                        case MainMenuOption.InitializeData:
                            // Call the initialization function to populate the system with initial data
                            Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
                            break;
                        case MainMenuOption.ShowAllData:
                            // Display all the data in the system
                            print(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
                            break;
                        case MainMenuOption.ResetDatabase:
                            // Reset the database by reading all entities and calling the reset function
                            s_dalVolunteer.ReadAll();
                            s_dalCall.ReadAll();
                            s_dalAssignment.ReadAll();
                            s_dalConfig.Reset();  // Reset the configuration
                            break;
                        case MainMenuOption.Exit:
                            exit = true;  // Set exit flag to true to break the loop
                            break;
                        default:
                            // Handle invalid menu choices
                            Console.WriteLine("Invalid choice, try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch and display any exceptions that occur during initialization
                Console.WriteLine($"An error occurred during initialization: {ex.Message}");
            }
        }
        private static void VolunteerMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nVolunteer Menu:");
                Console.WriteLine("1. Add Volunteer");
                Console.WriteLine("2. Read Volunteer by ID");
                Console.WriteLine("3. Read All Volunteers");
                Console.WriteLine("4. Update Volunteer");
                Console.WriteLine("5. Delete Volunteer");
                Console.WriteLine("6. Delete All Volunteers");
                Console.WriteLine("0. Exit");

                VolunteerMenuOption choice = (VolunteerMenuOption)int.Parse(Console.ReadLine() ?? "0");

                switch (choice)
                {
                    case VolunteerMenuOption.Create:
                        // Method to add a volunteer
                        // Input data from the user

                        Volunteer volunteer = helpVolunteer(); // Helper method to get volunteer data
                        s_dalVolunteer!.Create(volunteer); // Create the volunteer

                        break;
                    case VolunteerMenuOption.Read:
                        Console.WriteLine("Enter ID you want to read:");
                        int tempid = int.Parse(Console.ReadLine() ?? "0");
                        s_dalVolunteer!.Read(tempid); // Method to display volunteer by ID
                        break;
                    case VolunteerMenuOption.ReadAll:
                        foreach (var item in s_dalVolunteer!.ReadAll())
                            Console.WriteLine(item);
                        break;

                    case VolunteerMenuOption.Update:
                        Volunteer volunteer1 = helpVolunteer(); // Get updated volunteer data
                        s_dalVolunteer!.Update(volunteer1); // Update the volunteer information
                        break;
                    case VolunteerMenuOption.Delete:
                        Console.WriteLine("Enter ID you want to delete:");
                        int tempid2 = int.Parse(Console.ReadLine() ?? "0");
                        s_dalVolunteer!.Delete(tempid2); // Method to delete volunteer by ID
                        break;
                    case VolunteerMenuOption.DeleteAll:
                        s_dalVolunteer!.DeleteAll(); // Method to delete all volunteers
                        break;
                    case VolunteerMenuOption.Exit:
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }

        public static Volunteer helpVolunteer()
        {
            Console.WriteLine("Enter Volunteer ID:");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("Enter Volunteer Full Name:");
            string fullName = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Volunteer Phone Number:");
            string phoneNumber = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Volunteer Email:");
            string email = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Distance Type (0 = Aerial, 1 = Driving):");
            Distance typeDistance = (Distance)int.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("Enter Role (0 = Volunteer, 1 = Manager):");
            Role job = (Role)int.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("Is Active? (true/false):");
            bool active = bool.Parse(Console.ReadLine() ?? "true");

            Console.WriteLine("Enter Password (or press Enter to skip):");
            string? password = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(password))
                password = null;

            Console.WriteLine("Enter Full Address (or press Enter to skip):");
            string? fullAddress = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fullAddress))
                fullAddress = null;

            Console.WriteLine("Enter Latitude (or press Enter to skip):");
            string latitudeInput = Console.ReadLine() ?? "";
            double? latitude = string.IsNullOrWhiteSpace(latitudeInput) ? null : double.Parse(latitudeInput);

            Console.WriteLine("Enter Longitude (or press Enter to skip):");
            string longitudeInput = Console.ReadLine() ?? "";
            double? longitude = string.IsNullOrWhiteSpace(longitudeInput) ? null : double.Parse(longitudeInput);

            Console.WriteLine("Enter Max Reading (or press Enter to skip):");
            string maxReadingInput = Console.ReadLine() ?? "";
            double? maxReading = string.IsNullOrWhiteSpace(maxReadingInput) ? null : double.Parse(maxReadingInput);

            // יצירת אובייקט Volunteer לפי הסדר שהוגדר ב-record
            return new Volunteer(
                Id: id,
                FullName: fullName,
                PhoneNumber: phoneNumber,
                Email: email,
                TypeDistance: typeDistance,
                Job: job,
                Active: active,
                Password: password,
                FullAddress: fullAddress,
                Latitude: latitude,
                Longitude: longitude,
                MaxReading: maxReading
            );
        }

        public static void print(IVolunteer? s_dalVolunteer, ICall? s_dalCall, IAssignment? s_dalAssignment, IConfig? s_dalConfig)
        {

            Console.WriteLine("Volunteer Data:");
            if (s_dalVolunteer != null)
            {
                foreach (var volunteer in s_dalVolunteer.ReadAll())
                {
                    Console.WriteLine(volunteer); // Assuming ToString() is overridden for Volunteer
                }
            }
            else
            {
                Console.WriteLine("No Volunteer data available.");
            }

            Console.WriteLine("\nCall Data:");
            if (s_dalCall != null)
            {
                foreach (var call in s_dalCall.ReadAll())
                {
                    Console.WriteLine(call); // Assuming ToString() is overridden for Call
                }
            }
            else
            {
                Console.WriteLine("No Call data available.");
            }

            Console.WriteLine("\nAssignment Data:");
            if (s_dalAssignment != null)
            {
                foreach (var assignment in s_dalAssignment.ReadAll())
                {
                    Console.WriteLine(assignment); // Assuming ToString() is overridden for Assignment
                }
            }
            else
            {
                Console.WriteLine("No Assignment data available.");
            }

            Console.WriteLine("\nConfig Data:");
            if (s_dalConfig != null)
            {
                Console.WriteLine($"System Clock: {s_dalConfig.Clock}");
                // הצגת ערכי תצורה נוספים, אם ישנם
            }
            else
            {
                Console.WriteLine("No Config data available.");
            }
        }
        private static void CallMenu() // Choosing where to enter
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nCall Menu:");
                Console.WriteLine("1. Create Calls (Auto-generate 50 calls)");
                Console.WriteLine("2. Read Call by ID");
                Console.WriteLine("3. Read All Calls");
                Console.WriteLine("4. Update Call");
                Console.WriteLine("5. Delete Call");
                Console.WriteLine("6. Delete All Calls");
                Console.WriteLine("0. Exit");

                CallMenuOption choice = (CallMenuOption)int.Parse(Console.ReadLine() ?? "0");

                switch (choice)
                {
                    case CallMenuOption.Create:
                        Call call1 = helpCall();
                        s_dalCall!.Create(call1);
                        Console.WriteLine("50 calls have been created.");
                        break;

                    case CallMenuOption.Read:
                        Console.Write("Enter Call ID to display: ");
                        int id = int.Parse(Console.ReadLine() ?? "0");
                        var call = s_dalCall?.Read(id);

                        break;

                    case CallMenuOption.ReadAll:
                        {foreach (var item in s_dalCall!.ReadAll())
                            Console.WriteLine(item);
                            break;
                        }

                    case CallMenuOption.Update:
                        {
                            Call call2 = helpCall();
                            s_dalCall!.Update(call2);

                        }
                        break;

                    case CallMenuOption.Delete:
                        Console.Write("Enter Call ID to delete: ");
                        int deleteId = int.Parse(Console.ReadLine() ?? "0");
                        s_dalCall?.Delete(deleteId);
                        break;

                    case CallMenuOption.DeleteAll:
                        s_dalCall?.DeleteAll();
                        break;

                    case CallMenuOption.Exit:
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }

        }
        public static Call helpCall() // for things that repeat themselves
        {
            Console.Write("Enter Call ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter Call Type (0 - Type1, 1 - Type2, etc.): ");
            int callTypeInput = int.Parse(Console.ReadLine() ?? "0");
            CallType callType = (CallType)callTypeInput;

            Console.Write("Enter Verbal Description: ");
            string description = Console.ReadLine() ?? "";

            Console.Write("Enter Full Address: ");
            string fullAddress = Console.ReadLine() ?? "";

            Console.Write("Enter Latitude: ");
            double latitude = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter Longitude: ");
            double longitude = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter Opening Time (yyyy-MM-dd HH:mm): ");
            DateTime timeOpened = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString());

            Console.Write("Enter Max End Time (yyyy-MM-dd HH:mm) or leave empty if not applicable: ");
            string? maxTimeToCloseInput = Console.ReadLine();
            DateTime? maxTimeToClose = string.IsNullOrWhiteSpace(maxTimeToCloseInput) ? null : DateTime.Parse(maxTimeToCloseInput);

            // Create and return the Call object
            return new Call(
                Id: id,
                Type: callType,
                Description: description,
                FullAddress: fullAddress,
                Latitude: latitude,
                Longitude: longitude,
                TimeOpened: timeOpened,
                MaxTimeToClose: maxTimeToClose
            );
        }
        private static void AssignmentMenu()
        {

            bool exit = false;

            // Loop to keep displaying the menu until the user chooses to exit
            while (!exit)
            {
                Console.WriteLine("\nChoose the method you'd like to perform:");
                Console.WriteLine("0. Exit submenu");
                Console.WriteLine("1. Add a new object of this type to the list (Create)");
                Console.WriteLine("2. Read an object by ID (Read)");
                Console.WriteLine("3. Read the list of all objects of this type (ReadAll)");
                Console.WriteLine("4. Update an existing object's data (Update)");
                Console.WriteLine("5. Delete an existing object from the list (Delete)");
                Console.WriteLine("6. Delete all objects in the list (DeleteAll)");

                Console.Write("Choose an option: ");
                string? choice = Console.ReadLine();

                try
                {
                    // Switch statement to handle each menu option
                    switch (choice)
                    {
                        case "0":
                            // Exit the submenu
                            exit = true;
                            break;

                        case "1":
                            Console.WriteLine("Adding a new object");
                            // Code to create a new object and add it to the list
                            Assignment Assi = helpAssignment();
                            s_dalAssignment!.Create(Assi);

                            break;

                        case "2":
                            Console.WriteLine("Enter the ID of the object to Read:");
                            int idToRead = int.Parse(Console.ReadLine() ?? "0");
                            s_dalAssignment!.Read(idToRead);
                            // Code to read and display the object by ID

                            break;

                        case "3":
                            
                          {  foreach (var item in s_dalAssignment!.ReadAll())
                                Console.WriteLine(item);
                                break;
                            }

                        case "4":
                            Console.WriteLine("Enter the ID of the object to update:");

                            Assignment ToUpdate = helpAssignment();
                            s_dalAssignment!.Create(ToUpdate);

                            // Code to update the object by ID
                            break;

                        case "5":
                            Console.WriteLine("Enter the ID of the object to delete:");
                            int idToDelete = int.Parse(Console.ReadLine() ?? "0");
                            s_dalAssignment!.Delete(idToDelete);
                            // Code to delete the object by ID
                            break;

                        case "6":
                            Console.WriteLine("Deleting all objects in the list");
                            // Code to delete all objects of this type
                            s_dalAssignment!.DeleteAll();
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please select a valid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions and display an error message
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            static Assignment helpAssignment()
            {
                Console.Write("Enter Assignment ID (integer): ");
                int id = int.TryParse(Console.ReadLine(), out int parsedId) ? parsedId : 0;

                Console.Write("Enter Call ID (integer): ");
                int callId = int.TryParse(Console.ReadLine(), out int parsedCallId) ? parsedCallId : 0;

                Console.Write("Enter Volunteer ID (integer): ");
                int volunteerId = int.TryParse(Console.ReadLine(), out int parsedVolunteerId) ? parsedVolunteerId : 0;

                Console.Write("Enter the start time of treatment (yyyy-MM-dd HH:mm:ss): ");
                DateTime timeStart;
                if (!DateTime.TryParse(Console.ReadLine(), out timeStart))
                {
                    Console.WriteLine("Invalid date input. Setting StartTime to default (DateTime.MinValue).");
                    timeStart = DateTime.MinValue;
                }

                Console.Write("Enter the actual end time of handling (yyyy-MM-dd HH:mm:ss) or leave empty if not applicable: ");
                DateTime? timeEnd = null;
                string? timeEndInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(timeEndInput) && DateTime.TryParse(timeEndInput, out DateTime parsedEndTime))
                {
                    timeEnd = parsedEndTime;
                }

                Console.Write("Enter the type of treatment conclusion (0 = Completed, 1 = Self-Cancelled, 2 = Admin-Cancelled, 3 = Expired) or leave empty for default: ");
                TypeEnd? typeEndTreat = null;
                string? typeEndTreatInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(typeEndTreatInput) && Enum.TryParse(typeEndTreatInput, out TypeEnd parsedTypeEnd))
                {
                    typeEndTreat = parsedTypeEnd;
                }

                // Return a new Assignment object
                return new Assignment(
                    Id: id,
                    CallId: callId,
                    VolunteerId: volunteerId,
                    TimeStart: timeStart,
                    TimeEnd: timeEnd,
                    TypeEndTreat: typeEndTreat
                );
            }
      }
            private static void ConfigMenu()
            {
                bool exit = false;
                while (!exit)
                {
                    Console.WriteLine("\nConfig Menu:");
                    Console.WriteLine("1. Advance System Clock");
                    Console.WriteLine("2. Show System Clock");
                    Console.WriteLine("3. Set Config Variable");
                    Console.WriteLine("4. Show Config Variable");
                    Console.WriteLine("5. Reset Config");
                    Console.WriteLine("0. Exit");

                    ConfigMenuOption choice = (ConfigMenuOption)int.Parse(Console.ReadLine() ?? "0");

                    switch (choice)
                    {
                        case ConfigMenuOption.AdvanceSystemClock:
                            Console.Write("Enter the number of hours to advance the clock: ");
                            int hours = int.Parse(Console.ReadLine() ?? "0");
                            s_dalConfig!.Clock = s_dalConfig.Clock.AddHours(hours);
                            Console.WriteLine($"System clock advanced by {hours} hours."); break;

                        case ConfigMenuOption.ShowSystemClock:
                            Console.WriteLine($"Current System Clock: {s_dalConfig?.Clock}");
                            break;

                        case ConfigMenuOption.SetConfigVariable:
                            Console.WriteLine("Choose the variable to set:");
                            Console.WriteLine("1. Risk Range (in hours)");

                            int option = int.Parse(Console.ReadLine() ?? "0");

                            switch (option)
                            {
                                case 1:
                                    Console.Write("Enter the new risk range in hours: ");
                                    double hours1 = double.Parse(Console.ReadLine() ?? "1");
                                    s_dalConfig!.RiskRange = TimeSpan.FromHours(hours1);
                                    Console.WriteLine($"Risk range set to {hours1} hours.");
                                    break;
                                default:
                                    Console.WriteLine("Invalid option.");
                                    break;
                            }
                            break;

                        case ConfigMenuOption.ShowConfigVariable:
                            Console.WriteLine($"Current System Clock: {s_dalConfig!.Clock}");
                            Console.WriteLine($"Risk Range: {s_dalConfig.RiskRange.TotalHours} hours"); break;

                        case ConfigMenuOption.ResetConfig:
                            s_dalConfig?.Reset();
                            Console.WriteLine("Config reset to default.");
                            break;

                        case ConfigMenuOption.Exit:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice, try again.");
                            break;
                    }
                }
            }
        }

    }