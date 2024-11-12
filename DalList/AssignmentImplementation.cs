using DalApi;
using DO;

namespace Dal;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        // יצירת עותק עמוק של הפריט
        int newId = Config.NextAssignmentId;
        // {
        //     Id= Config.NextAssignmentId,          
        //     CallId=item.CallId,       
        //     VolunteerId=item.VolunteerId,   
        //     TimeStart=item.TimeStart,
        //     TimeEnd =item.TimeEnd , 
        //     TypeEndTreat =item.TypeEndTreat,   
        // };
        Assignment copy = item with { Id = newId };
        DataSource.Assignments.Add(copy);

        //   return ID;
    }

    public void Delete(int id)
    {

        var assignment = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            throw new Exception($"Assignment with ID={id} not exists");

        DataSource.Assignments.Remove(assignment);
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear(); // ניקוי כל הרשימה
    }

    public Assignment? Read(int id)
    {
        // חיפוש פריט לפי מזהה
        var assignment = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            return null;
        return assignment;

    }

    public List<Assignment> ReadAll()
    {

        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {


        int index = DataSource.Assignments.FindIndex(c => c.Id == item.Id);
        if (index == -1)
        {
            throw new Exception($"Assignment with ID={item.Id} not exists");
        }
        DataSource.Assignments[index] = item;
    }
}