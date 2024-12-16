using BO;
using System.Text.RegularExpressions;

using DalApi;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    //internal static BO.CallInProgress GetCallInProgress(DO.Call doCall, DateTime entryTime, double distanceFromVolunteer)
    //{
    //    return new BO.CallInProgress
    //    {
    //        Id = doCall.Id,
    //        CallId = doCall.Id,
    //        CallType = (BO.CallType)doCall.Type,
    //        Description = doCall.Description,
    //        FullAddress = doCall.FullAddress,
    //        MaxCompletionTime = doCall.MaxTimeToClose,
    //        EntryTime = entryTime,
    //        DistanceFromVolunteer = distanceFromVolunteer,
    //        OpeningTime = doCall.TimeOpened
    //    };
    //}
    internal static BO.CallInProgress GetCallInProgress(DO.Call doCall, DateTime entryTime, double distanceFromVolunteer)
    {
        return new BO.CallInProgress
        {
            Id = doCall.Id,
            CallId = doCall.Id,
            CallType = (BO.CallType)doCall.Type,
            Description = doCall.Description,
            FullAddress = doCall.FullAddress,
            MaxCompletionTime = doCall.MaxTimeToClose,
            EntryTime = entryTime,
            DistanceFromVolunteer = distanceFromVolunteer,
            OpeningTime = doCall.TimeOpened
        };
    }
    internal static BO.ClosedCallInList GetClosedCallInList(DO.Call doCall, DO.Assignment? doAssignment)
    {
        return new BO.ClosedCallInList
        {
            Id = doCall.Id,
            CallType = (BO.CallType)doCall.Type,
            FullAddress = doCall.FullAddress,
            OpeningTime = doCall.TimeOpened,
            EntryTime = doAssignment?.TimeStart ?? throw new BO.BlWrongItemtException($"Assignment missing for Call ID {doCall.Id}"),
            CompletionTime = doAssignment?.TimeEnd,
            CompletionType = doAssignment?.TypeEndTreat.HasValue == true
             ? (BO.AssignmentCompletionType?)doAssignment.TypeEndTreat.Value
             : null

        };
    }


    internal static CallAssignmentInList GetCallAssignmentInList(DO.Assignment doAssignment, string volunteerName)
    {
        return new CallAssignmentInList
        {
            VolunteerId = doAssignment.VolunteerId,
            VolunteerName = volunteerName,
            StartTime = doAssignment.TimeStart,
            EndTime = doAssignment.TimeEnd,
            CompletionType = doAssignment.TypeEndTreat.HasValue
                ? (BO.AssignmentCompletionType?)doAssignment.TypeEndTreat.Value
                : null
        };
    }

    internal static BO.OpenCallInList GetOpenCallInList(DO.Call doCall, double distanceFromVolunteer)
    {
        return new BO.OpenCallInList
        {
            Id = doCall.Id,
            CallType = (BO.CallType)doCall.Type,
            Description = doCall.Description,
            FullAddress = doCall.FullAddress,
            OpeningTime = doCall.TimeOpened,
            MaxCompletionTime = doCall.MaxTimeToClose,
            DistanceFromVolunteer = distanceFromVolunteer
        };
    }
    private static void ValidateCallData(DO.Call doCall)
    {
        if (string.IsNullOrWhiteSpace(doCall.Description))
            throw new ArgumentException("Description cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(doCall.FullAddress))
            throw new ArgumentException("FullAddress cannot be null or empty.");

        if (doCall.Latitude < -90 || doCall.Latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(doCall.Latitude), "Latitude must be between -90 and 90.");

        if (doCall.Longitude < -180 || doCall.Longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(doCall.Longitude), "Longitude must be between -180 and 180.");

        if (doCall.MaxTimeToClose.HasValue && doCall.MaxTimeToClose <= doCall.TimeOpened)
            throw new ArgumentException("MaxTimeToClose must be later than TimeOpened.");
    }

    internal static BO.Call ConvertDOToBO(DO.Call doCall)
    {
        // קריאה לפונקציית העזר לבדיקת תקינות
        ValidateCallData(doCall);

        // המרה ל־BO
        return new BO.Call
        {
            Id = doCall.Id,
            Type = (BO.CallType)doCall.Type,
            Description = doCall.Description,
            FullAddress = doCall.FullAddress,
            Latitude = doCall.Latitude,
            Longitude = doCall.Longitude,
            OpenTime = doCall.TimeOpened,
            MaxEndTime = doCall.MaxTimeToClose
        };
    }

    internal static BO.CallStatus CalculateCallStatus(BO.Call call, DateTime currentTime, TimeSpan maxTimeToClose, TimeSpan riskTimeSpan)
    {
        if (call.OpenTime == default)
        {
            return BO.CallStatus.Expired;
        }

        DateTime expectedEndTime = call.OpenTime + maxTimeToClose;

        if (call.Status == BO.CallStatus.Open && (expectedEndTime - currentTime) <= riskTimeSpan)
        {
            return BO.CallStatus.OpenRisk;
        }

        if (currentTime > expectedEndTime)
        {
            return BO.CallStatus.Expired;
        }

        if (call.Status == BO.CallStatus.Closed)
        {
            return BO.CallStatus.Closed;
        }

        if (call.Status == BO.CallStatus.InProgress)
        {
            return BO.CallStatus.InProgress;
        }

        return BO.CallStatus.Open;
    }

    internal static double CalculateAirDistance(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        const double EarthRadiusKm = 6371.0;

        double lat1Rad = DegreesToRadians(latitude1);
        double lon1Rad = DegreesToRadians(longitude1);
        double lat2Rad = DegreesToRadians(latitude2);
        double lon2Rad = DegreesToRadians(longitude2);

        double dlat = lat2Rad - lat1Rad;
        double dlon = lon2Rad - lon1Rad;

        double a = Math.Pow(Math.Sin(dlat / 2), 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * Math.Pow(Math.Sin(dlon / 2), 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

}

