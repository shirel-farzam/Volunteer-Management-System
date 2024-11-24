using Dal;
using DalApi;
using DO;


internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        // Load the list of calls from the XML file
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

        // Check if a call with the same ID already exists
        if (Calls.Any(c => c.Id == item.Id))
            throw new DalAlreadyExistsException($"Call with ID={item.Id} already exists");

        // Add the new call
        Calls.Add(item);

        // Save the updated list back to the XML file
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }

    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);

    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);

    }

    public Call? Read(int id)
    {
        // Load the list of calls from the XML file
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

        // Find the call with the specified ID
        Call? call = Calls.FirstOrDefault(c => c.Id == id);

        // If not found, throw an exception
        if (call == null)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist");

        // Return the found call
        return call;
    }

    public Call? Read(Func<Call, bool> filter)
    {
        // Load the list of calls from the XML file
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

        // Find the first call matching the filter
        Call? call = Calls.FirstOrDefault(filter);

        // If no match is found, throw an exception
        if (call == null)
            throw new DalDoesNotExistException("No call matching the given filter was found");

        // Return the matching call
        return call;
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        // Load the list of calls from the XML file
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

        // Apply the filter if provided, otherwise return all calls
        return filter == null ? Calls : Calls.Where(filter);
    }

    public void Update(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
}
