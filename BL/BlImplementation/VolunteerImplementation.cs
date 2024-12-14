namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Create(Volunteer boVolunteer)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }


    public DO.Role PasswordEntered(int Id, string password)
    {

        var volunteer = _dal.Volunteer.Read(Id);
        if (volunteer == null)

            throw new BO.BlDoesNotExistException("The user does not exist");

        if (volunteer.Password != password)

            throw new BO.BlIncorrectPasswordException("The password is incorrect");

        return volunteer.Job;
    }

    public IEnumerable<VolunteerInList> GetVolunteers(bool? Active, VolunteerSortField? sortBy)
    {
        // Retrieve all volunteers from the data store
        var volunteers = _dal.Volunteer.ReadAll();

        // Filter by active status (if a boolean value is provided)
        if (Active.HasValue)
        {
            volunteers = volunteers.Where(volunteer => volunteer.Active == Active.Value);
        }

        // Sort the list by the selected field or by ID if no field is selected
        volunteers = sortBy switch
        {

            VolunteerSortField.Name => volunteers.OrderBy(volunteer => volunteer.FullName), // Sort by volunteer's full name
            VolunteerSortField.Job => volunteers.OrderBy(volunteer => volunteer.Job),     // Sort by volunteer's job (role)
            VolunteerSortField.ActivityStatus => volunteers.OrderBy(volunteer => volunteer.Active), // Sort by activity status (active/inactive)
            _ => volunteers.OrderBy(volunteer => volunteer.Id) // Default: sort by volunteer ID
        };

        // Convert the data to the logical entity "VolunteerInList" for display
        return volunteers.Select(volunteer => new VolunteerInList
        {
            Id = volunteer.Id,
            FullName = volunteer.FullName,
            Active = volunteer.Active
        });
    }

    public Volunteer? Read(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(int id, Volunteer boVolunteer)
    {
        throw new NotImplementedException();
    }

    public Volunteer VolunteerDetails(int id)
    {
        //    try
        //    {
        //        // Retrieve the volunteer data from the DAL. 
        //        // If no volunteer exists with the given ID, throw a business logic exception.
        //        var doVolunteer = _dal.Volunteer.Read(id)
        //            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.");

        //        // Map the data object (DO) to the business object (BO)
        //        var boVolunteer = new BO.Volunteer
        //        {
        //            Id = doVolunteer.Id,                          // Volunteer ID
        //            FullName = doVolunteer.FullName,                  // Full name of the volunteer
        //            PhoneNumber = doVolunteer.PhoneNumber,       // Phone number of the volunteer
        //            Email = doVolunteer.Email,                    // Email address
        //            Password = doVolunteer.Password,              // User password (consider hashing if not already done)
        //            FullAddress = doVolunteer.FullAddress, // Full address of the volunteer
        //            Latitude = doVolunteer.Latitude,              // Current latitude of the volunteer
        //            Longitude = doVolunteer.Longitude,            // Current longitude of the volunteer
        //            Job = (BO.Role)doVolunteer.Job,              // Role of the volunteer, mapped to BO.Role
        //            Active = doVolunteer.Active,                  // Indicates if the volunteer is currently active
        //            //MaxReading = doVolunteer.distance,            // Maximum distance the volunteer can handle
        //            CurrentCall = null                             // Initialize CurrentCall as null (will populate later if exists)
        //        };

        //        // Check if there is an ongoing call associated with the volunteer
        //        var doCall = _dal.Call.Read(doVolunteer.CurrentCallId);
        //        if (doCall != null)
        //        {
        //            // Map the call data object (DO) to the business object (BO)
        //            boVolunteer.CurrentCall = new BO.CallInProgress
        //            {
        //                CallId = doCall.Id,                       // Call ID
        //                Description = doCall.Description,  // Description of the call
        //                CallType = (BO.CallType)doCall.Type, // Type of the call (mapped to BO.Calltype)
        //                                                     // Populate additional fields if necessary (e.g., location, status, etc.)
        //            };
        //        }

        //        // Return the fully populated BO.Volunteer object
        //        return boVolunteer;
        //    }
        //    catch (DO.DalDeletionImpossible ex) // Handle exception when volunteer does not exist
        //    {
        //        throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.", ex);
        //    }
        //    catch (Exception ex) // Catch all other exceptions and wrap in a general business logic exception
        //    {
        //        throw new BO.GeneralException("An unexpected error occurred while retrieving volunteer details.", ex);
        //    }
        //} 
    }
}