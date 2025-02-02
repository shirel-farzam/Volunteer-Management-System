using BlApi;
using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;

using Dal;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Helpers;

internal static class CallManager
{
    private static IDal _dal = DalApi.Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 
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


        DO.Volunteer? doVolunteer;
        lock (AdminManager.BlMutex)
            doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");

        //Find the appropriate CALL  and  Assignment by volunteer ID
        Assignment? doAssignment;
        lock (AdminManager.BlMutex)
            doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.TimeEnd == null).FirstOrDefault();
        DO.Call? doCall;
        lock (AdminManager.BlMutex)
            doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

        if (Tools.IsAddressValid(doVolunteer.FullAddress).Result == false) // לא כתובת אמיתית 
        {
            throw new BlWrongInputException("Invalid address of Volunteer");
        }

        double? LatitudeVolunteer = Tools.GetLatitude(doVolunteer.FullAddress);
        double? LongitudeVolunteer = Tools.GetLongitude(doVolunteer.FullAddress);

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
            DistanceFromVolunteer = Air_distance_between_2_addresses(doCall.Latitude, doCall.Longitude, LatitudeVolunteer, LongitudeVolunteer), // Air distance between 2 addresses
            Status = CallManager.CalculateCallStatus(doCall)
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
        lock (AdminManager.BlMutex)
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

    internal static BO.CallStatus CalculateCallStatus(DO.Call doCall)
    { 
        if (doCall.MaxTimeToClose < _dal.Config.Clock)
            return BO.CallStatus.Expired;
        Assignment? lastAssignment;
        lock (AdminManager.BlMutex)
            lastAssignment = _dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id).OrderByDescending(a => a.TypeEndTreat).FirstOrDefault();

        if (lastAssignment == null)
        {
            if (IsInRisk(doCall!))
                return BO.CallStatus.OpenRisk;
            else return BO.CallStatus.Open;
        }

        if (lastAssignment.TimeEnd.ToString() == "TreatedOnTime")
        {
            return BO.CallStatus.Closed;
        }

        if (lastAssignment.TypeEndTreat == null)
        {
            if (IsInRisk(doCall!))
                return BO.CallStatus.InProgressRisk;
            else return BO.CallStatus.InProgress;
        }

        return BO.CallStatus.Closed; //default

    }

    public static BO.CallStatus GetCallStatus(DO.Call doCall)
    {

        if (doCall.MaxTimeToClose < _dal.Config.Clock)
            return BO.CallStatus.Expired;

        Assignment? lastAssignment;
        lock (AdminManager.BlMutex)
           lastAssignment = _dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id).OrderByDescending(a => a.Id).FirstOrDefault();

        if (lastAssignment == null)
        {
            if (IsInRisk(doCall!))
                return BO.CallStatus.OpenRisk;
            else return BO.CallStatus.Open;
        }

        if (lastAssignment.TypeEndTreat.ToString() == "Treated")
        {
            return BO.CallStatus.Closed;
        }

        if (lastAssignment.TypeEndTreat == null)
        {
            if (IsInRisk(doCall!))
                return BO.CallStatus.InProgressRisk;
            else return BO.CallStatus.InProgress;
        }

        if (lastAssignment.TypeEndTreat.ToString() == "ExpiredCancel")
        {
            return BO.CallStatus.Expired;
        }

        if (IsInRisk(doCall!))
            return BO.CallStatus.OpenRisk;

        return BO.CallStatus.Open;

    }

    public static void MaxEndTimeCheck(DateTime? MaxEndTime, DateTime OpeningTime)
    {


        if (MaxEndTime < OpeningTime || MaxEndTime < AdminManager.Now)
        {
            throw new BlWrongInputException("The time entered according to the current time or opening time");
        }

    }

    //GetCallAssignInList
    //public static BO.CallAssignmentInList GetCallAssignInList(int Id)
    //{
    //    DO.Volunteer? doVolunteer = _dal.Volunteer.Read(Id) ?? throw new BlDoesNotExistException("eroor id");// ז

    //    //Find the appropriate CALL  and  Assignment by volunteer ID
    //    var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == Id && a.TimeEnd == null).FirstOrDefault();
    //    var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

    //    return new BO.CallAssignmentInList
    //    {
    //        VolunteerId = doAssignment.VolunteerId, // The ID of the volunteer
    //        VolunteerName = doVolunteer.FullName, // The name of the volunteer (helper method can be used)
    //        StartTime = doAssignment.TimeStart, // The time the volunteer started handling the call
    //        EndTime = doAssignment.TimeEnd, // The time the handling of the call was completed
    //        CompletionType = doAssignment.TypeEndTreat.HasValue
    //        ? (BO.AssignmentCompletionType?)doAssignment.TypeEndTreat.Value
    //        : null // Completion status (nullable)
    //    };
    //}
    public static BO.CallAssignmentInList GetCallAssignInList(int Id)
    {
        DO.Volunteer? doVolunteer;
        lock (AdminManager.BlMutex)
           doVolunteer = _dal.Volunteer.Read(Id) ?? throw new BlDoesNotExistException("error id");

        // Find the appropriate CALL and Assignment by volunteer ID
        Assignment? doAssignment;
        lock (AdminManager.BlMutex)

             doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == Id).FirstOrDefault();

        if (doAssignment == null)
        {
            return null;
        }

        return new BO.CallAssignmentInList
        {
            VolunteerId = doAssignment.VolunteerId, // The ID of the volunteer
            VolunteerName = doVolunteer.FullName, // The name of the volunteer (helper method can be used)
            StartTime = doAssignment.TimeStart, // The time the volunteer started handling the call
            EndTime = doAssignment.TimeEnd, // The time the handling of the call was completed
            CompletionType = (BO.AssignmentCompletionType?)doAssignment.TypeEndTreat // Completion status (nullable)
        };

    }

    //}
    //public static BO.CallAssignmentInList GetCallAssignInList(int Id)
    //{
    //    // קריאה למתנדב לפי מזהה
    //    DO.Volunteer? doVolunteer = _dal.Volunteer.Read(Id) ?? throw new BlDoesNotExistException("Volunteer with the specified ID does not exist.");

    //    // חיפוש השמה מתאימה לפי מזהה מתנדב
    //    var doAssignment = _dal.Assignment.ReadAll()
    //        .Where(a => a.VolunteerId == Id && a.TimeEnd == null)
    //        .FirstOrDefault();

    //    if (doAssignment == null)
    //    {
    //        throw new BlDoesNotExistException("No active assignment found for the specified volunteer.");
    //    }

    //    // חיפוש שיחה מתאימה לפי מזהה השיחה מההשמה
    //    var doCall = _dal.Call.ReadAll()
    //        .Where(c => c.Id == doAssignment.CallId)
    //        .FirstOrDefault();

    //    if (doCall == null)
    //    {
    //        throw new BlDoesNotExistException("No call found for the specified assignment.");
    //    }

    //    // החזרת האובייקט עם הנתונים
    //    return new BO.CallAssignmentInList
    //    {
    //        VolunteerId = doAssignment.VolunteerId, // מזהה המתנדב
    //        VolunteerName = doVolunteer.FullName, // שם המתנדב
    //        StartTime = doAssignment.TimeStart, // זמן התחלת הטיפול
    //        EndTime = doAssignment.TimeEnd, // זמן סיום הטיפול
    //        CompletionType = doAssignment.TypeEndTreat.HasValue
    //            ? (BO.AssignmentCompletionType?)doAssignment.TypeEndTreat.Value
    //            : null // סטטוס השלמת הטיפול (nullable)
    //    };
    //}


    //public static List<BO.CallAssignmentInList> GetCallAssignInList(int volunteerId)
    //{
    //    // בדיקת קיום מתנדב
    //    DO.Volunteer? doVolunteer = _dal.Volunteer.Read(volunteerId)
    //        ?? throw new BlDoesNotExistException($"Volunteer with ID {volunteerId} does not exist");

    //    // הבאת כל ההקצאות הפתוחות של המתנדב
    //    var doAssignments = _dal.Assignment.ReadAll()
    //        .Where(a => a.VolunteerId == volunteerId && a.TimeEnd == null)
    //        .ToList();

    //    // אם אין הקצאות פתוחות, מחזיר רשימה ריקה
    //    if (!doAssignments.Any())
    //    {
    //        return new List<BO.CallAssignmentInList>();
    //    }

    //    // ממפה את ההקצאות לאובייקטי BO
    //    return doAssignments.Select(a => new BO.CallAssignmentInList
    //    {
    //        VolunteerId = a.VolunteerId,
    //        VolunteerName = doVolunteer.FullName, // שם המתנדב
    //        StartTime = a.TimeStart,
    //        EndTime = a.TimeEnd,
    //        CompletionType = a.TypeEndTreat.HasValue
    //            ? (BO.AssignmentCompletionType?)a.TypeEndTreat.Value
    //            : null
    //    }).ToList();
    //}
    private static readonly object LockObject = new object();  // אובייקט נעילה לסנכרון הגישה

    public static List<BO.CallAssignmentInList> GetCallAssignmentsForCall(int callId)
    {

        // For the CallAssignments field in the GetViewingCall function

        // Search for all assignments related to the given call
        List<Assignment>? doAssignments;
        lock (AdminManager.BlMutex)

             doAssignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId).ToList();

        // If no assignments are found, return null
        if (!doAssignments.Any() || doAssignments == null)
        {
            return null; // No assignments found
        }
        List<CallAssignmentInList> callAssignInList;
        lock (AdminManager.BlMutex)
            // Use LINQ to convert the assignments into BO.CallAssignInList using the GetCallAssignInList function
             callAssignInList = (from doAssignment in doAssignments
                                let doVolunteer = _dal.Volunteer.Read(doAssignment.VolunteerId)
                                where doVolunteer != null
                                select GetCallAssignInList(doAssignment.VolunteerId)) // Calls the conversion function
                                .ToList();

        // Return the complete list
        return callAssignInList;

    }

    // GetCallInList
    public static BO.CallInList GetCallInList(int Id)
    {

        // Find the appropriate CALL  and  Assignment by volunteer ID
        Assignment? doAssignment;
        lock (AdminManager.BlMutex)
             doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == Id && a.TimeEnd == null).FirstOrDefault();
        DO.Call doCall;
        lock (AdminManager.BlMutex)
             doCall = _dal.Call.ReadAll().Where(c => c.Id == /*doAssignment*/Id).FirstOrDefault();
        IEnumerable<Assignment> GetTotalAssignmentsForCall;
        lock (AdminManager.BlMutex)
             GetTotalAssignmentsForCall = _dal.Assignment.ReadAll().Where(a => a.Id == Id);

        return new BO.CallInList
        {
            Id = doAssignment?.Id,
            CallId = doAssignment.CallId,
            Type = (BO.CallType)doCall.Type, // Enum conversion
            OpeningTime = doCall.TimeOpened, // The time when the call was opened

            TimeToFinish = CalculateTimeRemaining(doCall.MaxTimeToClose), // Time remaining until the maximum completion time
            LastVolunteerName = GetLatestVolunteerNameForCall(doAssignment.VolunteerId), // The name of the volunteer assigned to the call

            TreatmentDuration = CalculateCompletionTime(doAssignment.Id), // Total time taken to complete the call
            Status = CalculateCallStatus(doCall), // Current status of the call
            TotalAssignments = GetTotalAssignmentsForCall.Count(a => a.CallId == doAssignment.CallId) // Total number of assignments for the call
        };

    }

    public static BO.Call GetViewingCall(int CallId)
    {
        DO.Assignment doAssignment;
        lock (AdminManager.BlMutex)
             doAssignment = _dal.Assignment.ReadAll(a => a.CallId == CallId).FirstOrDefault();
        DO.Call? doCall;
        lock (AdminManager.BlMutex)
             doCall = _dal.Call.ReadAll().Where(c => c.Id == CallId).FirstOrDefault();

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
            Status = CalculateCallStatus(doCall), // Current status of the call
            CallAssignments = CallManager.GetCallAssignmentsForCall(doCall.Id)
        };

    }

    // Convert 
    //public static DO.Call BOConvertDO_Call(BO.Call BOCall)
    //{
    //    if (BOCall == null)
    //    {
    //        throw new ArgumentNullException(nameof(BOCall), "The provided BO.Call object cannot be null.");
    //    }

    //    // Convert BO.Call to DO.Call
    //    var DOCall = new DO.Call
    //    {
    //        Id = BOCall.Id,
    //        Type = (DO.CallType)Enum.Parse(typeof(DO.CallType), BOCall.Type.ToString()), // Use Enum.Parse if CallType is an Enum
    //        Description = BOCall.Description,
    //        FullAddress = BOCall.FullAddress,
    //        Latitude = (BOCall.Latitude ?? 0), // Convert to 0 if Latitude is null
    //        Longitude = (BOCall.Longitude ?? 0), // Convert to 0 if Longitude is null
    //        TimeOpened = BOCall.OpenTime,
    //        MaxTimeToClose = BOCall.MaxEndTime
    //    };

    //    return DOCall;
    //}

    public static TimeSpan? CalculateTimeRemaining(DateTime? maxEndTime)
    {
        if (maxEndTime == null)
            return null;

        return maxEndTime - AdminManager.Now;
    }

    public static string? GetLatestVolunteerNameForCall(int callId)
    {

        // Retrieve all assignments related to the call
        IEnumerable<Assignment>? assignments;
        lock (AdminManager.BlMutex)
             assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);

        if (!assignments.Any())
            return null;

        // Find the assignment with the latest entry time closest to the current system time
        var latestAssignment = assignments
            .OrderByDescending(a => a.TypeEndTreat)
            .FirstOrDefault();

        if (latestAssignment == null || latestAssignment.VolunteerId == null)
            return null;

        // Retrieve the volunteer associated with the assignment
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
             volunteer = _dal.Volunteer.Read((int)latestAssignment.VolunteerId);
        return volunteer?.FullName;

    }

    public static TimeSpan? CalculateCompletionTime(int callId)
    {

        // Retrieve all assignments related to the call
        IEnumerable<Assignment> assignments;
        lock (AdminManager.BlMutex)
             assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);

        // Check if the call has been completed
        var completedAssignment = assignments
            .Where(a => a.TimeEnd != null && a.TypeEndTreat != null)
            .OrderByDescending(a => a.TypeEndTreat)
            .FirstOrDefault();

        if (completedAssignment == null)
            return null;

        // Calculate the time taken to complete the call
        return completedAssignment.TimeStart - completedAssignment.TimeEnd;

    }

    internal static BO.CallInList ConvertDOCallToBOCallInList(DO.Call doCall)
    {


        if (doCall == null)
        {
            throw new ArgumentNullException(nameof(doCall), "The provided call is null.");
        }

        // קבל את כל ההקצותות עבור הקריאה
        IEnumerable<Assignment> assignmentsForCall;
        lock (AdminManager.BlMutex)
             assignmentsForCall = _dal.Assignment.ReadAll(a => a.CallId == doCall.Id) ?? new List<DO.Assignment>();

        // קבל את ההקצאה האחרונה על פי זמן ההתחלה
        Assignment? lastAssignmentsForCall;
        lock (AdminManager.BlMutex)
             lastAssignmentsForCall = assignmentsForCall.OrderByDescending(item => item.TimeStart).FirstOrDefault();

        // קבע את שם המתנדב האחרון אם יש
        var volunteerName = lastAssignmentsForCall != null
                             ? _dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)?.FullName
                             : null;

        // חישוב משך הזמן של טיפול אם יש
        var treatmentDuration = lastAssignmentsForCall != null && lastAssignmentsForCall.TimeEnd != null
                               ? lastAssignmentsForCall.TimeEnd - lastAssignmentsForCall.TimeStart
                               : null;
        BO.CallStatus status = GetCallStatus(doCall);

        // החזר את האובייקט CallInList
        return new BO.CallInList
        {
            Id = lastAssignmentsForCall?.Id,  // אם אין הקצאה אחרונה, id יהיה null
            CallId = doCall.Id,
            Type = (BO.CallType)doCall.Type,
            OpeningTime = doCall.TimeOpened,
            TimeToFinish = (doCall.MaxTimeToClose != null && doCall.MaxTimeToClose >= _dal.Config.Clock) ? doCall.MaxTimeToClose - _dal.Config.Clock : null,
            LastVolunteerName = (lastAssignmentsForCall != null)
                ? _dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)?.FullName
                : null,
            TreatmentDuration = status == BO.CallStatus.Closed ? lastAssignmentsForCall.TimeEnd - doCall.TimeOpened : null,

            Status = CallManager.GetCallStatus(doCall),
            TotalAssignments = (assignmentsForCall == null) ? 0 : assignmentsForCall.Count()
        };

    }

    //internal static BO.ClosedCallInList ConvertDOCallToBOCloseCallInList(DO.Call doCall, CallAssignmentInList lastAssignment)
    //{
    //    return new BO.ClosedCallInList
    //    {
    //        Id = doCall.Id,
    //        CallType = (BO.CallType)doCall.Type,
    //        FullAddress = doCall.FullAddress,
    //        OpeningTime = doCall.TimeOpened,
    //        EntryTime = lastAssignment.StartTime,
    //        CompletionTime = lastAssignment.EndTime,
    //        CompletionType = lastAssignment.CompletionType
    //    };
    //}

    //internal static BO.OpenCallInList ConvertDOCallToBOOpenCallInList(DO.Call doCall, int id)
    //{
    //    var vol = _dal.Volunteer.Read(id);
    //    double idLat = vol.Latitude ?? 0;
    //    double idLon = vol.Longitude ?? 0;
    //    return new BO.OpenCallInList
    //    {
    //        Id = doCall.Id,
    //        CallType = (BO.CallType)doCall.Type,
    //        Description = doCall.Description,
    //        FullAddress = doCall.FullAddress,
    //        OpeningTime = doCall.TimeOpened,
    //        MaxCompletionTime = doCall.MaxTimeToClose,
    //        DistanceFromVolunteer = VolunteerManager.CalculateDistance(doCall.Latitude, doCall.Longitude, idLat, idLon),
    //    };
    //}

    public static bool IsInRisk(DO.Call call)
    {
        // Check if the time left to close the call is within the configured risk range
        // or if the time left is less than or equal to 4 hours (you can adjust this condition as needed).
        lock (AdminManager.BlMutex)
            return (call.MaxTimeToClose - _dal.Config.Clock <= _dal.Config.RiskRange) ||
               (call.MaxTimeToClose - _dal.Config.Clock <= TimeSpan.FromHours(12));
    }

    internal static void UpdateExpired()
    {

        // Step 1: Retrieves all calls where the MaxTimeToEnd has passed.
        IEnumerable<DO.Call>? expiredCalls;
        lock (AdminManager.BlMutex)
             expiredCalls = _dal.Call.ReadAll(c => c.MaxTimeToClose < AdminManager.Now);

        // Step 2: Checks for calls without assignments and creates a new assignment with the expired status.
        foreach (var call in expiredCalls)
        {
            bool hasAssignment;
            lock (AdminManager.BlMutex)
                 hasAssignment = _dal.Assignment
                .ReadAll(a => a.CallId == call.Id)
                .Any();

            if (!hasAssignment)  // If there is no assignment for the call yet
            {
                var newAssignment = new DO.Assignment(
                    Id: 0,  // Creates a new ID for the assignment.
                    CallId: call.Id,
                    VolunteerId: 0,  // Volunteer ID is set to 0 (no assignment).
                    TimeStart: AdminManager.Now,  // Sets the start time to the current time.
                    TypeEndTreat: DO.TypeEnd.ExpiredCancel  // Sets the end type to expired.
                );
                lock (AdminManager.BlMutex)
                    _dal.Assignment.Create(newAssignment);  // Creates the new assignment.
            }
        }

        // Step 3: Updates assignments with null TimeEnd for calls that are expired.
        lock (AdminManager.BlMutex)
            foreach (var assignment in _dal.Assignment.ReadAll(a => a.TimeEnd == null))
        {
            var call = expiredCalls.FirstOrDefault(c => c.Id == assignment.CallId);
            if (call != null)  // If the call is still marked as expired
            {
                // Creating a new updated assignment
                var updatedAssignment = assignment with
                {

                    TimeEnd = AdminManager.Now,  // Sets the actual end time to the current time.
                    TypeEndTreat = DO.TypeEnd.ExpiredCancel  // Marks the end type as expired.
                };

                    // Now we update the assignment with the new updatedAssignment object
                    lock (AdminManager.BlMutex)
                         _dal.Assignment.Update(updatedAssignment);  // Updates the assignment with the new values.
                Observers.NotifyItemUpdated(updatedAssignment.Id); //stage 5
            }

        }
    }
    //public static void AddCallInternal1(BO.Call boCall)
    //{
    //    // יצירת משימה אסינכרונית
    //    var task = Task.Run(async () =>
    //    {
    //        return await VolunteerManager.GetCoordinatesFromAddressAsync(boCall.FullAddress);
    //    });

    //    // מחכים להשלמת המשימה
    //    double[] coordinate = task.Result;
    //    double latitude = coordinate[0];
    //    double longitude = coordinate[1];
    //    boCall.Latitude = latitude;
    //    boCall.Longitude = longitude;

    //    CallManager.IsLogicCall(boCall);

    //    DO.Call doCall = new
    //    (
    //        boCall.Id,
    //        (DO.CallType)boCall.Type,
    //        boCall.Description,
    //        boCall.FullAddress,
    //        latitude,
    //        longitude,
    //        boCall.OpenTime,
    //        boCall.MaxEndTime
    //    );

    //    lock (AdminManager.BlMutex) // שלב 7
    //    {
    //        _dal.Call.Create(doCall);
    //    }
    //}
    public static void AddCallInternal1(BO.Call boCall)      ///////// לבדוק אם זה טוב 

    {
        // יצירת אובייקט השיחה בלי לחשב קואורדינטות עדיין
        DO.Call doCall = new
        (
            boCall.Id,
            (DO.CallType)boCall.Type,
            boCall.Description,
            boCall.FullAddress,
            0, // ערכים זמניים עד לעדכון הקואורדינטות
            0,
            boCall.OpenTime,
            boCall.MaxEndTime
        );

        lock (AdminManager.BlMutex) // שלב 7
        {
            _dal.Call.Create(doCall);
        }

        // הרצת חישוב הקואורדינטות ברקע ללא חסימה
        _ = Task.Run(async () =>
        {
            double[]? coordinate = await VolunteerManager.GetCoordinatesFromAddressAsync(boCall.FullAddress);
            if (coordinate is not null)
            {
                doCall = doCall with { Latitude = coordinate[0], Longitude = coordinate[1] };

                lock (AdminManager.BlMutex)
                    _dal.Call.Update(doCall);
            }
        });

        // המשך הביצוע בלי לחכות לתוצאה
    }


    public static BO.Call ReadInternal(int callId)
    {
        // הגדרת פונקציה לסינון משימות שקשורות לשיחה המבוקשת
        Func<DO.Assignment, bool> func = item => item.CallId == callId;
        IEnumerable<DO.Assignment> dataOfAssignments;

        lock (AdminManager.BlMutex) // שלב 7
            dataOfAssignments = _dal.Assignment.ReadAll(func); // שליפת המשימות הרלוונטיות

        // שליפת השיחה משכבת הנתונים או זריקת חריגה אם היא לא נמצאת
        DO.Call? doCall;
        lock (AdminManager.BlMutex) // שלב 7
            doCall = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"שיחה עם מזהה {callId} לא קיימת");

        // המרת האובייקט מסוג DO.Call לאובייקט עסקי מסוג BO.Call
        return new BO.Call
        {
            Id = callId,
            Type = (BO.CallType)doCall.Type, // המרת סוג השיחה
            Description = doCall.Description, // המרת התיאור
            FullAddress = doCall.FullAddress, // המרת הכתובת המלאה
            Latitude = doCall.Latitude, // המרת קו הרוחב
            Longitude = doCall.Longitude, // המרת קו האורך
            OpenTime = doCall.TimeOpened, // המרת זמן הפתיחה
            MaxEndTime = doCall.MaxTimeToClose, // המרת הזמן המקסימלי לסגירה
            Status = CallManager.GetCallStatus(doCall), // קביעת הסטטוס של השיחה
            CallAssignments = dataOfAssignments.Any()
                ? dataOfAssignments.Select(assign => new BO.CallAssignmentInList
                {
                    VolunteerId = assign.VolunteerId, // המרת מזהה המתנדב
                    VolunteerName = _dal.Volunteer.Read(assign.VolunteerId)?.FullName ?? "Unknown Volunteer", // המרת שם המתנדב
                    StartTime = assign.TimeStart, // המרת זמן התחלת המשימה
                    EndTime = assign.TimeEnd, // המרת זמן סיום המשימה
                    CompletionType = assign.TypeEndTreat.HasValue
                        ? (BO.AssignmentCompletionType)assign.TypeEndTreat.Value
                        : null, // המרת סוג הסיום, טיפול בערכים ריקים
                }).ToList()
                : null,
        };
    }
    public static IEnumerable<BO.CallInList> GetCallInListsInternal(BO.CallInListField? filter, object? obj, BO.CallInListField? sortBy)
    {
        // Retrieve all calls from the database, or throw an exception if none exist.
        IEnumerable<DO.Call> calls;

        lock (AdminManager.BlMutex) // stage 7
        {
            calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are no calls in the database");
        }

        // Convert all DO calls to BO calls in list.
        IEnumerable<BO.CallInList> boCallsInList = calls.Select(call => CallManager.ConvertDOCallToBOCallInList(call)).ToList();

        // Apply filter if specified.
        if (filter != null && obj != null)
        {
            switch (filter)
            {
                case BO.CallInListField.Id:
                    boCallsInList = boCallsInList.Where(item => item.Id == (int)obj);
                    break;
                case BO.CallInListField.CallId:
                    boCallsInList = boCallsInList.Where(item => item.CallId == (int)obj);
                    break;
                case BO.CallInListField.Type:
                    boCallsInList = boCallsInList.Where(item => item.Type == (BO.CallType)obj);
                    break;
                case BO.CallInListField.OpeningTime:
                    boCallsInList = boCallsInList.Where(item => item.OpeningTime == (DateTime)obj);
                    break;
                case BO.CallInListField.TimeToFinish:
                    boCallsInList = boCallsInList.Where(item => item.TimeToFinish == (TimeSpan)obj);
                    break;
                case BO.CallInListField.LastVolunteerName:
                    boCallsInList = boCallsInList.Where(item => item.LastVolunteerName == (string)obj);
                    break;
                case BO.CallInListField.TreatmentDuration:
                    boCallsInList = boCallsInList.Where(item => item.TreatmentDuration == (TimeSpan)obj);
                    break;
                case BO.CallInListField.Status:
                    if ((BO.CallStatus)obj == BO.CallStatus.None)
                        break;
                    boCallsInList = boCallsInList.Where(item => item.Status == (BO.CallStatus)obj);
                    break;
                case BO.CallInListField.TotalAssignments:
                    boCallsInList = boCallsInList.Where(item => item.TotalAssignments == (int)obj);
                    break;
            }
        }

        // Default sort by CallId if no sorting is specified.
        if (sortBy == null)
            sortBy = BO.CallInListField.CallId;

        // Apply sorting based on the specified field.
        switch (sortBy)
        {
            case BO.CallInListField.Id:
                boCallsInList = boCallsInList.OrderBy(item => item.Id.HasValue ? 0 : 1)
                                             .ThenBy(item => item.Id)
                                             .ToList();
                break;
            case BO.CallInListField.CallId:
                boCallsInList = boCallsInList.OrderBy(item => item.CallId).ToList();
                break;
            case BO.CallInListField.Type:
                boCallsInList = boCallsInList.OrderBy(item => item.Type).ToList();
                break;
            case BO.CallInListField.OpeningTime:
                boCallsInList = boCallsInList.OrderBy(item => item.OpeningTime).ToList();
                break;
            case BO.CallInListField.TimeToFinish:
                boCallsInList = boCallsInList.OrderBy(item => item.TimeToFinish).ToList();
                break;
            case BO.CallInListField.LastVolunteerName:
                boCallsInList = boCallsInList.OrderBy(item => item.LastVolunteerName).ToList();
                break;
            case BO.CallInListField.TreatmentDuration:
                boCallsInList = boCallsInList.OrderBy(item => item.TreatmentDuration).ToList();
                break;
            case BO.CallInListField.Status:
                boCallsInList = boCallsInList.OrderBy(item => item.Status).ToList();
                break;
            case BO.CallInListField.TotalAssignments:
                boCallsInList = boCallsInList.OrderBy(item => item.TotalAssignments).ToList();
                break;
        }

        // Return the filtered and sorted list of calls.
        return boCallsInList;
    }
    public static void UpdateTreatmentCancellationInternal(int volunteerId, int assignmentId)
    {

        DO.Assignment assigmnetToCancel;

        // Lock to synchronize DAL calls (stage 7)
        lock (AdminManager.BlMutex)
        {
            assigmnetToCancel = _dal.Assignment.Read(assignmentId)
                                ?? throw new BO.BlDeleteNotPossibleException("There is no assignment with this ID");
        }

        bool isManager = false;

        // Check if the volunteer is either the one assigned or a manager
        if (assigmnetToCancel.VolunteerId != volunteerId)
        {
            if (_dal.Volunteer.Read(volunteerId).Job == DO.Role.Manager)
                isManager = true;
            else
                throw new BO.BlDeleteNotPossibleException("The volunteer is not a manager or not assigned to this call");
        }

        // Validate if the assignment can be canceled
        if (assigmnetToCancel.TypeEndTreat != null ||
            (_dal.Call.Read(assigmnetToCancel.CallId).MaxTimeToClose > AdminManager.Now) ||
            assigmnetToCancel.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assignment is not open or has expired");

        // Prepare the updated assignment object
        DO.Assignment assigmnetToUP = new DO.Assignment
        {
            Id = assigmnetToCancel.Id,
            CallId = assigmnetToCancel.CallId,
            VolunteerId = assigmnetToCancel.VolunteerId,
            TimeStart = assigmnetToCancel.TimeStart,
            TimeEnd = AdminManager.Now,
            TypeEndTreat = isManager ? DO.TypeEnd.ManagerCancel : DO.TypeEnd.SelfCancel,
        };

        // Update the assignment in the DAL within a lock
        lock (AdminManager.BlMutex)
        {
            _dal.Assignment.Update(assigmnetToUP);
        }

        // Notify observers about the update (outside the lock)
        CallManager.Observers.NotifyItemUpdated(assignmentId); // Notify item update (stage 5)
        CallManager.Observers.NotifyListUpdated(); // Notify list update (stage 5)
    }
    public static IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteerInternal(int volunteerId, BO.CallType? type = null, BO.OpenCallInListField? sortField = null)
    {
        DO.Volunteer boVolunteer;
        lock (AdminManager.BlMutex) // stage 7
            boVolunteer = _dal.Volunteer.Read(volunteerId);
        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        IEnumerable<BO.OpenCallInList>? openCallInLists = new List<BO.OpenCallInList>();

        openCallInLists = from item in previousCalls
                          let DataCall = ReadInternal(item.Id)
                          where DataCall.Status == BO.CallStatus.Open || DataCall.Status == BO.CallStatus.OpenRisk
                          //let lastAssugnment = DataCall.CallAssignments.OrderBy(c => c.StartTime).Last()
                          select /*CallManager.ConvertDOCallToBOOpenCallInList(item, volunteerId);*/
                                  new BO.OpenCallInList
                                  {
                                      Id = DataCall.Id,
                                      CallType = (BO.CallType)DataCall.Type,
                                      Description = DataCall.Description,
                                      FullAddress = DataCall.FullAddress,
                                      OpeningTime = DataCall.OpenTime,
                                      MaxCompletionTime = DataCall.MaxEndTime,
                                      DistanceFromVolunteer = VolunteerManager.CalculateDistance((double)DataCall.Latitude, (double)DataCall.Longitude, (double)boVolunteer.Latitude, (double)boVolunteer.Longitude)
                                  };
        ;

        //openCallInLists = openCallInLists.Where(call => call.Id == volunteerId);
        if (type != null)
        {
            openCallInLists.Where(c => c.CallType == type).Select(c => c);
        }
        if (sortField == null)
        {
            sortField = BO.OpenCallInListField.Id;
        }
        switch (sortField)
        {
            case BO.OpenCallInListField.Id:
                openCallInLists.OrderBy(item => item.Id);
                break;
            case BO.OpenCallInListField.CallType:
                openCallInLists.OrderBy(item => item.CallType);
                break;
            case BO.OpenCallInListField.FullAddress:
                openCallInLists.OrderBy(item => item.FullAddress);
                break;
            case BO.OpenCallInListField.OpeningTime:
                openCallInLists.OrderBy(item => item.OpeningTime);
                break;
            case BO.OpenCallInListField.MaxCompletionTime:
                openCallInLists.OrderBy(item => item.MaxCompletionTime);
                break;
            case BO.OpenCallInListField.DistanceFromVolunteer:
                openCallInLists.OrderBy(item => item.DistanceFromVolunteer);
                break;

        }

        return openCallInLists;
    }
    public static void CancelTreatInternal(int idVol, int idAssig)
    {
        DO.Assignment assigmnetToCancel = _dal.Assignment.Read(idAssig)
            ?? throw new BO.BlDeleteNotPossibleException("there is no assignment with this ID");
        bool ismanager = false;

        if (assigmnetToCancel.VolunteerId != idVol)
        {
            if (_dal.Volunteer.Read(idVol).Job == DO.Role.Manager)
                ismanager = true;
            else
                throw new BO.BlDeleteNotPossibleException("the volunteer is not manager or not in this call");
        }

        if (assigmnetToCancel.TypeEndTreat != null || assigmnetToCancel.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assignment not open or expired");

        DO.Assignment assigmnetToUP = new DO.Assignment
        {
            Id = assigmnetToCancel.Id,
            CallId = assigmnetToCancel.CallId,
            VolunteerId = assigmnetToCancel.VolunteerId,
            TimeStart = assigmnetToCancel.TimeStart,
            TimeEnd = AdminManager.Now,
            TypeEndTreat = ismanager ? DO.TypeEnd.ManagerCancel : DO.TypeEnd.SelfCancel,
        };

        // Update the assignment in the DAL
        _dal.Assignment.Update(assigmnetToUP);

        // Notifications outside the lock
        CallManager.Observers.NotifyItemUpdated(idAssig); // stage 5
        CallManager.Observers.NotifyListUpdated();       // stage 5
        VolunteerManager.Observers.NotifyListUpdated();
        VolunteerManager.Observers.NotifyItemUpdated(idVol);
    }
    public static IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteerInternal(int id, BO.CallType? type, BO.ClosedCallInListField? sortBy)
    {
        IEnumerable<BO.ClosedCallInList> filteredCalls;
        IEnumerable<DO.Call> allCalls = _dal.Call.ReadAll();
        IEnumerable<DO.Assignment> allAssignments = _dal.Assignment.ReadAll();

        // Filter by volunteer ID and closed status (calls that have an end treatment type)
        filteredCalls = from call in allCalls
                        join assignment in allAssignments
                        on call.Id equals assignment.CallId
                        where assignment.VolunteerId == id && assignment.TypeEndTreat != null
                        select new BO.ClosedCallInList
                        {
                            Id = call.Id,
                            CallType = (BO.CallType)call.Type,
                            FullAddress = call.FullAddress,
                            OpeningTime = call.TimeOpened,
                            EntryTime = assignment.TimeStart,
                            CompletionTime = assignment.TimeEnd,
                            CompletionType = (BO.AssignmentCompletionType)assignment.TypeEndTreat
                        };

        // Filter by call type if provided
        if (type.HasValue)
        {
            filteredCalls = filteredCalls.Where(c => c.CallType == type.Value);
        }

        // Sort by the requested field or by default (call ID)
        if (sortBy.HasValue)
        {
            filteredCalls = sortBy.Value switch
            {
                BO.ClosedCallInListField.Id => filteredCalls.OrderBy(c => c.Id),
                BO.ClosedCallInListField.CallType => filteredCalls.OrderBy(c => c.CallType),
                BO.ClosedCallInListField.FullAddress => filteredCalls.OrderBy(c => c.FullAddress),
                BO.ClosedCallInListField.OpeningTime => filteredCalls.OrderBy(c => c.OpeningTime),
                BO.ClosedCallInListField.EntryTime => filteredCalls.OrderBy(c => c.EntryTime),
                BO.ClosedCallInListField.CompletionTime => filteredCalls.OrderBy(c => c.CompletionTime),
                BO.ClosedCallInListField.CompletionType => filteredCalls.OrderBy(c => c.CompletionType),
                _ => filteredCalls.OrderBy(c => c.Id)
            };
        }
        else
        {
            filteredCalls = filteredCalls.OrderBy(c => c.Id);
        }

        return filteredCalls;
    }
    public static void CloseTreatInternal(int idVol, int idAssig)
    {
        DO.Assignment assigmnetToClose = _dal.Assignment.Read(idAssig)
            ?? throw new BO.BlDeleteNotPossibleException("there is no assignment with this ID");

        if (assigmnetToClose.VolunteerId != idVol)
        {
            throw new BO.BlWrongInputException("the volunteer is not treat in this assignment");
        }

        if (assigmnetToClose.TypeEndTreat != null || assigmnetToClose.TimeEnd != null)
        {
            throw new BO.BlDeleteNotPossibleException("The assignment not open");
        }

        DO.Assignment assignmentToUP = new DO.Assignment
        {
            Id = assigmnetToClose.Id,
            CallId = assigmnetToClose.CallId,
            VolunteerId = assigmnetToClose.VolunteerId,
            TimeStart = assigmnetToClose.TimeStart,
            TimeEnd = AdminManager.Now,
            TypeEndTreat = DO.TypeEnd.Treated,
        };

        try
        {
            _dal.Assignment.Update(assignmentToUP);
        }
        catch (DO.DalAlreadyExistsException)
        {
            throw new BO.BlDeleteNotPossibleException("cannot update in DO");
        }
    }
    public static void UpdateInternal(BO.Call boCall)
    {
        // Convert the BO.Call object to a DO.Call object for data layer processing.
        DO.Call doCall = new
                (
                boCall.Id, // Call ID
                (DO.CallType)boCall.Type, // Convert call type from BO to DO
                boCall.Description, // Description of the call
                boCall.FullAddress, // Full address of the call
                boCall.Latitude ?? null, // Default latitude if null
                boCall.Longitude ?? null, // Default longitude if null
                boCall.OpenTime, // The time the call was opened
                boCall.MaxEndTime // The maximum time allowed for the call to close
                );
        try
        {
            // Update the call in the data layer.
            lock (AdminManager.BlMutex) // stage 7
                _dal.Call.Update(doCall);
            VolunteerManager.updateCoordinatesForCallAddressAsync(doCall);

            // Notify observers outside of the lock.
            CallManager.Observers.NotifyItemUpdated(doCall.Id); // stage 5
            CallManager.Observers.NotifyListUpdated(); // stage 5
        }
        catch (DO.DalDeletionImpossible ex)
        {
            // Handle exception if the call does not exist, wrapping it in a business logic exception.
            throw new BO.BlDoesNotExistException($"Call with ID={boCall.Id} does Not exist", ex);
        }
    }

    public static void ChooseCallForTreatInternal(int volunteerId, int callId)
    {
        DO.Volunteer vol;
        lock (AdminManager.BlMutex) // stage 7
            vol = _dal.Volunteer.Read(volunteerId)
                           ?? throw new BO.BlNullPropertyException($"There is no volunteer with this ID {volunteerId}");

        // Reads the call from the business logic or throws an exception if not found
        BO.Call bocall = ReadInternal(callId)
                         ?? throw new BO.BlNullPropertyException($"There is no call with this ID {callId}");

        // Ensures the call is not open or expired before assigning a volunteer
        if (bocall.Status != BO.CallStatus.Open || bocall.Status == BO.CallStatus.OpenRisk)
            throw new BO.BlAlreadyExistsException($"The call is open or expired. Call ID is={callId}");

        // Creates a new assignment record for the volunteer and call
        DO.Assignment assignmentToCreate = new DO.Assignment
        {
            Id = 0,
            CallId = callId,
            VolunteerId = volunteerId,
            TimeStart = AdminManager.Now, // Sets the start time for the assignment
            TimeEnd = null,               // No end time initially
            TypeEndTreat = null           // No completion type initially
        };

        try
        {
            lock (AdminManager.BlMutex)
            {// stage 7
                _dal.Assignment.Create(assignmentToCreate);
            } // Adds the assignment to the DAL
            CallManager.Observers.NotifyListUpdated(); //stage 5
            CallManager.Observers.NotifyItemUpdated(volunteerId);
            //VolunteerManager.Observers.NotifyListUpdated();
            //VolunteerManager.Observers.NotifyItemUpdated(assignmentToCreate.CallId);
        }
        catch (DO.DalDeletionImpossible)
        {
            throw new BO.BlAlreadyExistsException("Impossible to create assignment"); // Handles creation failure
        }
    }
    

}

