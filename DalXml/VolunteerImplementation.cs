namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    private XElement createVolunteerElement(Volunteer item)
    {
        return new XElement("Volunteer",
            new XElement("Id", item.Id),
            new XElement("Name", item.FullName),
            new XElement("PhoneNumber", item.PhoneNumber),
            new XElement("Email", item.Email),
            new XElement("TypeDistance", item.TypeDistance),
            new XElement("Job", item.Job),
            new XElement("IsActive", item.Active),
            new XElement("Password", item.Password),
            new XElement("FullAddress", item.FullAddress),
            new XElement("Latitude", item.Latitude),
            new XElement("Longitude", item.Longitude),
            new XElement("MaxReading", item.MaxReading)
        );
    }

    static Volunteer getVolunteer(XElement v)
    {
        return new Volunteer(
            Id: v.ToIntNullable("Id") ?? throw new FormatException("Cannot convert Id"),
            FullName: (string?)v.Element("Name") ?? "",
            PhoneNumber: (string?)v.Element("PhoneNumber") ?? "Unavailable",
            Email: (string?)v.Element("Email") ?? "Unavailable",
            TypeDistance: v.ToEnumNullable<Distance>("TypeDistance") ?? Distance.Aerial,
            Job: v.ToEnumNullable<Role>("Job") ?? Role.Volunteer,
            Active: (bool?)v.Element("IsActive") ?? false,
            Password: (string?)v.Element("Password"),
            FullAddress: (string?)v.Element("FullAddress"),
            Latitude: v.ToDoubleNullable("Latitude"),
            Longitude: v.ToDoubleNullable("Longitude"),
            MaxReading: v.ToDoubleNullable("MaxReading")
        );
    }

    public void Create(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        if (volunteersRootElem.Elements("Volunteer").Any(v => (int?)v.Element("Id") == item.Id))
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");

        volunteersRootElem.Add(createVolunteerElement(item));
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    public void Delete(int id)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? toRemove = volunteersRootElem.Elements("Volunteer").FirstOrDefault(v => (int?)v.Element("Id") == id);
        if (toRemove == null)
            throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        toRemove.Remove();
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLElement(new XElement("Volunteers"), Config.s_volunteers_xml);
    }

    public Volunteer? Read(int id)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? volunteerElem = volunteersRootElem.Elements("Volunteer").FirstOrDefault(v => (int?)v.Element("Id") == id);
        return volunteerElem == null ? null : getVolunteer(volunteerElem);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)
            .Elements("Volunteer")
            .Select(v => getVolunteer(v))
            .FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)
            .Elements("Volunteer")
            .Select(v => getVolunteer(v))
            .Where(filter ?? (_ => true));
    }

    public void Update(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? existingVolunteer = volunteersRootElem.Elements("Volunteer").FirstOrDefault(v => (int?)v.Element("Id") == item.Id);
        if (existingVolunteer == null)
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist");

        existingVolunteer.ReplaceWith(createVolunteerElement(item));
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }
}
