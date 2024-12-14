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

    public int[] GetCallCountsByTypeEnd()
    {
        // Step 1: Retrieve calls from the data source
        var calls = _dal.Call.ReadAll();

        // Step 2: Group calls by TypeEnd
        var groupedCalls = calls
            .GroupBy(call => (int)call.TypeEnd) // Grouping by TypeEnd
            .ToDictionary(group => group.Key, group => group.Count()); // Creating a dictionary: TypeEnd -> Call count

        // Step 3: Calculate the maximum value of TypeEnd
        int maxTypeEnd = groupedCalls.Any() ? groupedCalls.Keys.Max() : 0;

        // Step 4: Create a results array with appropriate size
        int[] result = new int[maxTypeEnd + 1];

        // Step 5: Populate the array with the counts for each TypeEnd
        foreach (var kvp in groupedCalls)
        {
            result[kvp.Key] = kvp.Value;
        }

        // Step 6: Return the results array
        return result;
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

