namespace Dal;
using DO;
using DalApi;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        // Generate a new ID for the call
        int newId = Config.NextCallId;

        // Create a copy of the item with the new ID and add it to the list
        Call copy = item with { Id = newId };
        DataSource.Calls.Add(copy);
    }

    public void Delete(int id)
    {
        // Find the call by its ID
        Call? call1 = DataSource.Calls.Find(c => c.Id == id);
        if (call1 == null)
            throw new Exception($"Call with ID={id} not exists");

        // Remove the call from the list
        DataSource.Calls.Remove(call1);
    }

    public void DeleteAll()
    {
        // Clear the entire list of calls
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        // Find a call by its ID
        var findCall = DataSource.Calls.Find(c => c.Id == id);
        if (findCall != null)
            return findCall;

        // Return null if the call does not exist
        return null;
    }

    public List<Call> ReadAll()
    {
        // Return a copy of the entire call list
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        // Find the index of the call by its ID
        int index = DataSource.Calls.FindIndex(c => c.Id == item.Id);
        if (index == -1)
            throw new Exception($"Call with ID={item.Id} does not exist.");

        // Update the call in the list
        DataSource.Calls[index] = item;
    }
}
