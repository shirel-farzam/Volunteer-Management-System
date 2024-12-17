using BlApi;
using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;

using Dal;

namespace Helpers;

internal static class CallManager
{
    private static IDal _dal = DalApi.Factory.Get; //stage 4
    internal static void IsLogicCall(BO.Call boCall)
    {
        // Ensure MaxEndTime is greater than OpenTime.
        if (boCall.MaxEndTime.HasValue && boCall.MaxEndTime.Value <= boCall.OpenTime)
        {
            throw new ArgumentException("MaxEndTime must be greater than OpenTime.");
        }

        // Check that CallType is valid (assuming enums start at 0).
        if (!Enum.IsDefined(typeof(BO.CallType), boCall.Type))
        {
            throw new ArgumentException("Invalid call type.");
        }

        // Validate the status (assuming statuses start at 0).
        if (!Enum.IsDefined(typeof(BO.CallStatus), boCall.Status))
        {
            throw new ArgumentException("Invalid call status.");
        }

    }
    internal static void IsValideCall(BO.Call boCall)
    {
        // Validate that the ID is positive.
        if (boCall.Id <= 0)
        {
            throw new ArgumentException("Call ID must be a positive integer.");
        }

        // Validate that the description is not null or empty.
        if (string.IsNullOrWhiteSpace(boCall.Description))
        {
            throw new ArgumentException("Description cannot be null or empty.");
        }

        // Validate that the latitude is within valid range (-90 to 90).
        if (boCall.Latitude != null && (boCall.Latitude < -90 || boCall.Latitude > 90))
        {
            throw new ArgumentException("Latitude must be between -90 and 90.");
        }

        // Validate that the longitude is within valid range (-180 to 180).
        if (boCall.Longitude != null && (boCall.Longitude < -180 || boCall.Longitude > 180))
        {
            throw new ArgumentException("Longitude must be between -180 and 180.");
        }

        // Validate that the address is not null or empty.
        if (string.IsNullOrWhiteSpace(boCall.FullAddress))
        {
            throw new ArgumentException("Address cannot be null or empty.");
        }

        // Validate the address format using an external API.
        if (!Tools.IsAddressValid(boCall.FullAddress).Result)
        {
            throw new ArgumentException("The address is invalid.");
        }
    }


    // CreateCallInProgress and 3 helper methods
    public static BO.CallInProgress GetCallInProgress(int VolunteerId)
    {

        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.TimeEnd == null).FirstOrDefault();
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();


        // בודק האם הכתובת אמיתית ומחזיר קווי אורך ורוחב עבור הכתובת 

        if (Tools.IsAddressValid(doVolunteer.FullAddress).Result == false)// לא כתובת אמיתית 
        {
            throw new BlInvalidaddress("Invalid address of Volunteer");// 
        }
        double? LatitudeVolunteer = Tools.GetLatitudeAsync(doVolunteer.FullAddress).Result;
        double? LongitudeVolunteer = Tools.GetLongitudeAsync(doVolunteer.FullAddress).Result;


        return new BO.CallInProgress

