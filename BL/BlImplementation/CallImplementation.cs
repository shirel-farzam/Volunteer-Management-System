namespace BlImplementation;
using BlApi;
using BO;
using Helpers;

internal class CallImplementation : ICall
{
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

    private readonly DalApi.IDal _dal = DalApi.Factory.Get; // Dependency to access the data access layer (DAL)

    public void AddCall(BO.Call boCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        double[] coordinate = VolunteerManager.GetCoordinatesFromAddress(boCall.FullAddress);
        double latitude = coordinate[0];
        double longitude = coordinate[1];
        boCall.Latitude = latitude;
        boCall.Longitude = longitude;
        try
        {
            CallManager.IsLogicCall(boCall);
            DO.Call doCall = new
                (
                boCall.Id,
                (DO.CallType)boCall.Type,
                boCall.Description,
                boCall.FullAddress,
                latitude,
                longitude,
                boCall.OpenTime,
                boCall.MaxEndTime
                );
            lock (AdminManager.BlMutex) // stage 7
                _dal.Call.Create(doCall);
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlAlreadyExistsException($"Call with ID={boCall.Id} already exists", ex);
        }
        CallManager.Observers.NotifyListUpdated();  //stage 5 
    }

   // public void AddCall(BO.Call boCall)
   // {
   //     // we need add 
   //     boCall.Latitude = Tools.GetLatitude(boCall.FullAddress);
   //     boCall.Longitude = Tools.GetLongitude(boCall.FullAddress);

   //     //boCall.Status = CallManager.CalculateCallStatus();
   //     //boCall.CallAssignments = null; // for first time not have CallAssignments

   //     CallManager.IsValideCall(boCall);
   //     CallManager.IsLogicCall(boCall);

   //     //var doCall = CallManager.BOConvertDO_Call(boCall.Id);
   //     //DO.Call doCall = CallManager.BOConvertDO_Call(boCall); // Converts the business object to a data object
   //     try
   //     {
   //         DO.Call doCall = new DO.Call(
   //     boCall.Id,
   //     (DO.CallType)boCall.Type,
   //    boCall.Description,
   //    boCall.FullAddress,
   //   boCall.Latitude ?? 0.0,
   // boCall.Longitude ?? 0.0,          // Longitude

   //    boCall.OpenTime,            // OpeningTime
   //    boCall.MaxEndTime

   //);


   //         _dal.Call.Create(doCall);
   //         CallManager.Observers.NotifyListUpdated(); //stage 5   

   //     }
   //     catch (DO.DalAlreadyExistsException ex)
   //     {
   //         throw new BO.BlAlreadyExistsException($"Call with ID={boCall.Id} already exists", ex);
   //     }

   // }


    // Assigns a volunteer to treat a specific call and creates an assignment record.
    public void ChooseCallForTreat(int volunteerId, int callId)
    {
        DO.Volunteer vol;
        lock (AdminManager.BlMutex) // stage 7
            vol = _dal.Volunteer.Read(volunteerId)
                           ?? throw new BO.BlNullPropertyException($"There is no volunteer with this ID {volunteerId}");

        // Reads the call from the business logic or throws an exception if not found
        BO.Call bocall = Read(callId)
                         ?? throw new BO.BlNullPropertyException($"There is no call with this ID {callId}");

        // Ensures the call is not open or expired before assigning a volunteer
        if (bocall.Status != BO.CallStatus.Open || bocall.Status == BO.CallStatus.OpenRisk)
            throw new BO.BlAlreadyExistsException($"The call is open or expired. Call ID is={callId}");

        // Creates a new assignment record for the volunteer and call
        DO.Assignment assignmentToCreate = new DO.Assignment
        {
            Id = 0,
            CallId = callId,
            VolunteerId = volunteerId,
            TimeStart = AdminManager.Now, // Sets the start time for the assignment
            TimeEnd = null,               // No end time initially
            TypeEndTreat = null           // No completion type initially
        };

        try
        {
            lock (AdminManager.BlMutex) {// stage 7
                _dal.Assignment.Create(assignmentToCreate);
        } // Adds the assignment to the DAL
            CallManager.Observers.NotifyListUpdated(); //stage 5
            CallManager.Observers.NotifyItemUpdated(volunteerId);
            //VolunteerManager.Observers.NotifyListUpdated();
            //VolunteerManager.Observers.NotifyItemUpdated(assignmentToCreate.CallId);
        }
        catch (DO.DalDeletionImpossible)
        {
            throw new BO.BlAlreadyExistsException("Impossible to create assignment"); // Handles creation failure
        }
    }
    public void DeleteCall(int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        try
        {
            // Try to fetch the call and check its status
            var call = Read(callId);
            if (call.Status != BO.CallStatus.Open)
            {
                throw new BO.BlDeleteNotPossibleException($"Call with ID={callId} cannot be deleted because its status is '{call.Status}' and not 'Open'.");
            }

            // Delete the call
            lock (AdminManager.BlMutex)
            { // stage 7
                _dal.Call.Delete(callId);
            }
            CallManager.Observers.NotifyListUpdated(); // Notify observers
        }
        catch (DO.DalDoesNotExistException ex) // Correct exception name
        {
            // Handle case where the call does not exist in the data layer
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            // General exception handling for unexpected cases
            throw new BO.BLException("An error occurred while trying to delete the call.", ex);
        }
    }

