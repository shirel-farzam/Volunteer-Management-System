namespace BlImplementation;
using BlApi;
using BO;

using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

    private readonly DalApi.IDal _dal = DalApi.Factory.Get; // Access DAL instance

   
    public BO.Role Login(int username, string password)
    {
            // Call the original implementation of Login
            return VolunteerManager.LoginInternal(username, password);
    }
    
    public IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive, BO.VolunteerInListField? sortField = null, CallType? callType = null)
    {
            // Call the internal implementation
            return VolunteerManager.ReadAllInternal(isActive, sortField, callType);
    }
    
    public BO.Volunteer Read(int volunteerId)
    {
        // Call the internal implementation
        return VolunteerManager.ReadInternal(volunteerId);
    }
   
    public void Update(int volunteerId, BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        // Call the internal implementation
        VolunteerManager.UpdateInternal(volunteerId, boVolunteer);
    }
    
    // Adds a new volunteer with validations
    public void DeleteVolunteer(int volunteerId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        // Call the internal implementation
        VolunteerManager.DeleteVolunteerInternal(volunteerId);
    }
    public bool CanDelete(int id)
    {
        var volunteer = VolunteerManager.ReadInternal(id);
        return (volunteer.CurrentCall==null)&&(volunteer.TotalHandledCalls==0);
    }
    public void AddVolunteer(BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        // Call the internal implementation
        VolunteerManager.AddVolunteerInternal(boVolunteer);
    }

}