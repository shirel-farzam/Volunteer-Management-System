namespace Dal;
using DO;
using DalApi;
using System;


public class VolunteerlImplementation : IVolunteer
{

    public void Create(Volunteer item)
    {


        // בדיקה אם קיים אובייקט עם אותו מזהה
        if (DataSource.Volunteers.Any(v => v.Id == item.Id))
        {
            throw new Exception($"Volenteer with ID={item.Id} already exists"); ;
        }



        DataSource.Volunteers.Add(item);

        // return item.Id;
    }


    public Volunteer? Read(int id)
    {
        var volunteer1 = DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        if (volunteer1 == null)
            return null;
        return volunteer1;
    }

    public List<Volunteer> ReadAll()
    {



        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {


        var index = DataSource.Volunteers.FindIndex(v => v.Id == item.Id);
        if (index == -1) throw new Exception($"Volunteer with ID={item.Id} not exists");

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

    public void Delete(int id)
    {
        var volunteer = DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        if (volunteer == null) throw new Exception($"Volunteer with ID={id} not exists");

        DataSource.Volunteers.Remove(volunteer);
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }
}