    public int[] CountCall()
    {
        //var calls = _dal.Call.ReadAll(); // Read all calls from the data layer

        //    // Group assignments by their completion type (`TypeEndTreat`) across all calls
        //    var groupedCalls = calls
        //        .SelectMany(call => _dal.Assignment.ReadAll(ass => ass.CallId == call.Id)) // Get all assignments for each call
        //        .GroupBy(ass => (int)ass.TypeEndTreat) // Group assignments by `TypeEndTreat`
        //        .ToDictionary(group => group.Key, group => group.Count()); // Convert to dictionary: Key = TypeEndTreat, Value = Count

        //    // Determine the highest `TypeEndTreat` value for array size
        //    int maxTypeEnd = groupedCalls.Keys.Any() ? groupedCalls.Keys.Max() : 0;

        //    // Initialize the result array with the appropriate size
        //    int[] result = new int[maxTypeEnd + 1];

        //    // Populate the result array with counts for each completion type
        //    foreach (var kvp in groupedCalls)
        //    {
        //        result[kvp.Key] = kvp.Value;
        //    }

        //    return result; // Return the array with counts
        //}

        try
        {
            IEnumerable<DO.Call>? doCalls;
            lock (AdminManager.BlMutex) { //stage 7
                                          // Fetching calls from the data layer
                doCalls = _dal.Call.ReadAll();
            }

                // Converting the calls from DO to BO using your function
                var boCalls = doCalls.Select(doCall => CallManager.GetViewingCall(doCall.Id)).ToList();

                // Grouping the calls by status and counting occurrences
                var groupedCalls = boCalls
                    .GroupBy(call => call.Status)
                    .ToDictionary(group => (int)group.Key, group => group.Count());

                // Creating the result array with specific order and summing at the last position
                var quantities = new int[7];
                quantities[0] = groupedCalls.ContainsKey((int)CallStatus.Open) ? groupedCalls[(int)CallStatus.Open] : 0;
                quantities[1] = groupedCalls.ContainsKey((int)CallStatus.Closed) ? groupedCalls[(int)CallStatus.Closed] : 0;
                quantities[2] = groupedCalls.ContainsKey((int)CallStatus.InProgress) ? groupedCalls[(int)CallStatus.InProgress] : 0;
                quantities[3] = groupedCalls.ContainsKey((int)CallStatus.Expired) ? groupedCalls[(int)CallStatus.Expired] : 0;
                quantities[4] = groupedCalls.ContainsKey((int)CallStatus.InProgressRisk) ? groupedCalls[(int)CallStatus.InProgressRisk] : 0;
                quantities[5] = groupedCalls.ContainsKey((int)CallStatus.OpenRisk) ? groupedCalls[(int)CallStatus.OpenRisk] : 0;

                // Summing up all values into the last position
                quantities[6] = quantities.Take(6).Sum();

                return quantities;
                
        }
        catch (Exception ex)
        {
            throw new BlDoesNotExistException("Failed to retrieve call quantities by status.", ex);
        }
    }



