namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;                                            

    public string Login(string username, string password)
    {
        var volunteer = _dal.Volunteer.ReadAll(v => v.Email == username).FirstOrDefault();
        if (volunteer == null)
        {
            throw new BO.BlWrongItemtException($"User with email {username} not found.");
        }

        VolunteerManager.CheckPassword(password);

        if (volunteer.Password != password)
        {
            throw new BO.BlWrongItemtException("Incorrect password.");
        }

        return volunteer.Job.ToString();
    }

    public IEnumerable<BO.VolunteerInList> RequestVolunteerList(bool? isActive, VolunteerSortField? sortField = null)
    {
        // הנחה שיש פונקציה ב-VolunteerManager שמחזירה את כל המתנדבים
        var volunteers = _dal.Volunteer.ReadAll(); // או כל מקור נתונים אחר שמחזיר רשימת מתנדבים

        if (isActive.HasValue)
        {
            volunteers = volunteers.Where(v => v.Active == isActive.Value).ToList();
        }

        switch (sortField)
        {
            case VolunteerSortField.Name:
                volunteers = volunteers.OrderBy(v => v.FullName).ToList();
                break;
            case VolunteerSortField.ActivityStatus:
                volunteers = volunteers.OrderBy(v => v.Active).ToList();
                break;
            case VolunteerSortField.Job:
                volunteers = volunteers.OrderBy(v => v.Job).ToList();
                break;
            case VolunteerSortField.ID:
            default:
                volunteers = volunteers.OrderBy(v => v.Id).ToList();
                break;
        }

        // הפונקציה ב-VolunteerManager נקראת עכשיו נכון
        return volunteers.Select(v => VolunteerManager.GetVolunteerInList(v.Id));
    }



    public BO.Volunteer RequestVolunteerDetails(int volunteerId)
    {
        try
        {
            return (VolunteerManager.GetVolunteer(volunteerId));

        }
        catch (Exception ex) {
            Console.WriteLine($"Error while reading volunteer with ID {volunteerId}: {ex.Message}");
            return null;
        }

        
     }
       public void UpdateVolunteerDetails(int volunteerId, Volunteer volunteerDetails)
    {
        throw new NotImplementedException();
    }

    public void DeleteVolunteer(int volunteerId)
    {
        throw new NotImplementedException();
    }

    public void AddVolunteer(Volunteer volunteerDetails)
    {
        throw new NotImplementedException();
    }
}