        {
            Id = doAssignment.Id,
            CallId = doAssignment.CallId,
            CallType = (BO.CallType)doCall.Type,
            Description = doCall.Description,
            FullAddress = doCall.FullAddress,
            OpeningTime = doCall.TimeOpened,
            MaxCompletionTime = doCall.MaxTimeToClose,
            EntryTime = doAssignment.TimeStart,
            DistanceFromVolunteer = Air_distance_between_2_addresses(doCall.Latitude, doCall.Longitude, LatitudeVolunteer, LongitudeVolunteer),// Air distance between 2 addresses
            Status = CallManager.CalculateCallStatus(doCall.Id)

        };
    }

    private const double EarthRadiusKm = 6371.0; // Earth's radius in kilometers

    /// <summary>
    /// Calculates the air (great-circle) distance between two geographic points based on their latitude and longitude coordinates.
    /// </summary>
    /// <param name="lat1">Latitude of the first point</param>
    /// <param name="lon1">Longitude of the first point</param>
    /// <param name="lat2">Latitude of the second point</param>
    /// <param name="lon2">Longitude of the second point</param>
    /// <returns>The air distance in kilometers</returns>
    public static double Air_distance_between_2_addresses(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        // Check if any of the coordinates is null
        if (!lat1.HasValue || !lon1.HasValue || !lat2.HasValue || !lon2.HasValue)
        {
            throw new ArgumentNullException("One or more coordinate values are null.");
        }
        // Convert degrees to radians

        double lat1Rad = DegreesToRadians(lat1.Value);
        double lon1Rad = DegreesToRadians(lon1.Value);
        double lat2Rad = DegreesToRadians(lat2.Value);
        double lon2Rad = DegreesToRadians(lon2.Value);

        // Calculate coordinate differences
        double dLat = lat2Rad - lat1Rad;
        double dLon = lon2Rad - lon1Rad;

        // Compute the haversine formula
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Calculate the distance
        return EarthRadiusKm * c;
    }
    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">The angle in degrees</param>
    /// <returns>The angle in radians</returns>
    public static CallStatus CalculateCallStatus(int callId)
    {
        // שליפת הקריאה המתאימה מתוך ה-DAL
        var call = _dal.Call.ReadAll().FirstOrDefault(c => c.Id == callId);

        if (call == null)
        {
            throw new ArgumentException($"Call with ID {callId} not found.");
        }

        // זמן מקסימלי לסיום הקריאה
        DateTime? maxEndTime = call.MaxTimeToClose;

        // שליפת כל האסיינמנטים של הקריאה
        var assignments = _dal.Assignment.ReadAll()
            .Where(a => a.CallId == callId)
            .ToList();

        if (!assignments.Any())
        {
            return CallStatus.Open; // קריאה ללא אסיינמנטים, נחשבת כפתוחה
        }

        // שימוש בזמן הנוכחי
        DateTime currentClock = ClockManager.Now;

        // קריאה שהסתיימה ומבוטלת
        if (assignments.Any(a => a.TimeEnd == AssignmentCompletionType.AdminCanceled ||
                                 a.TimeEnd == AssignmentCompletionType.Canceled))
        {
            return CallStatus.Closed;
        }

        // קריאה שפג תוקפה
        if (maxEndTime.HasValue && maxEndTime <= currentClock)
        {
            return CallStatus.Expired;
        }

        // קריאה בטיפול בסיכון
        if (assignments.Any(a => a.TypeEndTreat != default &&
                                 a.TypeEndTreat == null &&
                                 maxEndTime.HasValue &&
                                 maxEndTime <= currentClock))
        {
            return CallStatus.InProgressRisk;
        }

        // קריאה בטיפול
        if (assignments.Any(a => a.TypeEndTreat == null))
        {
            return CallStatus.InProgress;
        }

        // קריאה פתוחה בסיכון
        if (assignments.All(a => a.TypeEndTreat == null) &&
            maxEndTime.HasValue &&
            maxEndTime <= currentClock)
        {
            return CallStatus.OpenRisk;
        }

        // קריאה פתוחה
        return CallStatus.Open;
    }



    // CALL - function for viewing, function that checks for correctness (add and update) + helper method
    // helper method- 1- GetCallAssignmentsForCall 2- CalculateCallStatus
    public static BO.Call GetViewingCall(int VolunteerId)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.TimeEnd == null).FirstOrDefault();
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

        // Create the object
        return new BO.Call
        {

            Id = doCall.Id, // Call identifier
            Type = (BO.CallType)doCall.Type, // Enum conversion
            Description = doCall.Description,
            FullAddress = doCall.FullAddress, // Full address of the call
            Latitude = (double)doCall.Latitude, // Latitude coordinate of the address
            Longitude = (double)doCall.Longitude, // Longitude coordinate of the address
            OpenTime = doCall.TimeOpened, // Time when the call was opened
            MaxEndTime = doCall.MaxTimeToClose, // Maximum completion time for the call
            Status = CalculateCallStatus(doCall.Id), // Current status of the call
            CallAssignments = CallManager.GetCallAssignmentsForCall(doCall.Id),

        };
    }
    public static List<BO.CallInList> GetCallAssignmentsForCall(int callId)
    {
        // For the CallAssignments field in the GetViewingCall function


        // Search for all assignments related to the given call
        var doAssignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId).ToList();

        // If no assignments are found, return null
        if (!doAssignments.Any())
        {
            return null; // No assignments found
        }

        // Use LINQ to convert the assignments into BO.CallAssignInList using the GetCallAssignInList function
        var callAssignInList = (from doAssignment in doAssignments
                                let doVolunteer = _dal.Volunteer.Read(doAssignment.VolunteerId)
                                where doVolunteer != null
                                select GetCallAssignInList(doAssignment.VolunteerId)) // Calls the conversion function
                                .ToList();

        // Return the complete list
        return callAssignInList;
    }

    // CALL - function for Add or update
    public static BO.Call GetAdd_update_Call(int VolunteerId)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.EndOfTime == null).FirstOrDefault();
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

        // logic chack
        if (Tools.IsAddressValid(doCall.FullAddress).Result)
            throw new BlInvalidaddress($"The address = {doCall.FullAddress}provided is invalid.");
        MaxEndTimeCheck(doCall.MaxTimeToClose, doCall.TimeOpened);// If not good throw an exception from within the method


        // Create the object
        return new BO.Call
        {

            Id = doCall.Id, // Call identifier
            Type = (BO.CallType)doCall.Type, // Enum conversion
            Description = doCall.Description,
            FullAddress = doCall.FullAddress, // Full address that chack above 
            Latitude = Tools.GetLatitudeAsync(doCall.FullAddress).Result, // Latitude coordinate of the address
            Longitude = Tools.GetLongitudeAsync(doCall.FullAddress).Result, // Longitude coordinate of the address
            OpenTime = doCall.TimeOpened, // Time when the call was opened
            MaxEndTime = doCall.MaxTimeToClose, // Maximum completion time for the call
            Status = CalculateCallStatus(doCall.Id), // Current status of the call
            CallAssignments = CallManager.GetCallAssignmentsForCall(doCall.Id),

        };
    }
    public static void MaxEndTimeCheck(DateTime? MaxEndTime, DateTime OpeningTime)
    {
        if (MaxEndTime < OpeningTime || MaxEndTime < ClockManager.Now)
        {
            throw new BlMaximum_time_to_finish_readingException("The time entered according to the current time or opening time");
        }
    }


    // GetCallAssignInList
    public static BO.CallAssignInList GetCallAssignInList(int Id)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(Id) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == Id && a.EndOfTime == null).FirstOrDefault();
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

        return new BO.CallAssignInList
        {
            VolunteerId = doAssignment.VolunteerId, // The ID of the volunteer
            VolunteerName = doVolunteer.FullName, // The name of the volunteer (helper method can be used)
            EnterTime = doAssignment.time_entry_treatment, // The time the volunteer started handling the call
            CompletionTime = doAssignment.time_end_treatment, // The time the handling of the call was completed
            CompletionStatus = (BO.CallAssignmentEnum?)doAssignment.EndOfTime // Completion status (nullable)
        };



    }

    // GetCallInList
    public static BO.CallInList GetCallInList(int Id)
    {
        //DO.Volunteer? doVolunteer = _dal.Volunteer.Read(Id) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignmentn by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == Id && a.EndOfTime == null).FirstOrDefault();// לבדוק
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();
        var GetTotalAssignmentsForCall = _dal.Assignment.ReadAll().Where(a => a.Id == Id);

        return new BO.CallInList
        {
            Id = doAssignment?.Id,
            CallId = doAssignment.CallId,
            Type = (BO.CallType)doCall.Type, // Enum conversion
            OpeningTime = doCall.TimeOpened, // The time when the call was opened

            TimeToFinish = CalculateTimeRemaining(doCall.MaxTimeToClose), // Time remaining until the maximum completion time
            LastVolunteerName = GetLatestVolunteerNameForCall(doAssignment.VolunteerId), // The name of the volunteer assigned to the call

            TreatmentDuration = CalculateCompletionTime(doAssignment.Id), // Total time taken to complete the call
            Status = CalculateCallStatus(doCall.Id), // Current status of the call
            TotalAssignments = GetTotalAssignmentsForCall.Count(a => a.CallId == doAssignment.CallId) // Total number of assignments for the call

        };

    }


    // Convert 
    public static DO.Call BOConvertDO_Call(int Id)
    {
        // Retrieve the DO.Call object using the provided ID
        var BOCall = _dal.Call.Read(Id);
        if (BOCall == null)
        {
            throw new BO.Incompatible_ID($"Call with ID {Id} not found.");
        }

        // Convert DO.Call to BO.Call
        var DOCall = new DO.Call
        {
            Id = BOCall.Id,
            Type = (DO.CallType)BOCall.Type, // Explicit cast to BO.Calltype enum
            Description = BOCall.Description,
            FullAddress = BOCall.FullAddress,
            Latitude = BOCall.Latitude ?? 0, // Convert nullable to non-nullable
            Longitude = BOCall.Longitude ?? 0, // Convert nullable to non-nullable
            TimeOpened = BOCall.TimeOpened,
            MaxTimeToClose = BOCall.MaxTimeToClose
        };

        return DOCall;
    }


    public static TimeSpan? CalculateTimeRemaining(DateTime? maxEndTime)
    {
        if (maxEndTime == null)
            return null;

        return maxEndTime - ClockManager.Now;
    }
    public static string? GetLatestVolunteerNameForCall(int callId)
    {
        // Retrieve all assignments related to the call
        var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);

        if (!assignments.Any())
            return null;

        // Find the assignment with the latest entry time closest to the current system time
        var latestAssignment = assignments
            .OrderByDescending(a => a.TypeEndTreat)
            .FirstOrDefault();

        if (latestAssignment == null || latestAssignment.VolunteerId == null)
            return null;

        // Retrieve the volunteer associated with the assignment
        var volunteer = _dal.Volunteer.Read((int)latestAssignment.VolunteerId);
        return volunteer?.FullName;
    }
    public static TimeSpan? CalculateCompletionTime(int callId)
    {
        // Retrieve all assignments related to the call
        var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);

        // Check if the call has been completed
        var completedAssignment = assignments
            .Where(a => a.TimeEnd != null && a.TypeEndTreat != null)
            .OrderByDescending(a => a.TypeEndTreat)
            .FirstOrDefault();

        if (completedAssignment == null)
            return null;

        // Calculate the time taken to complete the call
        return completedAssignment.time_end_treatment - completedAssignment.time_entry_treatment;
    }




}