
namespace BlApi;

public interface IVolunteer
{
    public DO.Role PasswordEntered(int Id, string password);
    IEnumerable<BO.VolunteerInList> GetVolunteers(bool? isActive,BO.VolunteerSortField? sortBy);
    BO.Volunteer VolunteerDetails(int id);
    BO.Volunteer? Read(int id);
    void Update(int id, BO.Volunteer boVolunteer);
    void Delete(int id);
    void Create(BO.Volunteer boVolunteer);
   


}
