using Dal;
using DalApi;
using DO;


namespace DalTest
{
    public class Program
    {
        // Creating instances of data access layer (DAL) for handling different entities

        // s_dalAssignment is used to interact with the data layer for Assignment objects
        private static IAssignment? s_dalAssignment = new AssignmentImplementation();

        // s_dalCall is used to interact with the data layer for Call objects
        private static ICall? s_dalCall = new CallImplementation();

        // s_dalVolunteer is used to interact with the data layer for Volunteer objects
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();

        // s_dalConfig is used to interact with the data layer for Config settings
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

            // Loop to keep displaying the menu until the user chooses to exit
            while (!exit)
            {
                Console.WriteLine("\nVolunteer Menu:");
                Console.WriteLine("1. Add Volunteer"); // Option to add a new volunteer
                Console.WriteLine("2. Read Volunteer by ID"); // Option to read a volunteer by their ID
                Console.WriteLine("3. Read All Volunteers"); // Option to read all volunteers
                Console.WriteLine("4. Update Volunteer"); // Option to update volunteer details
                Console.WriteLine("5. Delete Volunteer"); // Option to delete a volunteer by ID
                Console.WriteLine("6. Delete All Volunteers"); // Option to delete all volunteers
                Console.WriteLine("0. Exit"); // Option to exit the menu

                // Get the user input for menu selection and convert to enum
                VolunteerMenuOption choice = (VolunteerMenuOption)int.Parse(Console.ReadLine() ?? "0");

                // Switch statement to handle the different menu options
                switch (choice)
                {
                    case VolunteerMenuOption.Create:
                        // Method to add a volunteer
                        // Input data from the user
                        Volunteer volunteer = helpVolunteer(); // Helper method to get volunteer data
                        s_dalVolunteer!.Create(volunteer); // Create the volunteer in the data layer
                        break;

                    case VolunteerMenuOption.Read:
                        // Option to read a volunteer by ID
                        Console.WriteLine("Enter ID you want to read:");
                        int tempid = int.Parse(Console.ReadLine() ?? "0");
                        s_dalVolunteer!.Read(tempid); // Display volunteer data by ID
                        break;

                    case VolunteerMenuOption.ReadAll:
                        // Option to read all volunteers
                        foreach (var item in s_dalVolunteer!.ReadAll())
                            Console.WriteLine(item); // Display all volunteers
                        break;

                    case VolunteerMenuOption.Update:
                        // Option to update volunteer details
                        Volunteer volunteer1 = helpVolunteer(); // Get updated volunteer data
                        s_dalVolunteer!.Update(volunteer1); // Update the volunteer information
                        break;

                    case VolunteerMenuOption.Delete:
                        // Option to delete a volunteer by ID
                        Console.WriteLine("Enter ID you want to delete:");
                        int tempid2 = int.Parse(Console.ReadLine() ?? "0");
                        s_dalVolunteer!.Delete(tempid2); // Delete the volunteer by ID
                        break;

                    case VolunteerMenuOption.DeleteAll:
                        // Option to delete all volunteers
                        s_dalVolunteer!.DeleteAll(); // Delete all volunteers
                        break;

                    case VolunteerMenuOption.Exit:
                        // Exit the menu
                        exit = true;
                        break;

                    default:
                        // Handle invalid menu choice
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }

        //collects volunteer details
        //from the user and returns
        //a Volunteer object with the entered information.
        public static Volunteer helpVolunteer()
        {
            // Prompt user to enter the Volunteer ID
            Console.WriteLine("Enter Volunteer ID:");
            int id = int.Parse(Console.ReadLine() ?? "0");

            // Prompt user to enter the full name of the volunteer
            Console.WriteLine("Enter Volunteer Full Name:");
            string fullName = Console.ReadLine() ?? "";

            // Prompt user to enter the phone number of the volunteer
            Console.WriteLine("Enter Volunteer Phone Number:");
            string phoneNumber = Console.ReadLine() ?? "";

            // Prompt user to enter the email address of the volunteer
            Console.WriteLine("Enter Volunteer Email:");
            string email = Console.ReadLine() ?? "";

            // Prompt user to select the distance type (Aerial or Driving)
            Console.WriteLine("Enter Distance Type (0 = Aerial, 1 = Driving):");
            Distance typeDistance = (Distance)int.Parse(Console.ReadLine() ?? "0");

            // Prompt user to select the role (Volunteer or Manager)
            Console.WriteLine("Enter Role (0 = Volunteer, 1 = Manager):");
            Role job = (Role)int.Parse(Console.ReadLine() ?? "0");

            // Prompt user to input whether the volunteer is active or not
            Console.WriteLine("Is Active? (true/false):");
            bool active = bool.Parse(Console.ReadLine() ?? "true");

            // Prompt user to enter a password (optional)
            Console.WriteLine("Enter Password (or press Enter to skip):");
            string? password = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(password))
                password = null;

            // Prompt user to enter the full address (optional)
            Console.WriteLine("Enter Full Address (or press Enter to skip):");
            string? fullAddress = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fullAddress))
                fullAddress = null;

