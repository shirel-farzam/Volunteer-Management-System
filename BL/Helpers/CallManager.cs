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
    internal static BO.CallInProgress GetCallInProgressReadonly(DO.Call doCall, DateTime entryTime, double distanceFromVolunteer)
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

}

