namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        // Load the list of assignments from the XML file
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);

        // Check if an assignment with the same ID already exists
        if (Assignments.Any(a => a.Id == item.Id))
            throw new DalAlreadyExistsException($"Assignment with ID={item.Id} already exists");

        // Add the new assignment
        Assignments.Add(item);

        // Save the updated list back to the XML file
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }

    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);

    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);

    }

    public Assignment? Read(int id)
    {
        // Load the list of assignments from the XML file
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);

        // Find the assignment with the specified ID
        Assignment? assignment = Assignments.FirstOrDefault(a => a.Id == id);

        // If not found, throw an exception
        if (assignment == null)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");

        // Return the found assignment
        return assignment;
        }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        // Load the list of assignments from the XML file
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);

        // Find the first assignment matching the filter
        Assignment? assignment = Assignments.FirstOrDefault(filter);

        // If no match is found, throw an exception
        if (assignment == null)
            throw new DalDoesNotExistException("No assignment matching the given filter was found");

        // Return the matching assignment
        return assignment;
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        // Load the list of assignments from the XML file
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);

        // Apply the filter if provided, otherwise return all assignments
        return filter == null ? Assignments : Assignments.Where(filter);
    }

    public void Update(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
}
