namespace BlApi;

public interface ICall  : IObservable
{
    /// <summary>
    /// Counts the number of calls in the system.
    /// </summary>
    /// <returns>An array of integers representing the count of calls, possibly categorized.</returns>
    public int[] CountCall();

    /// <summary>
    /// Retrieves a list of calls filtered and sorted based on provided parameters.
    /// </summary>
    /// <param name="filter">Optional filter field for calls (e.g., by call type, status, etc.).</param>
    /// <param name="obj">The object to filter the list by (e.g., a specific call type, date range).</param>
    /// <param name="sortBy">Optional sorting field to order the returned call list.</param>
    /// <returns>A collection of calls with relevant details based on the filter and sort options.</returns>
    public IEnumerable<BO.CallInList> GetCallInLists(BO.CallInListField? filter, object? obj, BO.CallInListField? sortBy);

    /// <summary>
    /// Retrieves the details of a specific call.
    /// </summary>
    /// <param name="callId">The ID of the call to retrieve.</param>
    /// <returns>A call object containing all the details of the specified call.</returns>
    public BO.Call Read(int callId);

    /// <summary>
    /// Updates an existing call's information.
    /// </summary>
    /// <param name="boCall">The updated call object containing new details.</param>
    public void Update(BO.Call boCall);

    /// <summary>
    /// Deletes a specific call by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call to delete.</param>
    public void DeleteCall(int callId);

    /// <summary>
    /// Adds a new call to the system.
    /// </summary>
    /// <param name="boCall">The call object containing details for the new call.</param>
    public void AddCall(BO.Call boCall);

    /// <summary>
    /// Retrieves a list of closed calls associated with a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to get closed calls for.</param>
    /// <param name="type">Optional filter by call type (e.g., FoodTransport, InventoryCheck).</param>
    /// <param name="sortField">Optional sorting field to order the returned closed call list.</param>
    /// <returns>A collection of closed calls for the specified volunteer.</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? type = null, BO.ClosedCallInListField? sortField = null);

    /// <summary>
    /// Retrieves a list of open calls assigned to a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to get open calls for.</param>
    /// <param name="type">Optional filter by call type.</param>
    /// <param name="sortField">Optional sorting field to order the returned open call list.</param>
    /// <returns>A collection of open calls for the specified volunteer.</returns>
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? type = null, BO.OpenCallInListField? sortField = null);

    /// <summary>
    /// Cancels an ongoing treatment assignment for a volunteer.
    /// </summary>
    /// <param name="idVol">The ID of the volunteer whose treatment is to be canceled.</param>
    /// <param name="idAssig">The ID of the assignment to cancel.</param>
    public void CancelTreat(int idVol, int idAssig);

    /// <summary>
    /// Closes an ongoing treatment assignment for a volunteer.
    /// </summary>
    /// <param name="idVol">The ID of the volunteer whose treatment is to be closed.</param>
    /// <param name="idAssig">The ID of the assignment to close.</param>
    public void CloseTreat(int idVol, int idAssig);

    /// <summary>
    /// Updates the status of a treatment cancellation for a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="assignmentId">The ID of the assignment whose treatment cancellation is to be updated.</param>
    public void UpdateTreatmentCancellation(int volunteerId, int assignmentId);

    /// <summary>
    /// Assigns a specific call to a volunteer for treatment.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to whom the call is being assigned.</param>
    /// <param name="callId">The ID of the call being assigned to the volunteer.</param>
    public void ChooseCallForTreat(int volunteerId, int callId);

    //public bool CanDelete(int callId);
}