    public BO.Call Read(int callId)
    {

        // Define a filter function to get assignments related to the specified call
        Func<DO.Assignment, bool> func = item => item.CallId == callId;
        IEnumerable<DO.Assignment> dataOfAssignments;
        lock (AdminManager.BlMutex) // stage 7
            dataOfAssignments = _dal.Assignment.ReadAll(func); // Fetch the related assignments

        // Read the call from the data layer or throw an exception if not found
        DO.Call? doCall;
        lock (AdminManager.BlMutex) // stage 7
            doCall = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist");

        // Map the data object (DO.Call) to a business object (BO.Call)
        return new BO.Call
        {
            Id = callId,

            Type = (BO.CallType)doCall.Type, // Map the call type
            Description = doCall.Description, // Map the description
            FullAddress = doCall.FullAddress, // Map the address
            Latitude = doCall.Latitude, // Map the latitude
            Longitude = doCall.Longitude, // Map the longitude
            OpenTime = doCall.TimeOpened, // Map the time the call was opened
            MaxEndTime = doCall.MaxTimeToClose, // Map the maximum time to close the call
            Status = CallManager.GetCallStatus(doCall), // Determine the call's status
            CallAssignments = dataOfAssignments.Any()
                ? dataOfAssignments.Select(assign => new BO.CallAssignmentInList
                {
                    VolunteerId = assign.VolunteerId, // Map the volunteer ID
                    VolunteerName = _dal.Volunteer.Read(assign.VolunteerId)?.FullName ?? "Unknown Volunteer", // Map the volunteer's name
                    StartTime = assign.TimeStart, // Map the assignment start time
                    EndTime = assign.TimeEnd, // Map the assignment end time
                    CompletionType = assign.TypeEndTreat.HasValue
                        ? (BO.AssignmentCompletionType)assign.TypeEndTreat.Value
                        : null, // Map the completion type, handling nulls
                }).ToList()
                : null,
        };
    }


    //public IEnumerable<BO.CallInList> GetCallInLists(BO.CallInListField? filter, object? obj, BO.CallInListField? sortBy)
    //{
    //    // Fetch all calls from the data layer, throw an exception if no calls exist in the database
    //    IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are no calls in the database");

    //    // Convert the data-layer calls (DO.Call) to business-layer call lists (BO.CallInList)
    //    IEnumerable<BO.CallInList> boCallsInList = _dal.Call.ReadAll()
    //        .Select(call => CallManager.ConvertDOCallToBOCallInList(call));

    //    // Apply filtering logic if a filter and a filter object are provided
    //    if (filter != null && obj != null)
    //    {
    //        switch (filter)
    //        {
    //            case BO.CallInListField.Id:
    //                // Filter by ID
    //                boCallsInList = boCallsInList.Where(item => item.Id == (int)obj);
    //                break;

    //            case BO.CallInListField.CallId:
    //                // Filter by CallId
    //                boCallsInList = boCallsInList.Where(item => item.CallId == (int)obj);
    //                break;

    //            case BO.CallInListField.Type:
    //                // Filter by call type
    //                boCallsInList = boCallsInList.Where(item => item.Type == (BO.CallType)obj);
    //                break;

    //            case BO.CallInListField.OpeningTime:
    //                // Filter by opening time
    //                boCallsInList = boCallsInList.Where(item => item.OpeningTime == (DateTime)obj);
    //                break;

    //            case BO.CallInListField.TimeToFinish:
    //                // Filter by time to finish
    //                boCallsInList = boCallsInList.Where(item => item.TimeToFinish == (TimeSpan)obj);
    //                break;

    //            case BO.CallInListField.LastVolunteerName:
    //                // Filter by last volunteer name
    //                boCallsInList = boCallsInList.Where(item => item.LastVolunteerName == (string)obj);
    //                break;

    //            case BO.CallInListField.TreatmentDuration:
    //                // Filter by treatment duration
    //                boCallsInList = boCallsInList.Where(item => item.TreatmentDuration == (TimeSpan)obj);
    //                break;

    //            case BO.CallInListField.Status:
    //                // Filter by call status
    //                boCallsInList = boCallsInList.Where(item => item.Status == (BO.CallStatus)obj);
    //                break;

    //            case BO.CallInListField.TotalAssignments:
    //                // Filter by total assignments
    //                boCallsInList = boCallsInList.Where(item => item.TotalAssignments == (int)obj);
    //                break;
    //        }
    //    }

    //    // Default sorting field if no sort field is provided
    //    if (sortBy == null)
    //        sortBy = BO.CallInListField.CallId;

    //    // Apply sorting based on the specified sort field
    //    switch (sortBy)
    //    {
    //        case BO.CallInListField.Id:
    //            // Sort by ID, prioritizing null values
    //            boCallsInList = boCallsInList.OrderBy(item => item.Id.HasValue ? 0 : 1)
    //                                         .ThenBy(item => item.Id)
    //                                         .ToList();
    //            break;