            // Prompt user to enter latitude (optional)
            Console.WriteLine("Enter Latitude (or press Enter to skip):");
            string latitudeInput = Console.ReadLine() ?? "";
            double? latitude = string.IsNullOrWhiteSpace(latitudeInput) ? null : double.Parse(latitudeInput);

            // Prompt user to enter longitude (optional)
            Console.WriteLine("Enter Longitude (or press Enter to skip):");
            string longitudeInput = Console.ReadLine() ?? "";
            double? longitude = string.IsNullOrWhiteSpace(longitudeInput) ? null : double.Parse(longitudeInput);

            // Prompt user to enter max reading value (optional)
            Console.WriteLine("Enter Max Reading (or press Enter to skip):");
            string maxReadingInput = Console.ReadLine() ?? "";
            double? maxReading = string.IsNullOrWhiteSpace(maxReadingInput) ? null : double.Parse(maxReadingInput);

            // Return a new Volunteer object with the provided input data
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
            // Print Volunteer Data
            Console.WriteLine("Volunteer Data:");
            if (s_dalVolunteer != null)
            {
                // Iterate through the list of volunteers and print each one (assuming ToString() is overridden)
                foreach (var volunteer in s_dalVolunteer.ReadAll())
                {
                    Console.WriteLine(volunteer);
                }
            }
            else
            {
                Console.WriteLine("No Volunteer data available.");
            }

            // Print Call Data
            Console.WriteLine("\nCall Data:");
            if (s_dalCall != null)
            {
                // Iterate through the list of calls and print each one (assuming ToString() is overridden)
                foreach (var call in s_dalCall.ReadAll())
                {
                    Console.WriteLine(call);
                }
            }
            else
            {
                Console.WriteLine("No Call data available.");
            }

            // Print Assignment Data
            Console.WriteLine("\nAssignment Data:");
            if (s_dalAssignment != null)
            {
                // Iterate through the list of assignments and print each one (assuming ToString() is overridden)
                foreach (var assignment in s_dalAssignment.ReadAll())
                {
                    Console.WriteLine(assignment);
                }
            }
            else
            {
                Console.WriteLine("No Assignment data available.");
            }

