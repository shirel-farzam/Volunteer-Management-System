
namespace BlApi;

public interface IVolunteer
{
    void EnterSystem(BO.Volunteer boVolunteer, string password);
    IEnumerable<BO.VolunteerInList> GetVolunteers(bool? isActive,BO.VolunteerSortField? sortBy);
    BO.Volunteer? Read(int id);
    void Update(int id, BO.Volunteer boVolunteer);
    void Delete(int id);
    void Create(BO.Volunteer boVolunteer);
   


}
