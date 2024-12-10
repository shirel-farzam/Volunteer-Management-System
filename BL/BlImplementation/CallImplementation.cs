namespace BlImplementation;
using BlApi;
using BO;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddCall(Call call)
    {
        throw new NotImplementedException();
    }

    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public void DeleteCall(int callId)
    {
        throw new NotImplementedException();
    }

    public int[] GetCallCountsByStatus()
    {
        throw new NotImplementedException();
    }

    public Call GetCallDetails(int callId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CallInList> GetCallList(CallField? filterField, object? filterValue, CallField? sortField)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, CallType? type = null, CallField? sortField = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallType? type = null, CallField? sortField = null)
    {
        throw new NotImplementedException();
    }

    public void UpdateCallDetails(Call call)
    {
        throw new NotImplementedException();
    }

    public void UpdateTreatmentCancellation(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void UpdateTreatmentCompletion(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }
}
