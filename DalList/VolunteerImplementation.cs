namespace Dal;
using DO;
using DalApi;
using System;

internal class VolunteerImplementation : IVolunteer
{
    // Create a new Volunteer
    public void Create(Volunteer item)
    {
        // Check if a volunteer with the same ID already exists in the DataSource
        if (DataSource.Volunteers.Any(v => v.Id == item.Id))
        {
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists"); // If exists, throw an exception
        }

        DataSource.Volunteers.Add(item); // Add the new volunteer to the DataSource
        // return item.Id; // (Commented out: ID return value can be added if needed)
    }

    // Read a Volunteer by its ID
    public Volunteer? Read(int id)
    {
        // var volunteer1 = DataSource.Volunteers.FirstOrDefault(v => v.Id == id); // Search for the volunteer by ID stage 1


        //if (volunteer1 == null)
        //    return null; // If no volunteer is found, return null
        //return volunteer1; // Return the found volunteer
        return DataSource.Volunteers.FirstOrDefault(item => item.Id == id); //stage 2

    }
    public Volunteer? Read(Func<Volunteer, bool> filter) // stage 2
    {
        // Finds the first volunteer that matches the filter function and returns it
        return DataSource.Volunteers.FirstOrDefault(filter);
    }


    // Read all Volunteers
    //public List<Volunteer> ReadAll()
    //{
    //    return new List<Volunteer>(DataSource.Volunteers); // Return a new list containing all volunteers in DataSource
    //} stage 1
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) //stage 2
   => filter == null
       ? DataSource.Volunteers.Select(item => item)
    : DataSource.Volunteers.Where(filter);

    // Update an existing Volunteer
    public void Update(Volunteer item)
    {
        // Find the index of the volunteer to update by ID
        var index = DataSource.Volunteers.FindIndex(v => v.Id == item.Id);
        if (index == -1) throw new DalDoesNotExistException($"Volunteer with ID={item.Id} not exists"); // If not found, throw an exception

        // Replace the existing volunteer at the found index with the updated volunteer details
        DataSource.Volunteers[index] = new Volunteer
        {
            Id = item.Id,
            FullName = item.FullName,
            PhoneNumber = item.PhoneNumber,
            Email = item.Email,
            TypeDistance = item.TypeDistance,
            Job = item.Job,
            Active = item.Active,
            Password = item.Password,
            FullAddress = item.FullAddress,
            Latitude = item.Latitude,
            Longitude = item.Longitude,
            MaxReading = item.MaxReading,
        };
    }

    // Delete a Volunteer by its ID
    public void Delete(int id)
    {
        var volunteer = DataSource.Volunteers.FirstOrDefault(v => v.Id == id); // Search for the volunteer by ID
        if (volunteer == null) throw new DalDoesNotExistException($"Volunteer with ID={id} not exists"); // If not found, throw an exception

        DataSource.Volunteers.Remove(volunteer); // Remove the volunteer from the DataSource
    }

    // Delete all Volunteers
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear(); // Clear all volunteers from the DataSource
    }
}
