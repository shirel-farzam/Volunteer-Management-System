namespace Dal;
using DO;
using DalApi;
using System;

public class VolunteerImplementation : IVolunteer
{
    // Creates a new volunteer and adds it to the data source
    public void Create(Volunteer item)
    {
        // Check if a volunteer with the same ID already exists
        if (DataSource.Volunteers.Any(v => v.Id == item.Id))
        {
            throw new Exception($"Volunteer with ID={item.Id} already exists"); ;
        }

        // Add the new volunteer to the data source
        DataSource.Volunteers.Add(item);
    }

    // Retrieves a volunteer by its ID
    public Volunteer? Read(int id)
    {
        var volunteer1 = DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        if (volunteer1 == null)
            return null;
        return volunteer1;
    }

    // Retrieves all volunteers from the data source
    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    // Updates an existing volunteer in the data source
    public void Update(Volunteer item)
    {
        // Find the index of the volunteer to be updated
        var index = DataSource.Volunteers.FindIndex(v => v.Id == item.Id);
        if (index == -1) throw new Exception($"Volunteer with ID={item.Id} not exists");

        // Update the volunteer's data at the specified index
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

    // Deletes a volunteer by its ID
    public void Delete(int id)
    {
        var volunteer = DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        if (volunteer == null) throw new Exception($"Volunteer with ID={id} not exists");

        // Remove the volunteer from the data source
        DataSource.Volunteers.Remove(volunteer);
    }

    // Deletes all volunteers from the data source
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }
}
