namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

    private readonly DalApi.IDal _dal = DalApi.Factory.Get; // Access DAL instance

    // Volunteer login with username and password validation
    public BO.Role Login(int username, string password)
    {
        var volunteer = _dal.Volunteer.Read(username)
            ?? throw new BO.BlNullPropertyException("The volunteer does not exist");
        VolunteerManager.CheckPassword(password); // Validate password format
        if (volunteer.Password != password)
            throw new BO.BlWrongInputException("Incorrect password");

        return (BO.Role)volunteer.Job; // Return volunteer's role
    }

    // Retrieves a filtered and sorted list of volunteers
    public IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive, BO.VolunteerInListField? sortField = null)
    {
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll()
            ?? throw new BO.BlNullPropertyException("No volunteers in the database");

        // Convert DO.Volunteer to BO.VolunteerInList
        var boVolunteersInList = volunteers
            .Select(VolunteerManager.ConvertDOToBOInList);

        // Apply filter and sorting if specified
        var filteredVolunteers = isActive.HasValue
            ? boVolunteersInList.Where(v => v.Active == isActive)
            : boVolunteersInList;

        var sortedVolunteers = sortField.HasValue
            ? filteredVolunteers.OrderBy(v => sortField switch
            {
                BO.VolunteerInListField.Id => (object)v.Id,
                BO.VolunteerInListField.FullName => v.FullName,
                BO.VolunteerInListField.Active => v.Active,
                BO.VolunteerInListField.TotalCallsHandled => v.TotalCallsHandled,
                BO.VolunteerInListField.TotalCallsCanceled => v.TotalCallsCanceled,
                BO.VolunteerInListField.TotalCallsExpired => v.TotalCallsExpired,
                BO.VolunteerInListField.CurrentCallId => v.CurrentCallId ?? 0,
                BO.VolunteerInListField.CurrentCallType => v.CurrentCallType.ToString(),
                _ => v.Id
            })
            : filteredVolunteers.OrderBy(v => v.Id); // Default sort by ID

        return sortedVolunteers;
    }

    // Retrieves detailed information about a specific volunteer
    public BO.Volunteer Read(int volunteerId)
    {
        var doVolunteer = _dal.Volunteer.Read(volunteerId)
            ?? throw new BO.BlWrongInputException($"Volunteer with ID={volunteerId} does not exist");
        var calls = _dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();

        int totalCallsHandled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated);
        int totalCallsCanceled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.SelfCancel);
        int totalCallsExpired = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.ExpiredCancel);
        int? currentCallId = calls.FirstOrDefault(ass => ass.TimeEnd == null)?.Id;


        return new BO.Volunteer
        {
            Id = volunteerId,
            FullName = doVolunteer.FullName,
            PhoneNumber = doVolunteer.PhoneNumber,
            TypeDistance = (BO.Distance)doVolunteer.TypeDistance,
            Email = doVolunteer.Email,
            Job = (BO.Role)doVolunteer.Job,
            Active = doVolunteer.Active,
            Password = doVolunteer.Password,
            FullAddress = doVolunteer.FullAddress,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            CurrentCall = VolunteerManager.GetCallIn(doVolunteer),
            TotalCanceledCalls = totalCallsCanceled,
            TotalExpiredCalls = totalCallsExpired,
            TotalHandledCalls = totalCallsHandled,
            MaxReading=doVolunteer.MaxReading
            
        };
    }

    // Updates volunteer details with validations
    public void Update(int volunteerId, BO.Volunteer boVolunteer)
    {
        var doVolunteer = _dal.Volunteer.Read(volunteerId)
            ?? throw new BO.BlWrongInputException($"Volunteer with ID={volunteerId} does not exist");

        var manager = _dal.Volunteer.Read(boVolunteer.Id)
            ?? throw new BO.BlWrongInputException($"Manager with ID={boVolunteer.Id} does not exist");

        if (manager.Job != DO.Role.Manager&&volunteerId!=boVolunteer.Id)
            throw new BO.BlWrongInputException("Only a manager can update details");

        if (boVolunteer.FullAddress != doVolunteer.FullAddress)
        {
            var coordinates = VolunteerManager.GetCoordinatesFromAddress(boVolunteer.FullAddress);
            boVolunteer.Latitude = coordinates[0];
            boVolunteer.Longitude = coordinates[1];
        }

        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);

        var volunteerUpdate = new DO.Volunteer(
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
            _dal.Volunteer.Update(volunteerUpdate);
            VolunteerManager.Observers.NotifyItemUpdated(volunteerUpdate.Id);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
    }

    // Deletes a volunteer if there are no active assignments
    public void DeleteVolunteer(int volunteerId)
    {
        var doVolunteer = _dal.Volunteer.Read(volunteerId);
        var assignments = _dal.Assignment.ReadAll(ass => ass.VolunteerId == volunteerId);

        if (assignments.Any(ass => ass.TimeEnd == null))
            throw new BO.BlWrongInputException("Volunteer has active assignments");

        try
        {
            _dal.Volunteer.Delete(volunteerId);
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5  	

        }
        catch (DO.DalDeletionImpossible ex)
        {
            throw new BO.BlDeleteNotPossibleException("Deletion failed due to invalid ID", ex);
        }
    }

    // Adds a new volunteer with validations
    public void AddVolunteer(BO.Volunteer boVolunteer)
    {
        var coordinates = VolunteerManager.GetCoordinatesFromAddress(boVolunteer.FullAddress);
        boVolunteer.Latitude = coordinates[0];
        boVolunteer.Longitude = coordinates[1];

        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);

        var doVolunteer = new DO.Volunteer(
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
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5   
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
    }
}
