using Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

/// <summary>
/// Implementation of the IVolunteer interface for managing Volunteer entities in the DAL.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Converts an XElement into a Volunteer object.
    /// </summary>
    /// <param name="v">The XElement to convert.</param>
    /// <returns>A Volunteer object created from the XElement.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    static Volunteer GetVolunteer(XElement v)
    {
        Volunteer volunteer = new Volunteer
        (
            Id: int.TryParse((string?)v.Element("Id"), out var id) ? id : throw new FormatException("Invalid ID format."),
            FullName: (string?)v.Element("FullName") ?? throw new FormatException("FullName is missing."),
            PhoneNumber: (string?)v.Element("PhoneNumber") ?? throw new FormatException("PhoneNumber is missing."),
            Email: (string?)v.Element("Email") ?? throw new FormatException("Email is missing."),
            TypeDistance: Enum.TryParse((string?)v.Element("TypeDistance"), out Distance distanceType)
                ? distanceType
                : throw new FormatException("Invalid TypeDistance format."),
            Job: Enum.TryParse((string?)v.Element("Job"), out Role role)
                ? role
                : throw new FormatException("Invalid Job format."),
            Active: bool.TryParse((string?)v.Element("Active"), out bool active) ? active : throw new FormatException("Invalid Active format."),
            Password: (string?)v.Element("Password"),
            FullAddress: (string?)v.Element("FullAddress"),
            Latitude: double.TryParse((string?)v.Element("Latitude"), out double latitude) ? latitude : null,
            Longitude: double.TryParse((string?)v.Element("Longitude"), out double longitude) ? longitude : null,
            MaxReading: double.TryParse((string?)v.Element("MaxReading"), out double maxReading) ? maxReading : null
        );

        return volunteer;
    }

    /// <summary>
    /// Creates a new Volunteer entity.
    /// </summary>
    /// <param name="item">The Volunteer entity to create.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown if a Volunteer with the same ID already exists.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        if (volunteersRoot.Elements().Any(v => (int?)v.Element("Id") == item.Id))
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists.");

        volunteersRoot.Add(CreateVolunteerElement(item));
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Converts a Volunteer object into an XElement.
    /// </summary>
    /// <param name="v">The Volunteer object to convert.</param>
    /// <returns>An XElement representing the Volunteer object.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    private static XElement CreateVolunteerElement(Volunteer v)
    {
        return new XElement("Volunteer",
            new XElement("Id", v.Id),
            new XElement("FullName", v.FullName),
            new XElement("PhoneNumber", v.PhoneNumber),
            new XElement("Email", v.Email),
            new XElement("TypeDistance", v.TypeDistance),
            new XElement("Job", v.Job),
            new XElement("Active", v.Active),
            new XElement("Password", v.Password),
            new XElement("FullAddress", v.FullAddress),
            new XElement("Latitude", v.Latitude),
            new XElement("Longitude", v.Longitude),
            new XElement("MaxReading", v.MaxReading)
        );
    }

    /// <summary>
    /// Deletes a Volunteer entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Volunteer with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? volunteerElem = volunteersRoot.Elements()
            .FirstOrDefault(v => (int?)v.Element("Id") == id)
            ?? throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist.");

        volunteerElem.Remove();
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Deletes all Volunteer entities.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        volunteersRoot.RemoveAll();
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Reads a Volunteer entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to read.</param>
    /// <returns>The Volunteer entity with the specified ID, or null if it does not exist.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        XElement? volunteerElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)
            .Elements()
            .FirstOrDefault(v => (int?)v.Element("Id") == id);

        return volunteerElem is null ? null : GetVolunteer(volunteerElem);
    }

    /// <summary>
    /// Reads the first Volunteer entity that matches a specified filter.
    /// </summary>
    /// <param name="filter">The filter function to apply.</param>
    /// <returns>The first Volunteer entity that matches the filter, or null if none match.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)
            .Elements()
            .Select(GetVolunteer)
            .FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all Volunteer entities, optionally filtered by a specified condition.
    /// </summary>
    /// <param name="filter">The optional filter function to apply.</param>
    /// <returns>An enumerable collection of Volunteer entities that match the filter, or all entities if no filter is provided.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        var volunteers = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)
            .Elements()
            .Select(GetVolunteer);

        return filter == null ? volunteers : volunteers.Where(filter);
    }

    /// <summary>
    /// Updates an existing Volunteer entity.
    /// </summary>
    /// <param name="item">The updated Volunteer entity.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Volunteer with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? volunteerElem = volunteersRoot.Elements()
            .FirstOrDefault(v => (int?)v.Element("Id") == item.Id)
            ?? throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist.");

        volunteerElem.Remove();
        volunteersRoot.Add(CreateVolunteerElement(item));
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }
}