            // Print Config Data
            Console.WriteLine("\nConfig Data:");
            if (s_dalConfig != null)
            {
                // Print system clock and any additional configuration data
                Console.WriteLine($"System Clock: {s_dalConfig.Clock}");
            }
            else
            {
                Console.WriteLine("No Config data available.");
            }
        }

        // Call menu method that handles the different options for managing calls
        private static void CallMenu() // Choosing where to enter
        {
            bool exit = false;

            // Loop to keep displaying the menu until the user chooses to exit
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

                // Parse the user's choice from the menu options
                CallMenuOption choice = (CallMenuOption)int.Parse(Console.ReadLine() ?? "0");

                // Switch statement to handle each menu option
                switch (choice)
                {
                    // Case for creating and auto-generating 50 calls
                    case CallMenuOption.Create:
                        Call call1 = helpCall(); // Calls the method to gather information for a new call
                        s_dalCall!.Create(call1); // Creates the new call and adds it to the list
                        Console.WriteLine("50 calls have been created.");
                        break;

                    // Case for reading a single call by its ID
                    case CallMenuOption.Read:
                        Console.Write("Enter Call ID to display: ");
                        int id = int.Parse(Console.ReadLine() ?? "0"); // Parse the call ID
                        var call = s_dalCall?.Read(id); // Retrieves the call by ID from the data layer
                        break;

                    // Case for reading and displaying all calls
                    case CallMenuOption.ReadAll:
                        {
                            foreach (var item in s_dalCall!.ReadAll()) // Loops through all the calls and displays them
                                Console.WriteLine(item);
                            break;
                        }

                    // Case for updating an existing call
                    case CallMenuOption.Update:
                        {
                            Call call2 = helpCall(); // Gather data for the call to be updated
                            s_dalCall!.Update(call2); // Updates the call in the data layer
                        }
                        break;

                    // Case for deleting a specific call by ID
                    case CallMenuOption.Delete:
                        Console.Write("Enter Call ID to delete: ");
                        int deleteId = int.Parse(Console.ReadLine() ?? "0"); // Parse the call ID to delete
                        s_dalCall?.Delete(deleteId); // Deletes the call from the data layer
                        break;

                    // Case for deleting all calls
                    case CallMenuOption.DeleteAll:
                        s_dalCall?.DeleteAll(); // Deletes all calls from the data layer
                        break;

                    // Case for exiting the menu
                    case CallMenuOption.Exit:
                        exit = true; // Exits the loop
                        break;

                    // Default case to handle invalid menu choices
                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }

        public static Call helpCall() // for things that repeat themselves
        {
            Console.Write("Enter Call ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0"); // Get the Call ID from the user

            Console.Write("Enter Call Type (0 - Type1, 1 - Type2, etc.): ");
            int callTypeInput = int.Parse(Console.ReadLine() ?? "0"); // Get the Call Type as an integer
            CallType callType = (CallType)callTypeInput; // Convert the input integer to CallType enum

            Console.Write("Enter Verbal Description: ");
            string description = Console.ReadLine() ?? ""; // Get the verbal description of the call

            Console.Write("Enter Full Address: ");
            string fullAddress = Console.ReadLine() ?? ""; // Get the full address of the call

            Console.Write("Enter Latitude: ");
            double latitude = double.Parse(Console.ReadLine() ?? "0"); // Get the latitude of the call location

            Console.Write("Enter Longitude: ");
            double longitude = double.Parse(Console.ReadLine() ?? "0"); // Get the longitude of the call location

            Console.Write("Enter Opening Time (yyyy-MM-dd HH:mm): ");
            DateTime timeOpened = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString()); // Get the time when the call was opened

            Console.Write("Enter Max End Time (yyyy-MM-dd HH:mm) or leave empty if not applicable: ");
            string? maxTimeToCloseInput = Console.ReadLine(); // Optionally get the maximum end time for the call
            DateTime? maxTimeToClose = string.IsNullOrWhiteSpace(maxTimeToCloseInput) ? null : DateTime.Parse(maxTimeToCloseInput); // If provided, parse the max end time

            // Create and return the Call object
            return new Call(
                Id: id, // Set the Call ID
                Type: callType, // Set the Call Type
                Description: description, // Set the verbal description
                FullAddress: fullAddress, // Set the full address
                Latitude: latitude, // Set the latitude
                Longitude: longitude, // Set the longitude
                TimeOpened: timeOpened, // Set the time the call was opened
                MaxTimeToClose: maxTimeToClose // Set the maximum end time if provided, otherwise null
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
                string? choice = Console.ReadLine(); // Get the user's choice from the menu

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
                            // Adding a new object
                            Console.WriteLine("Adding a new object");
                            Assignment Assi = helpAssignment(); // Get the details for the new Assignment
                            s_dalAssignment!.Create(Assi); // Add the new Assignment to the list using the DAL

                            break;

                        case "2":
                            // Read an object by ID
                            Console.WriteLine("Enter the ID of the object to Read:");
                            int idToRead = int.Parse(Console.ReadLine() ?? "0"); // Get the ID to read
                            s_dalAssignment!.Read(idToRead); // Read the object by ID using the DAL

                            break;

                        case "3":
                            // Read all objects of this type
                            foreach (var item in s_dalAssignment!.ReadAll()) // Loop through all items in the list
                                Console.WriteLine(item); // Display each item in the list
                            break;

                        case "4":
                            // Update an existing object's data
                            Console.WriteLine("Enter the ID of the object to update:");
                            Assignment ToUpdate = helpAssignment(); // Get the updated details for the Assignment
                            s_dalAssignment!.Create(ToUpdate); // Update the object using the DAL

                            break;

                        case "5":
                            // Delete an existing object from the list
                            Console.WriteLine("Enter the ID of the object to delete:");
                            int idToDelete = int.Parse(Console.ReadLine() ?? "0"); // Get the ID to delete
                            s_dalAssignment!.Delete(idToDelete); // Delete the object by ID using the DAL
                            break;
                        case "6":
                            // Delete all objects from the list
                            Console.WriteLine("Deleting all objects in the list");
                            s_dalAssignment!.DeleteAll(); // Delete all objects using the DAL
                            break;

                        default:
                            // Handle invalid choice
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
            // Method to help gather the necessary information for creating an Assignment object
            static Assignment helpAssignment()
            {
                // Prompt for and parse Assignment ID
                Console.Write("Enter Assignment ID (integer): ");
                int id = int.TryParse(Console.ReadLine(), out int parsedId) ? parsedId : 0;

                // Prompt for and parse Call ID
                Console.Write("Enter Call ID (integer): ");
                int callId = int.TryParse(Console.ReadLine(), out int parsedCallId) ? parsedCallId : 0;

                // Prompt for and parse Volunteer ID
                Console.Write("Enter Volunteer ID (integer): ");
                int volunteerId = int.TryParse(Console.ReadLine(), out int parsedVolunteerId) ? parsedVolunteerId : 0;

                // Prompt for and parse Start Time of treatment, if invalid set to DateTime.MinValue
                Console.Write("Enter the start time of treatment (yyyy-MM-dd HH:mm:ss): ");
                DateTime timeStart;
                if (!DateTime.TryParse(Console.ReadLine(), out timeStart))
                {
                    Console.WriteLine("Invalid date input. Setting StartTime to default (DateTime.MinValue).");
                    timeStart = DateTime.MinValue;
                }

                // Prompt for and parse the actual end time of handling, if empty leave it as null
                Console.Write("Enter the actual end time of handling (yyyy-MM-dd HH:mm:ss) or leave empty if not applicable: ");
                DateTime? timeEnd = null;
                string? timeEndInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(timeEndInput) && DateTime.TryParse(timeEndInput, out DateTime parsedEndTime))
                {
                    timeEnd = parsedEndTime;
                }

                // Prompt for and parse the type of treatment conclusion, if empty leave it as null
                Console.Write("Enter the type of treatment conclusion (0 = Completed, 1 = Self-Cancelled, 2 = Admin-Cancelled, 3 = Expired) or leave empty for default: ");
                TypeEnd? typeEndTreat = null;
                string? typeEndTreatInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(typeEndTreatInput) && Enum.TryParse(typeEndTreatInput, out TypeEnd parsedTypeEnd))
                {
                    typeEndTreat = parsedTypeEnd;
                }

                // Return a new Assignment object with the gathered data
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
        // Config Menu method to handle system configuration settings
        private static void ConfigMenu()
        {
            bool exit = false;

            // Loop to keep displaying the menu until the user chooses to exit
            while (!exit)
            {
                Console.WriteLine("\nConfig Menu:");
                Console.WriteLine("1. Advance System Clock");
                Console.WriteLine("2. Show System Clock");
                Console.WriteLine("3. Set Config Variable");
                Console.WriteLine("4. Show Config Variable");
                Console.WriteLine("5. Reset Config");
                Console.WriteLine("0. Exit");

                // Parse the user's choice from the menu
                ConfigMenuOption choice = (ConfigMenuOption)int.Parse(Console.ReadLine() ?? "0");

                switch (choice)
                {
                    // Advance the system clock by a certain number of hours
                    case ConfigMenuOption.AdvanceSystemClock:
                        Console.Write("Enter the number of hours to advance the clock: ");
                        int hours = int.Parse(Console.ReadLine() ?? "0");
                        s_dalConfig!.Clock = s_dalConfig.Clock.AddHours(hours); // Update the clock in the config
                        Console.WriteLine($"System clock advanced by {hours} hours.");
                        break;

                    // Show the current system clock
                    case ConfigMenuOption.ShowSystemClock:
                        Console.WriteLine($"Current System Clock: {s_dalConfig?.Clock}");
                        break;

                    // Set a specific configuration variable
                    case ConfigMenuOption.SetConfigVariable:
                        Console.WriteLine("Choose the variable to set:");
                        Console.WriteLine("1. Risk Range (in hours)");

                        int option = int.Parse(Console.ReadLine() ?? "0");

                        switch (option)
                        {
                            // Set the risk range variable in hours
                            case 1:
                                Console.Write("Enter the new risk range in hours: ");
                                double hours1 = double.Parse(Console.ReadLine() ?? "1");
                                s_dalConfig!.RiskRange = TimeSpan.FromHours(hours1); // Set the risk range
                                Console.WriteLine($"Risk range set to {hours1} hours.");
                                break;
                            default:
                                Console.WriteLine("Invalid option.");
                                break;
                        }
                        break;

                    // Show the current configuration variables
                    case ConfigMenuOption.ShowConfigVariable:
                        Console.WriteLine($"Current System Clock: {s_dalConfig!.Clock}");
                        Console.WriteLine($"Risk Range: {s_dalConfig.RiskRange.TotalHours} hours");
                        break;

                    // Reset the configuration to its default settings
                    case ConfigMenuOption.ResetConfig:
                        s_dalConfig?.Reset(); // Reset the configuration
                        Console.WriteLine("Config reset to default.");
                        break;

                    // Exit the configuration menu
                    case ConfigMenuOption.Exit:
                        exit = true;
                        break;
                    // Handle invalid input for menu option
                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }
    }
}