    //        case BO.CallInListField.CallId:
    //            // Sort by CallId
    //            boCallsInList = boCallsInList.OrderBy(item => item.CallId).ToList();
    //            break;

    //        case BO.CallInListField.Type:
    //            // Sort by call type
    //            boCallsInList = boCallsInList.OrderBy(item => item.Type).ToList();
    //            break;

    //        case BO.CallInListField.OpeningTime:
    //            // Sort by opening time
    //            boCallsInList = boCallsInList.OrderBy(item => item.OpeningTime).ToList();
    //            break;

    //        case BO.CallInListField.TimeToFinish:
    //            // Sort by time to finish
    //            boCallsInList = boCallsInList.OrderBy(item => item.TimeToFinish).ToList();
    //            break;

    //        case BO.CallInListField.LastVolunteerName:
    //            // Sort by last volunteer name
    //            boCallsInList = boCallsInList.OrderBy(item => item.LastVolunteerName).ToList();
    //            break;

    //        case BO.CallInListField.TreatmentDuration:
    //            // Sort by treatment duration
    //            boCallsInList = boCallsInList.OrderBy(item => item.TreatmentDuration).ToList();
    //            break;

    //        case BO.CallInListField.Status:
    //            // Sort by call status
    //            boCallsInList = boCallsInList.OrderBy(item => item.Status).ToList();
    //            break;

    //        case BO.CallInListField.TotalAssignments:
    //            // Sort by total assignments
    //            boCallsInList = boCallsInList.OrderBy(item => item.TotalAssignments).ToList();
    //            break;
    //    }

    //    return boCallsInList;
    //}
    public IEnumerable<BO.CallInList> GetCallInLists(BO.CallInListField? filter, object? obj, BO.CallInListField? sortBy)
{
    // Retrieve all calls from the database, or throw an exception if none exist.
    IEnumerable<DO.Call> calls;

    lock (AdminManager.BlMutex) // stage 7
    {
        calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are no calls in the database");
    }

    // Convert all DO calls to BO calls in list.
    IEnumerable<BO.CallInList> boCallsInList = calls.Select(call => CallManager.ConvertDOCallToBOCallInList(call)).ToList();

    // Apply filter if specified.
    if (filter != null && obj != null)
    {
        switch (filter)
        {
            case BO.CallInListField.Id:
                boCallsInList = boCallsInList.Where(item => item.Id == (int)obj);
                break;
            case BO.CallInListField.CallId:
                boCallsInList = boCallsInList.Where(item => item.CallId == (int)obj);
                break;
            case BO.CallInListField.Type:
                boCallsInList = boCallsInList.Where(item => item.Type == (BO.CallType)obj);
                break;
            case BO.CallInListField.OpeningTime:
                boCallsInList = boCallsInList.Where(item => item.OpeningTime == (DateTime)obj);
                break;
            case BO.CallInListField.TimeToFinish:
                boCallsInList = boCallsInList.Where(item => item.TimeToFinish == (TimeSpan)obj);
                break;
            case BO.CallInListField.LastVolunteerName:
                boCallsInList = boCallsInList.Where(item => item.LastVolunteerName == (string)obj);
                break;
            case BO.CallInListField.TreatmentDuration:
                boCallsInList = boCallsInList.Where(item => item.TreatmentDuration == (TimeSpan)obj);
                break;
            case BO.CallInListField.Status:
                if ((BO.CallStatus)obj == BO.CallStatus.None)
                    break;
                boCallsInList = boCallsInList.Where(item => item.Status == (BO.CallStatus)obj);
                break;
            case BO.CallInListField.TotalAssignments:
                boCallsInList = boCallsInList.Where(item => item.TotalAssignments == (int)obj);
                break;
        }
    }

    // Default sort by CallId if no sorting is specified.
    if (sortBy == null)
        sortBy = BO.CallInListField.CallId;

    // Apply sorting based on the specified field.
    switch (sortBy)
    {
        case BO.CallInListField.Id:
            boCallsInList = boCallsInList.OrderBy(item => item.Id.HasValue ? 0 : 1)
                                         .ThenBy(item => item.Id)
                                         .ToList();
            break;
        case BO.CallInListField.CallId:
            boCallsInList = boCallsInList.OrderBy(item => item.CallId).ToList();
            break;
        case BO.CallInListField.Type:
            boCallsInList = boCallsInList.OrderBy(item => item.Type).ToList();
            break;
        case BO.CallInListField.OpeningTime:
            boCallsInList = boCallsInList.OrderBy(item => item.OpeningTime).ToList();
            break;
        case BO.CallInListField.TimeToFinish:
            boCallsInList = boCallsInList.OrderBy(item => item.TimeToFinish).ToList();
            break;
        case BO.CallInListField.LastVolunteerName:
            boCallsInList = boCallsInList.OrderBy(item => item.LastVolunteerName).ToList();
            break;
        case BO.CallInListField.TreatmentDuration:
            boCallsInList = boCallsInList.OrderBy(item => item.TreatmentDuration).ToList();
            break;
        case BO.CallInListField.Status:
            boCallsInList = boCallsInList.OrderBy(item => item.Status).ToList();
            break;
        case BO.CallInListField.TotalAssignments:
            boCallsInList = boCallsInList.OrderBy(item => item.TotalAssignments).ToList();
            break;
    }

    // Return the filtered and sorted list of calls.
    return boCallsInList;
}


