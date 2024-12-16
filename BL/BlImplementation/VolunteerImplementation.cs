namespace BlImplementation;
using BlApi;
using BO;
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
            throw new BO.BlWrongItemException($"User with email {username} not found.");
        }

        VolunteerManager.CheckPassword(password);

        if (volunteer.Password != password)
        {
            throw new BO.BlWrongItemException("Incorrect password.");
        }

        return volunteer.Job.ToString();
    }

    public IEnumerable<BO.VolunteerInList> RequestVolunteerList(bool? isActive,  BO.VolunteerSortField? sortField = null)
    {
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll() ?? throw new BO.BlNullPropertyException("There are not volunteers int database");
        IEnumerable<BO.VolunteerInList> boVolunteersInList = volunteers
            .Select(doVolunteer => VolunteerManager.ConvertDOToBOInList(doVolunteer));

        // If an 'active' filter is provided, filter the volunteers based on their active status
        // Otherwise, keep all volunteers without filtering
        var filteredVolunteers = isActive.HasValue
              ? boVolunteersInList.Where(v => v.Active == isActive)
              : boVolunteersInList;

        // If a 'sortBy' criteria is provided, sort the filtered volunteers by the selected property
        var sortedVolunteers = sortField.HasValue
            ? filteredVolunteers.OrderBy(v =>
                sortField switch
                {
                    // Sorting by different properties of the volunteer (ID, Full Name, etc.)
                    BO.VolunteerSortField.ID => (object)v.Id, // Sorting by ID (T.Z)
                    BO.VolunteerSortField.Name => v.FullName, // Sorting by full name
                    BO.VolunteerSortField.ActivityStatus => v.Active, // Sorting by active status
                    BO.VolunteerSortField.SumCalls => v.TotalCallsHandled, // Sorting by total number of calls
                    BO.VolunteerSortField.SumCanceled => v.TotalCallsCanceled, // Sorting by total number of cancellations
                    BO.VolunteerSortField.SumExpired => v.TotalCallsExpired, // Sorting by total number of expired calls
                    BO.VolunteerSortField.IdCall => v.CurrentCallId ?? null, // Sorting by call ID (nullable)
                    BO.VolunteerSortField.CType => v.CurrentCallType.ToString(), // Sorting by call type (converted to string)
                })
            : filteredVolunteers.OrderBy(v => v.Id); // Default sorting by ID (T.Z) if no 'sortBy' is provided

        // Return the sorted and filtered list of volunteers
        return sortedVolunteers;
    }
    public BO.Volunteer RequestVolunteerDetails(int volunteerId)
    {

        var doVolunteer = _dal.Volunteer.Read(volunteerId) ??
        throw new BO.BlWrongInputException($"Volunteer with ID={volunteerId} does Not exist");
        return new()
        {
            Id = volunteerId,
            FullName = doVolunteer.FullName,
            PhoneNumber = doVolunteer.PhoneNumber,
            TypeDistance = (BO.Distance)doVolunteer.TypeDistance,
            Job = (BO.Role)doVolunteer.Job,
            Active = doVolunteer.Active,
            Password = doVolunteer.Password,
            FullAddress = doVolunteer.FullAddress,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            CurrentCall = VolunteerManager.GetCallIn(doVolunteer),
        };

    }
    public void UpdateVolunteerDetails(int volunteerId, BO.Volunteer boVolunteer)
    {
        // קריאת המתנדב מה-DAL
        DO.Volunteer doVolunteer = _dal.Volunteer.Read(volunteerId)
            ?? throw new BO.BlWrongInputException($"Volunteer with ID={volunteerId} does not exist");

        // בדיקת האם המשתמש שמבצע את העדכון הוא מנהל
        DO.Volunteer manager = _dal.Volunteer.Read(boVolunteer.Id)
            ?? throw new BO.BlWrongInputException($"Volunteer with ID={boVolunteer.Id} does not exist");

        if (manager.Job != DO.Role.Boss)
            throw new BO.BlWrongInputException("Only a manager can update volunteer details.");

        // אם הכתובת השתנתה, עדכון הקואורדינטות
        if (boVolunteer.FullAddress != doVolunteer.FullAddress)
        {
            double[] coordinates = VolunteerManager.GetCoordinates(boVolunteer.FullAddress);
            boVolunteer.Latitude = coordinates[0];
            boVolunteer.Longitude = coordinates[1];
        }

        // בדיקות לוגיות
        VolunteerManager.CheckLogic(boVolunteer);

        // בדיקות פורמט
        VolunteerManager.CheckFormat(boVolunteer);

        // אם המשתמש אינו מנהל, לא ניתן לשנות את התפקיד
        if (manager.Job != DO.Role.Boss && boVolunteer.Job != (BO.Role)doVolunteer.Job)
            throw new BO.BlWrongInputException("You do not have permission to change the role.");

        // יצירת אובייקט מעודכן
        DO.Volunteer volunteerUpdate = new DO.Volunteer(
            boVolunteer.Id,
            boVolunteer.FullName,
            boVolunteer.PhoneNumber,
            boVolunteer.Email,
            (DO.Distance)boVolunteer.TypeDistance,
            (DO.Role)boVolunteer.Job,
            boVolunteer.Active,
            boVolunteer.Password,
            boVolunteer.FullAddress,
            boVolunteer.Latitude,
            boVolunteer.Longitude,
            boVolunteer.MaxReading
        );

        // ניסיון לעדכון ב-DAL
        try
        {
            _dal.Volunteer.Update(volunteerUpdate);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} does not exist", ex);
        }
    }

    public void DeleteVolunteer(int volunteerId)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(volunteerId);
        IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll(ass => ass.VolunteerId == volunteerId);

        if (assignments != null && assignments.Count(ass => ass.TimeEnd == null) > 0)

            throw new BlWrongInputException("cannot delete have assignment in treat");
        try
        {
            _dal.Volunteer.Delete(volunteerId);
        }
        catch (DO.DalDeletionImpossible doEx)
        {
            throw new BO.BlDeleteNotPossibleException("id not valid", doEx);
        }
    }
    public void AddVolunteer(BO.Volunteer boVolunteer)
    {

        double[] cordinate = VolunteerManager.GetCoordinates(boVolunteer.FullAddress);
        double latitude = cordinate[0];
        double longitude = cordinate[1];
        boVolunteer.Latitude = latitude;
        boVolunteer.Longitude = longitude;
        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);
        DO.Volunteer doVolunteer = new
            (
            boVolunteer.Id,
            boVolunteer.FullName,
            boVolunteer.PhoneNumber,
            boVolunteer.Email,
            (DO.Distance)boVolunteer.TypeDistance,
            (DO.Role)boVolunteer.Job,
            boVolunteer.Active,
            boVolunteer.Password,
            boVolunteer.FullAddress,
            boVolunteer.Latitude,
            boVolunteer.Longitude,
            boVolunteer.MaxReading

            );

        try
        {
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
    }
}