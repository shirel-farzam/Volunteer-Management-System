namespace Dal;
using DO;
using DalApi;
using System;
using System.Runtime.CompilerServices;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Creates a new volunteer and adds it to the data source.
    /// Throws an exception if a volunteer with the same ID already exists.
    /// </summary>
    /// <param name="item">The volunteer to be created.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        // Check if the volunteer already exists in the data source
        if (DataSource.Volunteers.Any(v => v.Id == item.Id))
        {
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
        }

        // Add the volunteer to the data source
        DataSource.Volunteers.Add(item);
    }

    /// <summary>
    /// Retrieves a volunteer by their unique ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to retrieve.</param>
    /// <returns>The volunteer with the given ID or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    /// Retrieves a volunteer using a custom filter function.
    /// </summary>
    /// <param name="filter">The filter function to apply to the volunteers.</param>
    /// <returns>The first volunteer that matches the filter or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }

    /// <summary>
    /// Retrieves all volunteers, optionally filtered by a custom function.
    /// </summary>
    /// <param name="filter">An optional filter function to apply to the list of volunteers.</param>
    /// <returns>A list of volunteers that match the filter.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) =>
        filter == null
            ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);

    /// <summary>
    /// Updates an existing volunteer in the data source.
    /// Throws an exception if the volunteer does not exist.
    /// </summary>
    /// <param name="item">The volunteer with updated data.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        // Find the index of the volunteer in the data source
        var index = DataSource.Volunteers.FindIndex(v => v.Id == item.Id);
        if (index == -1) throw new DalDoesNotExistException($"Volunteer with ID={item.Id} not exists");

        // Update the volunteer in the data source
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

    /// <summary>
    /// Deletes a volunteer by their unique ID.
    /// Throws an exception if the volunteer does not exist.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        // Find the volunteer to delete
        var volunteer = DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        if (volunteer == null) throw new DalDoesNotExistException($"Volunteer with ID={id} not exists");

        // Remove the volunteer from the data source
        DataSource.Volunteers.Remove(volunteer);
    }

    /// <summary>
    /// Deletes all volunteers from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }
}
