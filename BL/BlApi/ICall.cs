
namespace BlApi;

public interface ICall
{
    int[] GetCallCountsByStatus();
    IEnumerable<BO.CallInList> GetCallList(BO.CallField? filterField, object? filterValue, BO.CallField? sortField);
    // Method to request details of a specific call
    BO.Call GetCallDetails(int callId);

    // Method to update the details of a call
    void UpdateCallDetails(BO.Call call);

    // Method to delete a call
    void DeleteCall(int callId);

    // Method to add a new call
    void AddCall(BO.Call call);

    // Method to request a list of closed calls handled by a specific volunteer
    IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? type = null, BO.CallField? sortField = null);

    // Method to request a list of open calls available for a specific volunteer
    IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? type = null, BO.CallField? sortField = null);

    // Method to update the completion of treatment for a specific call
    void UpdateTreatmentCompletion(int volunteerId, int assignmentId);

    // Method to update the cancellation of treatment for a specific call
    void UpdateTreatmentCancellation(int volunteerId, int assignmentId);

    // Method to assign a call to a specific volunteer
    void AssignCallToVolunteer(int volunteerId, int callId);
}
