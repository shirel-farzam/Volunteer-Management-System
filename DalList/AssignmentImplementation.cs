using DalApi;
using DO;

namespace Dal;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        // Generate a new ID for the assignment
        int newId = Config.NextAssignmentId;

        // Create a copy of the item with the new ID and add it to the list
        Assignment copy = item with { Id = newId };
        DataSource.Assignments.Add(copy);
    }

    public void Delete(int id)
    {
        // Find the assignment by its ID
        var assignment = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            throw new Exception($"Assignment with ID={id} not exists");

        // Remove the assignment from the list
        DataSource.Assignments.Remove(assignment);
    }

    public void DeleteAll()
    {
        // Clear the entire list of assignments
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        // Search for an assignment by ID
        var assignment = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            return null;
        return assignment;
    }

    public List<Assignment> ReadAll()
    {
        // Return a copy of the entire assignment list
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        // Find the index of the assignment by ID
        int index = DataSource.Assignments.FindIndex(c => c.Id == item.Id);
        if (index == -1)
        {
            throw new Exception($"Assignment with ID={item.Id} not exists");
        }
        // Update the assignment in the list
        DataSource.Assignments[index] = item;
    }
}
