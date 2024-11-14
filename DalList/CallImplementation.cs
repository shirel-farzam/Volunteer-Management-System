namespace Dal;
using DO;
using DalApi;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        // יצירת מזהה חדש על בסיס המספר הרץ הבא
        int newId = Config.NextCallId;
        //Call newItem = new Call 
        //{
        //    Id = newId,
        //     Type=item.Type,
        //     Description=item.Description,
        //     FullAddress=item.FullAddress,
        //     Latitude=item.Latitude,
        //     Longitude=item.Longitude,
        //     TimeOpened = item.TimeOpened,
        //     MaxTimeToClose = item.MaxTimeToClose    

        //};
        Call copy = item with { Id = newId };

        DataSource.Calls.Add(copy);
        //   return newItem.Id;
    }

    public void Delete(int id)
    {
        Call? call1 = DataSource.Calls.Find(c => c.Id == id);
        if (call1 == null)
            throw new Exception($"Call with ID={id} not exists");
        DataSource.Calls.Remove(call1);
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        var findCall = DataSource.Calls.Find(c => c.Id == id);
        if (findCall != null)
            return findCall;
        return null;

    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        int index = DataSource.Calls.FindIndex(c => c.Id == item.Id);
        if (index == -1)
            throw new Exception($"Call with ID={item.Id} does not exist.");


        DataSource.Calls[index] = item;
    }
}