    //public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? type = null, BO.ClosedCallInListField? sortField = null)
    //{

    //    IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
    //    List<BO.ClosedCallInList>? Calls = new List<BO.ClosedCallInList>();

    //    Calls.AddRange(from item in previousCalls
    //                   let DataCall = Read(item.Id)
    //                   where DataCall.Status == BO.CallStatus.Closed && DataCall.CallAssignments?.Any() == true
    //                   let lastAssugnment = DataCall.CallAssignments.OrderBy(c => c.StartTime).Last()
    //                   select CallManager.ConvertDOCallToBOCloseCallInList(item, lastAssugnment));
    //    IEnumerable<BO.ClosedCallInList>? closedCallInLists = Calls.Where(call => call.Id == volunteerId);
    //    if (type != null)
    //    {
    //        closedCallInLists.Where(c => c.CallType == type).Select(c => c);
    //    }
    //    if (sortField == null)
    //    {
    //        sortField = BO.ClosedCallInListField.Id;
    //    }
    //    switch (sortField)
    //    {
    //        case BO.ClosedCallInListField.Id:
    //            closedCallInLists.OrderBy(item => item.Id);
    //            break;
    //        case BO.ClosedCallInListField.CallType:
    //            closedCallInLists.OrderBy(item => item.CallType);
    //            break;
    //        case BO.ClosedCallInListField.FullAddress:
    //            closedCallInLists.OrderBy(item => item.FullAddress);
    //            break;
    //        case BO.ClosedCallInListField.OpeningTime:
    //            closedCallInLists.OrderBy(item => item.OpeningTime);
    //            break;
    //        case BO.ClosedCallInListField.EntryTime:
    //            closedCallInLists.OrderBy(item => item.EntryTime);
    //            break;
    //        case BO.ClosedCallInListField.CompletionTime:
    //            closedCallInLists.OrderBy(item => item.CompletionTime);
    //            break;
    //        case BO.ClosedCallInListField.CompletionType:
    //            closedCallInLists.OrderBy(item => item.CompletionType);
    //            break;

    //    }
    //    return closedCallInLists;

    //}
    //public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? type = null, BO.ClosedCallInListField? sortField = null)
    //{
    //    // קריאה ל-API
    //    IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
    //    List<BO.ClosedCallInList> Calls = new List<BO.ClosedCallInList>();

    //    Calls.AddRange(from item in previousCalls
    //                   let DataCall = Read(item.Id)
    //                   where DataCall.Status == BO.CallStatus.Closed && DataCall.CallAssignments?.Any() == true
    //                   let lastAssignment = DataCall.CallAssignments.OrderBy(c => c.StartTime).Last()
    //                   select CallManager.ConvertDOCallToBOCloseCallInList(item, lastAssignment));

    //    // סינון לפי VolunteerId
    //    IEnumerable<BO.ClosedCallInList> closedCallInLists = Calls.Where(call => call.Id == volunteerId);

    //    // סינון לפי סוג קריאה
    //    if (type != null)
    //    {
    //        closedCallInLists = closedCallInLists.Where(c => c.CallType == type);
    //    }

