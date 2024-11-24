namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        // Load the list of volunteers from the XML file
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        // Check if a volunteer with the same ID already exists
        if (Volunteers.Any(v => v.Id == item.Id))
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");

        // Add the new volunteer
        Volunteers.Add(item);

        // Save the updated list back to the XML file
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }

    public void Delete(int id)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Volunteers.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);

    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), Config.s_volunteers_xml);

    }

    public Volunteer? Read(int id)
    {
        
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        // Search for the volunteer with the specified ID
        Volunteer? volunteer = Volunteers.FirstOrDefault(v => v.Id == id);

        // If not found, throw an exception
        if (volunteer == null)
            throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        // Return the found volunteer
        return volunteer;
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        // Load the list of volunteers from the XML file
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        // Find the first volunteer matching the filter
        Volunteer? volunteer = Volunteers.FirstOrDefault(filter);

        // If no match is found, throw an exception
        if (volunteer == null)
            throw new DalDoesNotExistException("No volunteer matching the given filter was found");

        // Return the matching volunteer
        return volunteer;
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        // Load the list of volunteers from the XML file
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        // Apply the filter if provided, otherwise return all volunteers
        return filter == null ? Volunteers : Volunteers.Where(filter);
    }

    public void Update(Volunteer item)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Volunteers.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }
}
