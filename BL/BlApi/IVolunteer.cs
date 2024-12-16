namespace BlApi;
using BO;

public interface IVolunteer
{
    public string Login(string username, string password);

    public IEnumerable<BO.VolunteerInList> RequestVolunteerList(bool? isActive, VolunteerSortField? sortField = null);

    public BO.Volunteer RequestVolunteerDetails(int volunteerId);

    public void UpdateVolunteerDetails(int volunteerId, BO.Volunteer boVolunteer);

    public void DeleteVolunteer(int volunteerId);

    public void AddVolunteer(BO.Volunteer boVolunteer);
}
