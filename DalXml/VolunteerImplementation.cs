using Dal;
using DalApi;
using DO;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
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

    public void Create(Volunteer item)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        if (volunteersRoot.Elements().Any(v => (int?)v.Element("Id") == item.Id))
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists.");

        volunteersRoot.Add(CreateVolunteerElement(item));
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }

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

    public void Delete(int id)
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? volunteerElem = volunteersRoot.Elements()
            .FirstOrDefault(v => (int?)v.Element("Id") == id)
            ?? throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist.");

        volunteerElem.Remove();
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }

    public void DeleteAll()
    {
        XElement volunteersRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        volunteersRoot.RemoveAll();
        XMLTools.SaveListToXMLElement(volunteersRoot, Config.s_volunteers_xml);
    }

    public Volunteer? Read(int id)
    {
        XElement? volunteerElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)
            .Elements()
            .FirstOrDefault(v => (int?)v.Element("Id") == id);

        return volunteerElem is null ? null : GetVolunteer(volunteerElem);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)
            .Elements()
            .Select(GetVolunteer)
            .FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        var volunteers = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)
            .Elements()
            .Select(GetVolunteer);

        return filter == null ? volunteers : volunteers.Where(filter);
    }

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
