using BlApi;
using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;

using Dal;
using System.Linq;

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

        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignment by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.TimeEnd == null).FirstOrDefault();
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();



        if (Tools.IsAddressValid(doVolunteer.FullAddress).Result == false)// לא כתובת אמיתית 
        {
            throw new BlWrongInputException("Invalid address of Volunteer");// 
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

    internal static BO.CallStatus CalculateCallStatus(DO.Call doCall)
    {
        if (doCall.MaxTimeToClose < _dal.Config.Clock)
            return BO.CallStatus.Expired;
        var lastAssignment = _dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id).OrderByDescending(a => a.TypeEndTreat).FirstOrDefault();

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
        return BO.CallStatus.Closed;//default
    }



    
    public static BO.CallStatus GetCallStatus(DO.Call doCall)
    {
        if (doCall.MaxTimeToClose < _dal.Config.Clock)
            return BO.CallStatus.Expired;
        var lastAssignment = _dal.Assignment.ReadAll(ass => ass.CallId == doCall.Id).OrderByDescending(a => a.TimeStart).FirstOrDefault();

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
        return BO.CallStatus.Closed;//default
    }


    public static void MaxEndTimeCheck(DateTime? MaxEndTime, DateTime OpeningTime)
    {
        if (MaxEndTime < OpeningTime || MaxEndTime < AdminManager.Now)
        {
            throw new BlWrongInputException("The time entered according to the current time or opening time");
        }
    }


    // GetCallAssignInList
    public static BO.CallAssignmentInList GetCallAssignInList(int Id)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(Id) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignment by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == Id && a.TimeEnd == null).FirstOrDefault();
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

        return new BO.CallAssignmentInList
        {
            VolunteerId = doAssignment.VolunteerId, // The ID of the volunteer
            VolunteerName = doVolunteer.FullName, // The name of the volunteer (helper method can be used)
            StartTime = doAssignment.TimeStart, // The time the volunteer started handling the call
            EndTime = doAssignment.TimeEnd, // The time the handling of the call was completed
            CompletionType = doAssignment.TypeEndTreat.HasValue
            ? (BO.AssignmentCompletionType?)doAssignment.TypeEndTreat.Value 
            : null // Completion status (nullable)
        };

    }

    // GetCallInList
    public static BO.CallInList GetCallInList(int Id)
    {
        //DO.Volunteer? doVolunteer = _dal.Volunteer.Read(Id) ?? throw new BlDoesNotExistException("eroor id");// ז

        //Find the appropriate CALL  and  Assignment by volunteer ID
        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == Id && a.TimeEnd == null).FirstOrDefault();// לבדוק
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
            Status = CalculateCallStatus(doCall), // Current status of the call
            TotalAssignments = GetTotalAssignmentsForCall.Count(a => a.CallId == doAssignment.CallId) // Total number of assignments for the call

        };

    }

    // Convert 
    public static DO.Call BOConvertDO_Call(BO.Call BOCall)
    {
        if (BOCall == null)
        {
            throw new ArgumentNullException(nameof(BOCall), "The provided BO.Call object cannot be null.");
        }

        // המרה של BO.Call ל-DO.Call
        var DOCall = new DO.Call
        {
            Id = BOCall.Id,
            Type = (DO.CallType)BOCall.Type, 
            Description = BOCall.Description,
            FullAddress = BOCall.FullAddress,
            Latitude = BOCall.Latitude ?? 0, 
            Longitude = BOCall.Longitude ?? 0, 
            TimeOpened = BOCall.OpenTime,
            MaxTimeToClose = BOCall.MaxEndTime
        };

        return DOCall;
    }

    public static TimeSpan? CalculateTimeRemaining(DateTime? maxEndTime)
    {
        if (maxEndTime == null)
            return null;

        return maxEndTime - AdminManager.Now;
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
        return completedAssignment.TimeStart - completedAssignment.TimeEnd;
    }
    internal static BO.CallInList ConvertDOCallToBOCallInList(DO.Call doCall)
    {
  
        if (doCall == null)
        {
            throw new ArgumentNullException(nameof(doCall), "The provided call is null.");
        }

        // קבל את כל ההקצותות עבור הקריאה
        var assignmentsForCall = _dal.Assignment.ReadAll(a => a.CallId == doCall.Id) ?? new List<DO.Assignment>();

        // קבל את ההקצאה האחרונה על פי זמן ההתחלה
        var lastAssignmentsForCall = assignmentsForCall.OrderByDescending(item => item.TimeStart).FirstOrDefault();

        // קבע את שם המתנדב האחרון אם יש
        var volunteerName = lastAssignmentsForCall != null
                            ? _dal.Volunteer.Read(lastAssignmentsForCall.VolunteerId)?.FullName
                            : null;

        // חישוב משך הזמן של טיפול אם יש
        var treatmentDuration = lastAssignmentsForCall != null && lastAssignmentsForCall.TimeEnd != null
                                ? lastAssignmentsForCall.TimeEnd - lastAssignmentsForCall.TimeStart
                                : null;

        // החזר את האובייקט CallInList
        return new BO.CallInList
        {
            Id = lastAssignmentsForCall?.Id,  // אם אין הקצאה אחרונה, id יהיה null
            CallId = doCall.Id,
            Type = (BO.CallType)doCall.Type,
            OpeningTime = doCall.TimeOpened,
            TimeToFinish = doCall.MaxTimeToClose != null ? doCall.MaxTimeToClose - _dal.Config.Clock : null,
            LastVolunteerName = volunteerName,
            TreatmentDuration = treatmentDuration,
            Status = CallManager.GetCallStatus(doCall),
            TotalAssignments = assignmentsForCall.Count()
        };
    }


    internal static BO.ClosedCallInList ConvertDOCallToBOCloseCallInList(DO.Call doCall, CallAssignmentInList lastAssignment)
    {
        return new BO.ClosedCallInList
        {
            Id = doCall.Id,
            CallType = (BO.CallType)doCall.Type,
            FullAddress = doCall.FullAddress,
            OpeningTime = doCall.TimeOpened,
            EntryTime = lastAssignment.StartTime,
            CompletionTime = lastAssignment.EndTime,
            CompletionType = lastAssignment.CompletionType
        };
    }

    
    internal static BO.OpenCallInList ConvertDOCallToBOOpenCallInList(DO.Call doCall, int id)
    {
        var vol = _dal.Volunteer.Read(id);
        double idLat = vol.Latitude ?? 0;
        double idLon = vol.Longitude ?? 0;
        return new BO.OpenCallInList
        {
            Id = doCall.Id,
            CallType = (BO.CallType)doCall.Type,
            Description = doCall.Description,
            FullAddress = doCall.FullAddress,
            OpeningTime = doCall.TimeOpened,
            MaxCompletionTime = doCall.MaxTimeToClose,
            DistanceFromVolunteer = VolunteerManager.CalculateDistance(doCall.Latitude, doCall.Longitude, idLat, idLon),
        };
    }
    public static bool IsInRisk(DO.Call call)
    {
        // Check if the time left to close the call is within the configured risk range
        // or if the time left is less than or equal to 4 hours (you can adjust this condition as needed).
        return (call.MaxTimeToClose - _dal.Config.Clock <= _dal.Config.RiskRange) ||
               (call.MaxTimeToClose - _dal.Config.Clock <= TimeSpan.FromHours(12));
    }


    internal static void UpdateExpired()
    {
        // Step 1: Retrieves all calls where the MaxTimeToEnd has passed.
        var expiredCalls = _dal.Call.ReadAll(c => c.MaxTimeToClose < AdminManager.Now);

        // Step 2: Checks for calls without assignments and creates a new assignment with the expired status.
        foreach (var call in expiredCalls)
        {
            var hasAssignment = _dal.Assignment
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
                _dal.Assignment.Create(newAssignment);  // Creates the new assignment.
            }
        }

        // Step 3: Updates assignments with null TimeEnd for calls that are expired.
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
                _dal.Assignment.Update(updatedAssignment);  // Updates the assignment with the new values.
                Observers.NotifyItemUpdated(updatedAssignment.Id); //stage 5

            }
        }
    }


}

