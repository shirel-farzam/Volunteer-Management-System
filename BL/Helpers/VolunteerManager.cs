using BlImplementation;
using DalApi;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Helpers;

internal class VolunteerManager
{
    private static readonly IDal s_dal = Factory.Get;
    internal static readonly ObserverManager Observers = new(); // stage 5

    internal static BO.VolunteerInList ConvertDOToBOInList(DO.Volunteer doVolunteer)
    {
        List<DO.Assignment>? calls;
        lock (AdminManager.BlMutex) // stage 7
            calls = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();

        int totalCallsHandled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated);
        int totalCallsCanceled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.SelfCancel);
        int totalCallsExpired = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.ExpiredCancel);
        int? currentCallId = calls.FirstOrDefault(ass => ass.TimeEnd == null)?.Id;

        return new BO.VolunteerInList
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.FullName,
            Active = doVolunteer.Active,
            TotalCallsHandled = totalCallsHandled,
            TotalCallsCanceled = totalCallsCanceled,
            TotalCallsExpired = totalCallsExpired,
            CurrentCallId = currentCallId
        };
    }


    
    internal static void CheckFormat(BO.Volunteer boVolunteer)
    {
        // Validate the ID of the volunteer.
        if (boVolunteer.Id <= 0 || boVolunteer.Id.ToString().Length < 8 || boVolunteer.Id.ToString().Length > 9)
        {
            throw new BO.BlWrongItemException($"Invalid ID {boVolunteer.Id}. It must be 8-9 digits.");
        }

        // Validate the FullName field.
        if (string.IsNullOrWhiteSpace(boVolunteer.FullName) || !Regex.IsMatch(boVolunteer.FullName, @"^[a-zA-Z\s]+$"))
        {
            throw new BO.BlWrongItemException($"FullName {boVolunteer.FullName} cannot be null, empty, or contain invalid characters.");
        }

        // Validate the PhoneNumber field.
        if (string.IsNullOrWhiteSpace(boVolunteer.PhoneNumber))
        {
            throw new BO.BlWrongItemException($"PhoneNumber {boVolunteer.PhoneNumber} cannot be null or empty.");
        }

        if (boVolunteer.PhoneNumber.Length != 10 || !boVolunteer.PhoneNumber.All(char.IsDigit) || boVolunteer.PhoneNumber[0] != '0')
        {
            throw new BO.BlWrongItemException($"Invalid PhoneNumber {boVolunteer.PhoneNumber}.");
        }

        // Validate the Email field.
        if (!Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new BO.BlWrongItemException("Invalid Email format.");
        }

        if (boVolunteer.MaxReading.HasValue && boVolunteer.MaxReading.Value <= 0)
        {
            throw new BO.BlWrongItemException($"MaxReading {boVolunteer.MaxReading} must be a positive number.");
        }

        if (boVolunteer.Latitude.HasValue && (boVolunteer.Latitude.Value < -90 || boVolunteer.Latitude.Value > 90))
        {
            throw new BO.BlWrongItemException("Latitude must be between -90 and 90.");
        }

        if (boVolunteer.Longitude.HasValue && (boVolunteer.Longitude.Value < -180 || boVolunteer.Longitude.Value > 180))
        {
            throw new BO.BlWrongItemException($"Longitude {boVolunteer.Longitude} must be between -180 and 180.");
        }
    }

    internal static void CheckLogic(BO.Volunteer boVolunteer)
    {
        try
        {
            CheckId(boVolunteer.Id);
            CheckPhoneNumber(boVolunteer.PhoneNumber);
            CheckEmail(boVolunteer.Email);
            CheckPassword(boVolunteer.Password);
            CheckAddress(boVolunteer);
        }
        catch (BO.BlWrongItemException ex)
        {
            throw new BO.BlWrongItemException("The item has a logic problem", ex);
        }
    }

    internal static void CheckId(int id)
    {
        string idString = id.ToString();
        if (idString.Length != 9)
        {
            throw new BO.BlWrongItemException($"This ID {id} is not valid.");
        }

        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = idString[i] - '0';
            int multiplier = (i % 2) + 1;
            int product = digit * multiplier;
            sum += product > 9 ? product - 9 : product;
        }

        if (sum % 10 != 0)
        {
            throw new BO.BlWrongItemException($"This ID {id} is not valid.");
        }
    }

    internal static void CheckPhoneNumber(string phoneNumber)
    {
        if (phoneNumber.Length != 10 || !phoneNumber.All(char.IsDigit) || phoneNumber[0] != '0')
        {
            throw new BO.BlWrongItemException($"Invalid phone number {phoneNumber}.");
        }
    }

    internal static void CheckEmail(string email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailPattern))
        {
            throw new BO.BlWrongItemException($"Invalid email {email}.");
        }
    }

    internal static void CheckPassword(string password)
    {
        if (password.Length < 5)
            throw new BO.BlWrongItemException("Password must be at least 5 characters long.");

        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            throw new BO.BlWrongItemException("Password must contain at least one letter and one digit.");
    }

    private const string ApiKey = "pk.3d8d3ac902d00ffcd65fdf9a26ec253c";
    private const string LocationIqBaseUrl = "https://us1.locationiq.com/v1/search.php";


    internal static async Task<double[]> GetCoordinatesFromAddressAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("The provided address is empty or invalid.", nameof(address));
        }

        using var client = new HttpClient();
        string requestUrl = $"{LocationIqBaseUrl}?key={ApiKey}&q={Uri.EscapeDataString(address)}&format=json";

        // קריאה אסינכרונית ל-API
        HttpResponseMessage response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error making geocoding request: {response.StatusCode}");
        }

        // קריאה אסינכרונית לתוכן התשובה
        string jsonResponse = await response.Content.ReadAsStringAsync();
        JsonDocument document = JsonDocument.Parse(jsonResponse);
        JsonElement root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
        {
            JsonElement firstResult = root[0];

            if (firstResult.TryGetProperty("lat", out JsonElement latElement) &&
                firstResult.TryGetProperty("lon", out JsonElement lonElement))
            {
                double latitude = double.Parse(latElement.GetString()!, CultureInfo.InvariantCulture);
                double longitude = double.Parse(lonElement.GetString()!, CultureInfo.InvariantCulture);

                return new[] { latitude, longitude };
            }
        }

        throw new Exception("Latitude or Longitude is missing in the response.");
    }
    internal static async Task updateCoordinatesForVolonteerAddressAsync(DO.Volunteer doVolunteer)
    {
        if (doVolunteer.FullAddress is not null)
        {
            double[] loc = await GetCoordinatesFromAddressAsync(doVolunteer.FullAddress);
            if (loc is not null)
            {
                doVolunteer = doVolunteer with { Latitude = loc[0], Longitude = loc[1] };
                lock (AdminManager.BlMutex)
                    s_dal.Volunteer.Update(doVolunteer);
                Observers.NotifyListUpdated();
                Observers.NotifyItemUpdated(doVolunteer.Id);
            }
        }
    }
    internal static async Task updateCoordinatesForCallAddressAsync(DO.Call doCall)
    {
        if (doCall.FullAddress is not null)
        {
            double[] loc = await GetCoordinatesFromAddressAsync(doCall.FullAddress);
            if (loc is not null)
            {
                doCall = doCall with { Latitude = loc[0], Longitude = loc[1] };
                lock (AdminManager.BlMutex)
                    s_dal.Call.Update(doCall);
                CallManager.Observers.NotifyListUpdated();
                CallManager.Observers.NotifyItemUpdated(doCall.Id);
            }
        }
    }

    internal static void CheckAddress(BO.Volunteer volunteer) // לבדוק האם היא תקינה  
    {
        // מחשב את הקואורדינטות באופן אסינכרוני אך לא מחכה לתוצאה
        _ = Task.Run(async () =>
        {
            double[] coordinates = await GetCoordinatesFromAddressAsync(volunteer.FullAddress);

            if (coordinates[0] != volunteer.Latitude || coordinates[1] != volunteer.Longitude)
                throw new BO.BlWrongItemException("Coordinates do not match.");
        });
    }


    internal static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadius = 6371000; // meters
        double lat1Rad = lat1 * Math.PI / 180;
        double lon1Rad = lon1 * Math.PI / 180;
        double lat2Rad = lat2 * Math.PI / 180;
        double lon2Rad = lon2 * Math.PI / 180;

        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadius * c;
    }
    //private static readonly Random s_rand = new();
    //private static int s_simulatorCounter = 0;

    //internal static void SimulateVolunteerRegistrationAndGrade() //stage 7
    //{
    //    Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";
    //    // var volunteerlist = GetVolunteerListHelp(true, null).ToList();
    //    //רשימת מתנדבים פעילים
    //    List<BO.VolunteerInList> activeVolunteers;
    //    lock (AdminManager.BlMutex)
    //        activeVolunteers = ReadAllInternal(true, null, null).ToList();
    //    Random random = new Random(); // יצירת מופע Random מחוץ ללולאה
    //    foreach (var volunteer in activeVolunteers)
    //    {
    //        bool hasUpdated = false;
    //        // אם למתנדב אין קריאה בטיפולו
    //        if (volunteer.CurrentCallId == null)
    //        {
    //            // הסתברות לבחירה רנדומלית של קריאה
    //            if (random.Next(0, 100) < 20) // הסתברות של 20%
    //            {
    //                var ChooseOpenCallInList = CallManager.GetOpenCallsForVolunteerInternal(volunteer.Id, null, null).ToList();

    //                if (ChooseOpenCallInList != null && ChooseOpenCallInList.Count > 0)
    //                {
    //                    //choose random call for volunteer
    //                    var randomIndex = s_rand.Next(ChooseOpenCallInList.Count);
    //                    var chosenCall = ChooseOpenCallInList[randomIndex];

    //                    CallManager.ChooseCallForTreatInternal(volunteer.Id, chosenCall.Id);
    //                }
    //            }

    //            else if (volunteer.CurrentCallId != null)    //there is call in treat
    //            {
    //                var callin = ReadInternal(volunteer.Id).CurrentCall!;
    //                if ((AdminManager.Now - callin.OpeningTime) >= TimeSpan.FromHours(3))
    //                {
    //                    CallManager.ChooseCallForTreatInternal(volunteer.Id, callin.CallId);
    //                }
    //                else
    //                {
    //                    int probability1 = s_rand.Next(1, 101); // מספר אקראי בין 1 ל-100

    //                    if (probability1 <= 10) // הסתברות של 10%
    //                    {
    //                        // ביטול הטיפול
    //                        CallManager.CancelTreatInternal(volunteer.Id, callin.Id);
    //                    }
    //                }
    //            }

    //        }
    //    }
    //}
    //internal static void SimulateVolunteerActivity() //stage 7
    //{
    //    // var volunteerImplementation = new VolunteerImplementation();
    //    Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";

        
    //    double probability = 0.2;

    //    // יצירת מספר אקראי בטווח 0 עד 1
    //    double randomValue = s_rand.NextDouble(); // מספר בין 0.0 ל-1.0
    //    var volunteerList = ReadAllInternal(true, null,null).ToList();
    //    int size = volunteerList.Count();
    //    // בדיקה אם המספר האקראי קטן מההסתברות
    //    for (int i = 0; i < size; i++)
    //    {
    //        var volunteer = ReadInternal(volunteerList[i].Id);
    //        if (volunteer.CurrentCall == null && randomValue < probability)
    //        {
    //            var openCallInListsToChose = CallManager.GetOpenCallsForVolunteerInternal(volunteer.Id, null, null).ToList();

    //            if (openCallInListsToChose != null && openCallInListsToChose.Count > 0)
    //            {
    //                //choose random call for volunteer
    //                var randomIndex = s_rand.Next(openCallInListsToChose.Count);
    //                var chosenCall = openCallInListsToChose[randomIndex];

    //                CallManager.ChooseCallForTreatInternal(volunteer.Id, chosenCall.Id);
    //            }
    //        }

    //        else if (volunteer.CurrentCall != null)    //there is call in treat
    //        {
    //            var callin = ReadInternal(volunteer.Id).CurrentCall!;
    //            if ((AdminManager.Now - callin.OpeningTime) >= TimeSpan.FromHours(3))
    //            {
    //                CallManager.CloseTreatInternal(volunteer.Id, callin.Id);
    //            }
    //            else
    //            {
    //                int probability1 = s_rand.Next(1, 101); // מספר אקראי בין 1 ל-100

    //                if (probability1 <= 10) // הסתברות של 10%
    //                {
    //                    // ביטול הטיפול
    //                    CallManager.CancelTreatInternal(volunteer.Id, callin.Id);
    //                }
    //            }
    //        }

    //    }
    //}
    //// Internal method for the original logic
    public static BO.Role LoginInternal(int username, string password)
    {
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex) // stage 7
             volunteer = s_dal.Volunteer.Read(username)
            ?? throw new BO.BlNullPropertyException("The volunteer does not exist");
        VolunteerManager.CheckPassword(password); // Validate password format
        if (volunteer.Password != password)
            throw new BO.BlWrongInputException("Incorrect password");

        return (BO.Role)volunteer.Job; // Return volunteer's role
    }
    // The internal implementation of the ReadAll logic
    public static IEnumerable<BO.VolunteerInList> ReadAllInternal(bool? isActive, BO.VolunteerInListField? sortField, BO.CallType? callType)
    {
        IEnumerable<DO.Volunteer> volunteers;
        lock (AdminManager.BlMutex) // stage 7
        {
            volunteers = s_dal.Volunteer.ReadAll()
                ?? throw new BO.BlNullPropertyException("No volunteers in the database");

            // Convert DO.Volunteer to BO.VolunteerInList
            var boVolunteersInList = volunteers
                .Select(VolunteerManager.ConvertDOToBOInList);

            // Apply filter for active status if specified
            var filteredVolunteers = isActive.HasValue
                ? boVolunteersInList.Where(v => v.Active == isActive)
                : boVolunteersInList;

            // Apply filter for call type if specified
            if (callType.HasValue && callType != BO.CallType.None)
            {
                filteredVolunteers = filteredVolunteers.Where(v => v.CurrentCallType == callType);
            }

            // Apply sorting if specified
            var sortedVolunteers = sortField.HasValue
                ? filteredVolunteers.OrderBy(v => sortField switch
                {
                    BO.VolunteerInListField.Id => (object)v.Id,
                    BO.VolunteerInListField.FullName => v.FullName,
                    BO.VolunteerInListField.Active => v.Active,
                    BO.VolunteerInListField.TotalCallsHandled => v.TotalCallsHandled,
                    BO.VolunteerInListField.TotalCallsCanceled => v.TotalCallsCanceled,
                    BO.VolunteerInListField.TotalCallsExpired => v.TotalCallsExpired,
                    BO.VolunteerInListField.CurrentCallId => v.CurrentCallId ?? 0,
                    BO.VolunteerInListField.CurrentCallType => v.CurrentCallType.ToString(),
                    _ => v.Id
                })
                : filteredVolunteers.OrderBy(v => v.Id); // Default sort by ID

            return sortedVolunteers;
        }
    }
    // Internal implementation of the Read logic
    public static BO.Volunteer ReadInternal(int volunteerId)
    {
        DO.Volunteer? doVolunteer;
        lock (AdminManager.BlMutex) // stage 7
        {
            doVolunteer = s_dal.Volunteer.Read(volunteerId)
               ?? throw new BO.BlWrongInputException($"Volunteer with ID={volunteerId} does not exist");
        }
        var calls = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();

        int totalCallsHandled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated);
        int totalCallsCanceled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.SelfCancel);
        int totalCallsExpired = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.ExpiredCancel);
        int? currentCallId = calls.FirstOrDefault(ass => ass.TimeEnd == null)?.Id;

        return new BO.Volunteer
        {
            Id = volunteerId,
            FullName = doVolunteer.FullName,
            PhoneNumber = doVolunteer.PhoneNumber,
            TypeDistance = (BO.Distance)doVolunteer.TypeDistance,
            Email = doVolunteer.Email,
            Job = (BO.Role)doVolunteer.Job,
            Active = doVolunteer.Active,
            Password = doVolunteer.Password,
            FullAddress = doVolunteer.FullAddress,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            //CurrentCall = VolunteerManager.(doVolunteer),
            TotalCanceledCalls = totalCallsCanceled,
            TotalExpiredCalls = totalCallsExpired,
            TotalHandledCalls = totalCallsHandled,
            MaxReading = doVolunteer.MaxReading
        };
    }

    // Internal implementation of the Update logic
    public static void UpdateInternal(int volunteerId, BO.Volunteer boVolunteer)
    {
        DO.Volunteer? doVolunteer;
        lock (AdminManager.BlMutex)
            doVolunteer = s_dal.Volunteer.Read(volunteerId)
                ?? throw new BO.BlWrongInputException($"Volunteer with ID={volunteerId} does not exist");
        DO.Volunteer? manager;
        lock (AdminManager.BlMutex)
            manager = s_dal.Volunteer.Read(boVolunteer.Id)
                ?? throw new BO.BlWrongInputException($"Manager with ID={boVolunteer.Id} does not exist");

        if (manager.Job != DO.Role.Manager && volunteerId != boVolunteer.Id)
            throw new BO.BlWrongInputException("Only a manager can update details");

      

        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);

        var volunteerUpdate = new DO.Volunteer(
            boVolunteer.Id,
            boVolunteer.FullName,
            boVolunteer.PhoneNumber,
            boVolunteer.Email,
            (DO.Distance)boVolunteer.TypeDistance,
            (DO.Role)boVolunteer.Job,
            boVolunteer.Active,
            boVolunteer.Password,
            boVolunteer.FullAddress,
            boVolunteer.Latitude,
            boVolunteer.Longitude,
            boVolunteer.MaxReading
        );

        try
        {
            lock (AdminManager.BlMutex)
                s_dal.Volunteer.Update(volunteerUpdate);
            VolunteerManager.Observers.NotifyItemUpdated(volunteerUpdate.Id);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
    }

    // Internal implementation of the DeleteVolunteer logic
    public static void DeleteVolunteerInternal(int volunteerId)
    {
        DO.Volunteer? doVolunteer;
        IEnumerable<DO.Assignment> assignments;

        lock (AdminManager.BlMutex) // stage 7
            doVolunteer = s_dal.Volunteer.Read(volunteerId);
        lock (AdminManager.BlMutex) // stage 7
            assignments = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == volunteerId);

        if (assignments.Any(ass => ass.TimeEnd == null))
            throw new BO.BlWrongInputException("Volunteer has active assignments");

        try
        {
            lock (AdminManager.BlMutex) // stage 7
                s_dal.Volunteer.Delete(volunteerId);
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (DO.DalDeletionImpossible ex)
        {
            throw new BO.BlDeleteNotPossibleException("Deletion failed due to invalid ID", ex);
        }
    }

        // Internal implementation of the AddVolunteer logic
    public static void AddVolunteerInternal(BO.Volunteer boVolunteer)
    {  
       

            VolunteerManager.CheckLogic(boVolunteer);
            VolunteerManager.CheckFormat(boVolunteer);

            var doVolunteer = new DO.Volunteer(
                boVolunteer.Id,
                boVolunteer.FullName,
                boVolunteer.PhoneNumber,
                boVolunteer.Email,
                (DO.Distance)boVolunteer.TypeDistance,
                (DO.Role)boVolunteer.Job,
                boVolunteer.Active,
                boVolunteer.Password,
                boVolunteer.FullAddress,
                null,
                null,
                boVolunteer.MaxReading
            );


            try
            {
                lock (AdminManager.BlMutex) // stage 7
                    s_dal.Volunteer.Create(doVolunteer);
                VolunteerManager.Observers.NotifyListUpdated(); //stage 5
            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
            }

        
       _=VolunteerManager.GetCoordinatesFromAddressAsync(boVolunteer.FullAddress);

    }

    private static Random s_rand = new Random();
    private static int s_simulatorCounter;

    internal static void SimulateVolunteerRegistrationAndGrade() //stage 7
    {
        // var volunteerImplementation = new VolunteerImplementation();
        Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";

        // var volunteerList = volunteerImplementation.GetVolunteerList(true,null);
        // var volunteerlist = GetVolunteerListHelp(true, null)/.ToList()/;
        double probability = 0.2;

        // יצירת מספר אקראי בטווח 0 עד 1
        double randomValue = s_rand.NextDouble(); // מספר בין 0.0 ל-1.0
        var volunteerList = ReadAllInternal(true, null,null).Where(v => v.Active).ToList();
        int size = volunteerList.Count();
        // בדיקה אם המספר האקראי קטן מההסתברות
        for (int i = 0; i < size; i++)
        {
            var volunteer = ReadInternal(volunteerList[i].Id);
            if (volunteer.CurrentCall == null && randomValue < probability)
            {
                var openCallInListsToChose = CallManager.GetOpenCallsForVolunteerInternal(volunteer.Id, null, null).ToList();

                if (openCallInListsToChose != null && openCallInListsToChose.Count > 0)
                {
                    //choose random call for volunteer
                    var randomIndex = s_rand.Next(openCallInListsToChose.Count);
                    var chosenCall = openCallInListsToChose[randomIndex];

                    CallManager.ChooseCallForTreatInternal(volunteer.Id, chosenCall.Id);
                }
            }

            else if (volunteer.CurrentCall != null)    //there is call in treat
            {
                var callin = ReadInternal(volunteer.Id).CurrentCall!;
                if ((AdminManager.Now - callin.EntryTime) >= TimeSpan.FromMinutes(15))
                {
                    CallManager.UpdateTreatmentCancellationInternal(volunteer.Id, callin.Id);
                }
                else
                {
                    int probability1 = s_rand.Next(1, 101); // מספר אקראי בין 1 ל-100

                    if (probability1 <= 10) // הסתברות של 10%
                    {
                        // ביטול הטיפול
                        CallManager.UpdateTreatmentCancellationInternal(volunteer.Id, callin.Id);
                    }
                }
            }

        }
    }

}