    //    // מיון
    //    if (sortField == null)
    //    {
    //        sortField = BO.ClosedCallInListField.Id; // ברירת מחדל
    //    }

    //    switch (sortField)
    //    {
    //        case BO.ClosedCallInListField.Id:
    //            closedCallInLists = closedCallInLists.OrderBy(item => item.Id);
    //            break;
    //        case BO.ClosedCallInListField.CallType:
    //            closedCallInLists = closedCallInLists.OrderBy(item => item.CallType);
    //            break;
    //        case BO.ClosedCallInListField.FullAddress:
    //            closedCallInLists = closedCallInLists.OrderBy(item => item.FullAddress);
    //            break;
    //        case BO.ClosedCallInListField.OpeningTime:
    //            closedCallInLists = closedCallInLists.OrderBy(item => item.OpeningTime);
    //            break;
    //        case BO.ClosedCallInListField.EntryTime:
    //            closedCallInLists = closedCallInLists.OrderBy(item => item.EntryTime);
    //            break;
    //        case BO.ClosedCallInListField.CompletionTime:
    //            closedCallInLists = closedCallInLists.OrderBy(item => item.CompletionTime);
    //            break;
    //        case BO.ClosedCallInListField.CompletionType:
    //            closedCallInLists = closedCallInLists.OrderBy(item => item.CompletionType);
    //            break;
    //    }

