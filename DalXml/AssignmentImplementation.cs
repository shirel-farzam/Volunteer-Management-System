namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Implementation of the IAssignment interface for managing Assignment entities in the DAL.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new Assignment entity and assigns it a unique ID.
    /// </summary>
    /// <param name="item">The Assignment entity to create.</param>
    public void Create(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        int newId = Config.NextAssignmentId; // Generate a unique ID
        Assignment newItem = item with { Id = newId }; // Create a new Assignment with the new ID
        assignments.Add(newItem);

        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes an Assignment entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Assignment to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Assignment with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");

        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes all Assignment entities from the storage.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    /// <summary>
    /// Reads an Assignment entity that matches a given filter.
    /// </summary>
    /// <param name="filter">The filter function to apply.</param>
    /// <returns>The first Assignment entity matching the filter, or null if none are found.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads an Assignment entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Assignment to read.</param>
    /// <returns>The Assignment entity with the specified ID, or null if it does not exist.</returns>
    public Assignment? Read(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    /// Reads all Assignment entities, optionally filtered by a given condition.
    /// </summary>
    /// <param name="filter">The optional filter function to apply.</param>
    /// <returns>A collection of all Assignment entities that match the filter, or all entities if no filter is provided.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return filter == null ? assignments : assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing Assignment entity.
    /// </summary>
    /// <param name="item">The updated Assignment entity.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Assignment with the specified ID does not exist.</exception>
    public void Update(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist");

        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }
}
