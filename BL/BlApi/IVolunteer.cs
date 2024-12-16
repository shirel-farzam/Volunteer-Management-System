namespace BlApi;
using BO;

public interface IVolunteer
{
    string Login(string username, string password);

    IEnumerable<VolunteerInList> RequestVolunteerList(bool? isActive, VolunteerInList? sortField = null);

    Volunteer RequestVolunteerDetails(int volunteerId);

    void UpdateVolunteerDetails(int volunteerId, Volunteer volunteerDetails);

    void DeleteVolunteer(int volunteerId);

    void AddVolunteer(Volunteer volunteerDetails);
}