    //    return closedCallInLists.ToList();
    //}

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? type = null, BO.OpenCallInListField? sortField = null)
    {
        DO.Volunteer boVolunteer;
        lock (AdminManager.BlMutex) // stage 7
            boVolunteer = _dal.Volunteer.Read(volunteerId);
        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        IEnumerable<BO.OpenCallInList>? openCallInLists = new List<BO.OpenCallInList>();
    
        openCallInLists = from item in previousCalls
                let DataCall = Read(item.Id)
                where DataCall.Status == BO.CallStatus.Open || DataCall.Status == BO.CallStatus.OpenRisk
                //let lastAssugnment = DataCall.CallAssignments.OrderBy(c => c.StartTime).Last()
                select /*CallManager.ConvertDOCallToBOOpenCallInList(item, volunteerId);*/
                        new BO.OpenCallInList
                        {
                            Id = DataCall.Id,
                            CallType = (BO.CallType)DataCall.Type,
                            Description = DataCall.Description,
                            FullAddress = DataCall.FullAddress,
                            OpeningTime = DataCall.OpenTime,
                            MaxCompletionTime = DataCall.MaxEndTime,
                            DistanceFromVolunteer = VolunteerManager.CalculateDistance((double)DataCall.Latitude, (double)DataCall.Longitude, (double)boVolunteer.Latitude, (double)boVolunteer.Longitude)
                        };
        ;

        //openCallInLists = openCallInLists.Where(call => call.Id == volunteerId);
        if (type != null)
        {
            openCallInLists.Where(c => c.CallType == type).Select(c => c);
        }
        if (sortField == null)
        {
            sortField = BO.OpenCallInListField.Id;
        }
        switch (sortField)
        {
            case BO.OpenCallInListField.Id:
                openCallInLists.OrderBy(item => item.Id);
                break;
            case BO.OpenCallInListField.CallType:
                openCallInLists.OrderBy(item => item.CallType);
                break;
            case BO.OpenCallInListField.FullAddress:
                openCallInLists.OrderBy(item => item.FullAddress);
                break;
            case BO.OpenCallInListField.OpeningTime:
                openCallInLists.OrderBy(item => item.OpeningTime);
                break;
            case BO.OpenCallInListField.MaxCompletionTime:
                openCallInLists.OrderBy(item => item.MaxCompletionTime);
                break;
            case BO.OpenCallInListField.DistanceFromVolunteer:
                openCallInLists.OrderBy(item => item.DistanceFromVolunteer);
                break;

        }

        return openCallInLists;

    }

    public void UpdateTreatmentCancellation(int volunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        DO.Assignment assigmnetToCancel;
        // עטיפת קריאות ה DAL ב lock
        lock (AdminManager.BlMutex) // stage 7
        
            assigmnetToCancel = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDeleteNotPossibleException("there is no assignment with this ID");
            bool ismanager = false;

            if (assigmnetToCancel.VolunteerId != volunteerId)
            {
                if (_dal.Volunteer.Read(volunteerId).Job == DO.Role.Manager)
                    ismanager = true;
                else throw new BO.BlDeleteNotPossibleException("the volunteer is not manager or not in this call");
            }

            if (assigmnetToCancel.TypeEndTreat != null || (_dal.Call.Read(assigmnetToCancel.CallId).MaxTimeToClose > AdminManager.Now) | assigmnetToCancel.TimeEnd != null)
                throw new BO.BlDeleteNotPossibleException("The assignment not open or expired");

            DO.Assignment assigmnetToUP = new DO.Assignment
            {
                Id = assigmnetToCancel.Id,
                CallId = assigmnetToCancel.CallId,
                VolunteerId = assigmnetToCancel.VolunteerId,
                TimeStart = assigmnetToCancel.TimeStart,
                TimeEnd = AdminManager.Now,
                TypeEndTreat = ismanager ? DO.TypeEnd.ManagerCancel : DO.TypeEnd.SelfCancel,
            };
        lock (AdminManager.BlMutex)
        {// stage 7
            // ביצוע העדכון ב DAL בתוך ה lock
            _dal.Assignment.Update(assigmnetToUP);
        }
        

        // הוצאת ה Notifications מחוץ ל lock
        CallManager.Observers.NotifyItemUpdated(assignmentId);  //stage 5
        CallManager.Observers.NotifyListUpdated();  //stage 5
    }

    public void CancelTreat(int idVol, int idAssig)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  // stage 7
        DO.Assignment assigmnetToCancel;
        lock (AdminManager.BlMutex) // stage 7
            assigmnetToCancel = _dal.Assignment.Read(idAssig) ?? throw new BO.BlDeleteNotPossibleException("there is no assignment with this ID");
            bool ismanager = false;

            if (assigmnetToCancel.VolunteerId != idVol)
            {
                if (_dal.Volunteer.Read(idVol).Job == DO.Role.Manager)
                    ismanager = true;
                else throw new BO.BlDeleteNotPossibleException("the volunteer is not manager or not in this call");
            }

            if (assigmnetToCancel.TypeEndTreat != null || assigmnetToCancel.TimeEnd != null)
                throw new BO.BlDeleteNotPossibleException("The assignment not open or expired");

            DO.Assignment assigmnetToUP = new DO.Assignment
            {
                Id = assigmnetToCancel.Id,
                CallId = assigmnetToCancel.CallId,
                VolunteerId = assigmnetToCancel.VolunteerId,
                TimeStart = assigmnetToCancel.TimeStart,
                TimeEnd = AdminManager.Now,
                TypeEndTreat = ismanager ? DO.TypeEnd.ManagerCancel : DO.TypeEnd.SelfCancel,
            };
        lock (AdminManager.BlMutex)
        {// stage 7
            // עדכון ה- Assignment ב- DAL בתוך ה- lock
            _dal.Assignment.Update(assigmnetToUP);
        }
        

        // Notifications מחוץ ל- lock
        CallManager.Observers.NotifyItemUpdated(idAssig);  // stage 5
        CallManager.Observers.NotifyListUpdated();  // stage 5
        VolunteerManager.Observers.NotifyListUpdated();
        VolunteerManager.Observers.NotifyItemUpdated(idVol);
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int id, BO.CallType? type, BO.ClosedCallInListField? sortBy)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  // stage 7

        IEnumerable<BO.ClosedCallInList> filteredCalls;
        IEnumerable<DO.Call> allCalls;
        lock (AdminManager.BlMutex) // stage 7
        
            // Retrieve all calls from the DAL within the lock
            allCalls = _dal.Call.ReadAll();
       IEnumerable<DO.Assignment>? allAssignments;
            // Retrieve all assignments from the DAL within the lock
        allAssignments = _dal.Assignment.ReadAll();

            // Filter by volunteer ID and closed status (calls that have an end treatment type)
            filteredCalls = from call in allCalls
                            join assignment in allAssignments
                            on call.Id equals assignment.CallId
                            where assignment.VolunteerId == id && assignment.TypeEndTreat != null
                            select new BO.ClosedCallInList
                            {
                                Id = call.Id,
                                CallType = (BO.CallType)call.Type,
                                FullAddress = call.FullAddress,
                                OpeningTime = call.TimeOpened,
                                EntryTime = assignment.TimeStart,
                                CompletionTime = assignment.TimeEnd,
                                CompletionType = (BO.AssignmentCompletionType)assignment.TypeEndTreat
                            };
        

        // Filter by call type if provided
        if (type.HasValue)
        {
            filteredCalls = filteredCalls.Where(c => c.CallType == type.Value);
        }

        // Sort by the requested field or by default (call ID)
        if (sortBy.HasValue)
        {
            filteredCalls = sortBy.Value switch
            {
                BO.ClosedCallInListField.Id => filteredCalls.OrderBy(c => c.Id),
                BO.ClosedCallInListField.CallType => filteredCalls.OrderBy(c => c.CallType),
                BO.ClosedCallInListField.FullAddress => filteredCalls.OrderBy(c => c.FullAddress),
                BO.ClosedCallInListField.OpeningTime => filteredCalls.OrderBy(c => c.OpeningTime),
                BO.ClosedCallInListField.EntryTime => filteredCalls.OrderBy(c => c.EntryTime),
                BO.ClosedCallInListField.CompletionTime => filteredCalls.OrderBy(c => c.CompletionTime),
                BO.ClosedCallInListField.CompletionType => filteredCalls.OrderBy(c => c.CompletionType),
                _ => filteredCalls.OrderBy(c => c.Id)
            };
        }
        else
        {
            filteredCalls = filteredCalls.OrderBy(c => c.Id);
        }

        return filteredCalls;
    }

    public void CloseTreat(int idVol, int idAssig)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  // stage 7
        DO.Assignment assigmnetToClose;
        lock (AdminManager.BlMutex) // stage 7
        
            assigmnetToClose = _dal.Assignment.Read(idAssig) ?? throw new BO.BlDeleteNotPossibleException("there is no assignment with this ID");
            if (assigmnetToClose.VolunteerId != idVol)
            {
                throw new BO.BlWrongInputException("the volunteer is not treat in this assignment");
            }
            if (assigmnetToClose.TypeEndTreat != null || assigmnetToClose.TimeEnd != null)
                throw new BO.BlDeleteNotPossibleException("The assignment not open");

            DO.Assignment assignmentToUP = new DO.Assignment
            {
                Id = assigmnetToClose.Id,
                CallId = assigmnetToClose.CallId,
                VolunteerId = assigmnetToClose.VolunteerId,
                TimeStart = assigmnetToClose.TimeStart,
                TimeEnd = AdminManager.Now,
                TypeEndTreat = DO.TypeEnd.Treated,
            };

            try
            {
                _dal.Assignment.Update(assignmentToUP);
            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw new BO.BlDeleteNotPossibleException("cannot update in DO");
            }
        
        // Notifications outside the lock
        CallManager.Observers.NotifyItemUpdated(idAssig);  // stage 5
        CallManager.Observers.NotifyListUpdated();  // stage 5
        VolunteerManager.Observers.NotifyListUpdated();
        VolunteerManager.Observers.NotifyItemUpdated(idVol);
    }

    public void Update(BO.Call boCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        // Get coordinates (latitude and longitude) for the provided address using the VolunteerManager utility.
        double[] coordinate = VolunteerManager.GetCoordinatesFromAddress(boCall.FullAddress);
        double latitude = coordinate[0];
        double longitude = coordinate[1];

        // Assign the calculated coordinates to the call object, handling nullable types.
        boCall.Latitude = latitude;
        boCall.Longitude = longitude;

        // Perform logic validation on the call object to ensure it complies with business rules.
        CallManager.IsLogicCall(boCall);

        // Convert the BO.Call object to a DO.Call object for data layer processing.
        DO.Call doCall = new
                    (
                    boCall.Id, // Call ID
                    (DO.CallType)boCall.Type, // Convert call type from BO to DO
                    boCall.Description, // Description of the call
                    boCall.FullAddress, // Full address of the call
                    boCall.Latitude ?? 0.0, // Default latitude if null
                    boCall.Longitude ?? 0.0, // Default longitude if null
                    boCall.OpenTime, // The time the call was opened
                    boCall.MaxEndTime // The maximum time allowed for the call to close
                    );
        try
        {
            // Update the call in the data layer.
            lock (AdminManager.BlMutex) // stage 7
                _dal.Call.Update(doCall);
            CallManager.Observers.NotifyItemUpdated(doCall.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (DO.DalDeletionImpossible ex)
        {
            // Handle exception if the call does not exist, wrapping it in a business logic exception.
            throw new BO.BlDoesNotExistException($"Call with ID={boCall.Id} does Not exist", ex);
        }
    }

}