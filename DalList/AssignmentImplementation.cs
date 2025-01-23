namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new assignment by generating a new ID and adding it to the data source.
    /// </summary>
    
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        // Generate a new ID for the assignment
        int newId = Config.NextAssignmentId;

        // Create a copy of the item with the new ID and add it to the list
        Assignment copy = item with { Id = newId };
        DataSource.Assignments.Add(copy);
    }

    /// <summary>
    /// Deletes an assignment by its ID from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        // Find the assignment by its ID
        var assignment = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            throw new DalDoesNotExistException($"Assignment with ID={id} not exists");

        // Remove the assignment from the list
        DataSource.Assignments.Remove(assignment);
    }

    /// <summary>
    /// Deletes all assignments from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        // Clear the entire list of assignments
        DataSource.Assignments.Clear();
    }

    /// <summary>
    /// Retrieves an assignment by its ID.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        // Search for an assignment by ID
        return DataSource.Assignments.FirstOrDefault(item => item.Id == id); //stage 2
    }

    /// <summary>
    /// Retrieves the first assignment that matches the provided filter function.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        // Finds the first assignment that matches the filter function and returns it
        return DataSource.Assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Retrieves all assignments that match the provided filter function.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        // If no filter is provided, return all assignments
        // Otherwise, return assignments that match the filter
        return filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing assignment in the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        // Find the index of the assignment by its ID
        int index = DataSource.Assignments.FindIndex(c => c.Id == item.Id);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} not exists");
        }

        // Update the assignment in the list
        DataSource.Assignments[index] = item;
    }
}
