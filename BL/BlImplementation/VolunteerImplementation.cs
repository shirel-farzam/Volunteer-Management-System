namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    // Handles volunteer login by verifying username and password
    public BO.Role Login(int username, string password)
    {
        var volunteer = _dal.Volunteer.Read(username)
            ?? throw new BO.BlNullPropertyException("The volunteer is null");
        VolunteerManager.CheckPassword(password);
        if (volunteer.Password != password)
            throw new BO.BlWrongInputException("The password does not match");

        return (BO.Role)volunteer.Job;
    }

    // Retrieves a filtered and/or sorted list of volunteers
    public IEnumerable<BO.VolunteerInList> RequestVolunteerList(bool? isActive, BO.VolunteerSortField? sortField = null)
    {
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll()
            ?? throw new BO.BlNullPropertyException("There are no volunteers in the database");

        IEnumerable<BO.VolunteerInList> boVolunteersInList = volunteers
            .Select(doVolunteer => VolunteerManager.ConvertDOToBOInList(doVolunteer));

        // Apply filtering based on 'isActive' status if provided
        var filteredVolunteers = isActive.HasValue
            ? boVolunteersInList.Where(v => v.Active == isActive)
            : boVolunteersInList;

        // Apply sorting based on the 'sortField' criteria if provided
        var sortedVolunteers = sortField.HasValue
            ? filteredVolunteers.OrderBy(v =>
                sortField switch
                {
                    BO.VolunteerSortField.ID => (object)v.Id, // Sort by ID
                    BO.VolunteerSortField.Name => v.FullName, // Sort by full name
                    BO.VolunteerSortField.ActivityStatus => v.Active, // Sort by active status
                    BO.VolunteerSortField.SumCalls => v.TotalCallsHandled, // Sort by total handled calls
                    BO.VolunteerSortField.SumCanceled => v.TotalCallsCanceled, // Sort by canceled calls
                    BO.VolunteerSortField.SumExpired => v.TotalCallsExpired, // Sort by expired calls
                    BO.VolunteerSortField.IdCall => v.CurrentCallId ?? null, // Sort by call ID (nullable)
                    BO.VolunteerSortField.CType => v.CurrentCallType.ToString(), // Sort by call type
                })
            : filteredVolunteers.OrderBy(v => v.Id); // Default sorting by ID

        return sortedVolunteers;
    }

    // Retrieves detailed information about a specific volunteer
    public BO.Volunteer RequestVolunteerDetails(int volunteerId)
    {
        var doVolunteer = _dal.Volunteer.Read(volunteerId)
            ?? throw new BO.BlWrongInputException($"Volunteer with ID={volunteerId} does not exist");

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

    // Updates volunteer details with necessary validations
    public void UpdateVolunteerDetails(int volunteerId, BO.Volunteer boVolunteer)
    {
        // Read the volunteer from DAL
        DO.Volunteer doVolunteer = _dal.Volunteer.Read(volunteerId)
            ?? throw new BO.BlWrongInputException($"Volunteer with ID={volunteerId} does not exist");

        // Ensure the updating user is a manager
        DO.Volunteer manager = _dal.Volunteer.Read(boVolunteer.Id)
            ?? throw new BO.BlWrongInputException($"Volunteer with ID={boVolunteer.Id} does not exist");

        if (manager.Job != DO.Role.Manager)
            throw new BO.BlWrongInputException("Only a manager can update volunteer details.");

        // If the address changed, update coordinates
        if (boVolunteer.FullAddress != doVolunteer.FullAddress)
        {
            double[] coordinates = VolunteerManager.GetCoordinates(boVolunteer.FullAddress);
            boVolunteer.Latitude = coordinates[0];
            boVolunteer.Longitude = coordinates[1];
        }

        // Perform logical and format checks on the volunteer details
        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);

        // Prevent non-managers from changing the role
        if (manager.Job != DO.Role.Manager && boVolunteer.Job != (BO.Role)doVolunteer.Job)
            throw new BO.BlWrongInputException("You do not have permission to change the role.");

        // Create an updated volunteer object
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

        // Attempt to update the volunteer in DAL
        try
        {
            _dal.Volunteer.Update(volunteerUpdate);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
    }

    // Deletes a volunteer if there are no active assignments
    public void DeleteVolunteer(int volunteerId)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(volunteerId);
        IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll(ass => ass.VolunteerId == volunteerId);

        if (assignments != null && assignments.Count(ass => ass.TimeEnd == null) > 0)
            throw new BlWrongInputException("Cannot delete - the volunteer has active assignments.");

        try
        {
            _dal.Volunteer.Delete(volunteerId);
        }
        catch (DO.DalDeletionImpossible doEx)
        {
            throw new BO.BlDeleteNotPossibleException("Deletion is not possible due to invalid ID", doEx);
        }
    }

    // Adds a new volunteer with validations
    public void AddVolunteer(BO.Volunteer boVolunteer)
    {
        // Retrieve coordinates for the address
        double[] cordinate = VolunteerManager.GetCoordinates(boVolunteer.FullAddress);
        double latitude = cordinate[0];
        double longitude = cordinate[1];
        boVolunteer.Latitude = latitude;
        boVolunteer.Longitude = longitude;

        // Perform logical and format checks on the volunteer details
        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);

        // Create a DO.Volunteer object
        DO.Volunteer doVolunteer = new(
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

        // Attempt to create the volunteer in DAL
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
