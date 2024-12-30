namespace BlApi;
using BO;
public interface IVolunteer : IObservable
{
    /// <summary>
    /// Logs in a volunteer using their username and password.
    /// </summary>
    /// <param name="username">The volunteer's username.</param>
    /// <param name="password">The volunteer's password.</param>
    /// <returns>The role of the volunteer (e.g., admin, regular volunteer).</returns>
    public BO.Role Login(int username, string password);

    /// <summary>
    /// Retrieves a list of volunteers based on their active status and optional sorting criteria.
    /// </summary>
    /// <param name="isActive">Filter by active status: true for active, false for inactive, null for all.</param>
    /// <param name="sortField">Optional field to sort the volunteer list by.</param>
    /// <returns>An enumerable list of volunteers.</returns>
    public IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive, VolunteerInListField? sortField = null);

    /// <summary>
    /// Retrieves the detailed information of a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to retrieve.</param>
    /// <returns>The details of the specified volunteer.</returns>
    public BO.Volunteer RequestVolunteerDetails(int volunteerId);

    /// <summary>
    /// Updates the details of an existing volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to update.</param>
    /// <param name="boVolunteer">The updated volunteer object containing new details.</param>
    public void UpdateVolunteerDetails(int volunteerId, BO.Volunteer boVolunteer);

    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to delete.</param>
    public void DeleteVolunteer(int volunteerId);

    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="boVolunteer">The volunteer object containing details of the new volunteer.</param>
    public void AddVolunteer(BO.Volunteer boVolunteer);
}
