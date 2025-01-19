namespace BlImplementation;
using BlApi;
using BO;
using DO;
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

    // Adds a new call to the system after validating it and converting it to the appropriate format.
    //public void AddCall(BO.Call boCall)
    //{
    //    if (boCall == null)
    //    {
    //        throw new ArgumentNullException(nameof(boCall), "The provided call cannot be null.");
    //    }

    //    CallManager.IsValideCall(boCall); // Validates the call's properties
    //    CallManager.IsLogicCall(boCall); // Ensures the call follows logical business rules

    //    DO.Call doCall = CallManager.BOConvertDO_Call(boCall); // Converts the business object to a data object

    //    try
    //    {
    //        _dal.Call.Create(doCall); // Adds the call to the DAL
    //        CallManager.Observers.NotifyListUpdated(); // Notify observers about the updated list
    //    }
    //    catch (Exception ex)
    //    {
    //        // Log the exception (optional, if you have logging in place)
    //        // Logger.LogError("Failed to add the new call.", ex);

    //        // Throws a custom exception to handle the error gracefully
    //        throw new BO.BlWrongInputException("Failed to add the new call.", ex);
    //    }
    //}

    public void AddCall(BO.Call boCall)
    {
        // we need add 
        boCall.Latitude = Tools.GetLatitude(boCall.FullAddress);
        boCall.Longitude = Tools.GetLongitude(boCall.FullAddress);

        //boCall.Status = CallManager.CalculateCallStatus();
        //boCall.CallAssignments = null; // for first time not have CallAssignments

        CallManager.IsValideCall(boCall);
        CallManager.IsLogicCall(boCall);

        //var doCall = CallManager.BOConvertDO_Call(boCall.Id);
        //DO.Call doCall = CallManager.BOConvertDO_Call(boCall); // Converts the business object to a data object
        try
        {
            DO.Call doCall = new DO.Call(
        boCall.Id,
        (DO.CallType)boCall.Type,
       boCall.Description,
       boCall.FullAddress,
      boCall.Latitude ?? 0.0,
    boCall.Longitude ?? 0.0,          // Longitude

       boCall.OpenTime,            // OpeningTime
       boCall.MaxEndTime

   );


            _dal.Call.Create(doCall);
            CallManager.Observers.NotifyListUpdated(); //stage 5   

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Call with ID={boCall.Id} already exists", ex);
        }

    }


    // Assigns a volunteer to treat a specific call and creates an assignment record.
    public void ChooseCallForTreat(int volunteerId, int callId)
    {
        // Reads the volunteer from the data layer or throws an exception if not found
        DO.Volunteer vol = _dal.Volunteer.Read(volunteerId)
                           ?? throw new BO.BlNullPropertyException($"There is no volunteer with this ID {volunteerId}");

        // Reads the call from the business logic or throws an exception if not found
        BO.Call bocall = Read(callId)
                         ?? throw new BO.BlNullPropertyException($"There is no call with this ID {callId}");

        // Ensures the call is not open or expired before assigning a volunteer
        if (bocall.Status == BO.CallStatus.Open || bocall.Status == BO.CallStatus.Expired)
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
            _dal.Assignment.Create(assignmentToCreate); // Adds the assignment to the DAL
            CallManager.Observers.NotifyListUpdated(); //stage 5   
        }
        catch (DO.DalDeletionImpossible)
        {
            throw new BO.BlAlreadyExistsException("Impossible to create assignment"); // Handles creation failure
        }
    }
    public void DeleteCall(int callId)
    {
        try
        {
            // Check if the call can be deleted (only calls with "Open" status can be deleted)
            if (Read(callId).Status == BO.CallStatus.Open)
            {
                _dal.Call.Delete(callId); // Delete the call from the data layer
                CallManager.Observers.NotifyListUpdated(); //stage 5   
                return;
            }
            // Throw exception if the call cannot be deleted
            throw new BO.BlDeleteNotPossibleException($"Call with id={callId} cannot be deleted");
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            // Handle the case where the call does not exist in the data layer
            throw new BO.BlDoesNotExistException($"Call with id={callId} does not exist");
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
            // Fetching calls from the data layer
            var doCalls = _dal.Call.ReadAll();

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
        IEnumerable<DO.Assignment> dataOfAssignments = _dal.Assignment.ReadAll(func); // Fetch the related assignments

        // Read the call from the data layer or throw an exception if not found
        var doCall = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist");

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


    public IEnumerable<BO.CallInList> GetCallInLists(BO.CallInListField? filter, object? obj, BO.CallInListField? sortBy)
    {
        // Fetch all calls from the data layer, throw an exception if no calls exist in the database
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are no calls in the database");

        // Convert the data-layer calls (DO.Call) to business-layer call lists (BO.CallInList)
        IEnumerable<BO.CallInList> boCallsInList = _dal.Call.ReadAll()
            .Select(call => CallManager.ConvertDOCallToBOCallInList(call));

        // Apply filtering logic if a filter and a filter object are provided
        if (filter != null && obj != null)
        {
            switch (filter)
            {
                case BO.CallInListField.Id:
                    // Filter by ID
                    boCallsInList = boCallsInList.Where(item => item.Id == (int)obj);
                    break;

                case BO.CallInListField.CallId:
                    // Filter by CallId
                    boCallsInList = boCallsInList.Where(item => item.CallId == (int)obj);
                    break;

                case BO.CallInListField.Type:
                    // Filter by call type
                    boCallsInList = boCallsInList.Where(item => item.Type == (BO.CallType)obj);
                    break;

                case BO.CallInListField.OpeningTime:
                    // Filter by opening time
                    boCallsInList = boCallsInList.Where(item => item.OpeningTime == (DateTime)obj);
                    break;

                case BO.CallInListField.TimeToFinish:
                    // Filter by time to finish
                    boCallsInList = boCallsInList.Where(item => item.TimeToFinish == (TimeSpan)obj);
                    break;

                case BO.CallInListField.LastVolunteerName:
                    // Filter by last volunteer name
                    boCallsInList = boCallsInList.Where(item => item.LastVolunteerName == (string)obj);
                    break;

                case BO.CallInListField.TreatmentDuration:
                    // Filter by treatment duration
                    boCallsInList = boCallsInList.Where(item => item.TreatmentDuration == (TimeSpan)obj);
                    break;

                case BO.CallInListField.Status:
                    // Filter by call status
                    boCallsInList = boCallsInList.Where(item => item.Status == (BO.CallStatus)obj);
                    break;

                case BO.CallInListField.TotalAssignments:
                    // Filter by total assignments
                    boCallsInList = boCallsInList.Where(item => item.TotalAssignments == (int)obj);
                    break;
            }
        }

        // Default sorting field if no sort field is provided
        if (sortBy == null)
            sortBy = BO.CallInListField.CallId;

        // Apply sorting based on the specified sort field
        switch (sortBy)
        {
            case BO.CallInListField.Id:
                // Sort by ID, prioritizing null values
                boCallsInList = boCallsInList.OrderBy(item => item.Id.HasValue ? 0 : 1)
                                             .ThenBy(item => item.Id)
                                             .ToList();
                break;

            case BO.CallInListField.CallId:
                // Sort by CallId
                boCallsInList = boCallsInList.OrderBy(item => item.CallId).ToList();
                break;

            case BO.CallInListField.Type:
                // Sort by call type
                boCallsInList = boCallsInList.OrderBy(item => item.Type).ToList();
                break;

            case BO.CallInListField.OpeningTime:
                // Sort by opening time
                boCallsInList = boCallsInList.OrderBy(item => item.OpeningTime).ToList();
                break;

            case BO.CallInListField.TimeToFinish:
                // Sort by time to finish
                boCallsInList = boCallsInList.OrderBy(item => item.TimeToFinish).ToList();
                break;

            case BO.CallInListField.LastVolunteerName:
                // Sort by last volunteer name
                boCallsInList = boCallsInList.OrderBy(item => item.LastVolunteerName).ToList();
                break;

            case BO.CallInListField.TreatmentDuration:
                // Sort by treatment duration
                boCallsInList = boCallsInList.OrderBy(item => item.TreatmentDuration).ToList();
                break;

            case BO.CallInListField.Status:
                // Sort by call status
                boCallsInList = boCallsInList.OrderBy(item => item.Status).ToList();
                break;

            case BO.CallInListField.TotalAssignments:
                // Sort by total assignments
                boCallsInList = boCallsInList.OrderBy(item => item.TotalAssignments).ToList();
                break;
        }

        return boCallsInList;
    }
    //public IEnumerable<BO.OpenCallInList> GetOpenCall(int id, BO.CallType? type, BO.OpenCallInList? sortBy)
    //{
    //    if (type == BO.CallType.None)
    //        type = null;
    //    DO.Volunteer volunteer = _dal.Volunteer.Read(id);
    //    if (volunteer == null)
    //        throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

    //    // Retrieve all calls from the BO
    //    IEnumerable<BO.CallInList> allCalls = GetCallInLists(null, null, null);

    //    // Retrieve all assignments from the DAL
    //    var calls = _dal.Call.ReadAll();
    //    double lonVol = (double)volunteer.Longitude;
    //    double latVol = (double)volunteer.Latitude;

    //    // Filter for only "Open" or "Risk Open" status
    //    IEnumerable<BO.OpenCallInList> filteredCalls = from call in allCalls
    //                                                   where (call.Status == BO.CallStatus.Open || call.Status == BO.CallStatus.OpenRisk)
    //                                                   let boCall = Read(call.CallId)
    //                                                   select new BO.OpenCallInList
    //                                                   {
    //                                                       Id = call.CallId,
    //                                                       CallType = call.Type,
    //                                                       Description = boCall.Description,
    //                                                       FullAddress = boCall.FullAddress,
    //                                                       OpeningTime = call.OpeningTime,
    //                                                       MaxCompletionTime = boCall.MaxEndTime,
    //                                                       Status = boCall.Status,
    //                                                       DistanceFromVolunteer = volunteer?.FullAddress != null ?
    //                                                      VolunteerManager.CalculateDistance(latVol, lonVol, (double)boCall.Latitude, (double)boCall.Longitude)/* : 0*/  // Calculate the distance between the volunteer and the call


    //                                                  };
    //    filteredCalls = from call in filteredCalls
    //                    where (volunteer.MaxReading == null || volunteer.MaxReading > call.DistanceFromVolunteer)
    //                    select call;


    //    // Filter by call type if provided
    //    if (type.HasValue)
    //    {
    //        filteredCalls = filteredCalls.Where(c => c.CallType == type.Value);
    //    }

    //    // Sort by the requested field or by default (call ID)
    //    if (sortBy.HasValue)
    //    {
    //        filteredCalls = sortBy.Value switch
    //        {
    //            BO.OpenCallInListField.Id => filteredCalls.OrderBy(c => c.Id),
    //            BO.OpenCallInListField.CallType => filteredCalls.OrderBy(c => c.CallType),
    //            //BO.EOpenCallInList.Description => filteredCalls.OrderBy(c => c.Description),
    //            BO.OpenCallInListField.FullAddress => filteredCalls.OrderBy(c => c.FullAddress),
    //            BO.OpenCallInListField.OpeningTime => filteredCalls.OrderBy(c => c.OpeningTime),
    //            BO.OpenCallInListField.MaxCompletionTime => filteredCalls.OrderBy(c => c.MaxCompletionTime),
    //            BO.OpenCallInListField.DistanceFromVolunteer => filteredCalls.OrderBy(c => c.DistanceFromVolunteer),

    //            _ => filteredCalls.OrderBy(c => c.Id)
    //        };
    //    }
    //    else
    //    {
    //        filteredCalls = filteredCalls.OrderBy(c => c.Id);
    //    }

    //    return filteredCalls;
    //}
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

        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        List<BO.OpenCallInList>? Calls = new List<BO.OpenCallInList>();

        Calls.AddRange(from item in previousCalls
                       let DataCall = Read(item.Id)
                       where DataCall.Status == BO.CallStatus.Open || DataCall.Status == BO.CallStatus.OpenRisk
                       let lastAssugnment = DataCall.CallAssignments.OrderBy(c => c.StartTime).Last()
                       select CallManager.ConvertDOCallToBOOpenCallInList(item, volunteerId));
        IEnumerable<BO.OpenCallInList>? openCallInLists = Calls.Where(call => call.Id == volunteerId);
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


        DO.Assignment assigmnetToCancel = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDeleteNotPossibleException("there is no assignment with this ID");
        bool ismanager = false;
        if (assigmnetToCancel.VolunteerId != volunteerId)
        {
            if (_dal.Volunteer.Read(volunteerId).Job == DO.Role.Manager)
                ismanager = true;
            else throw new BO.BlDeleteNotPossibleException("the volunteer is not manager or not in this call");
        }
        if (assigmnetToCancel.TypeEndTreat != null || (_dal.Call.Read(assigmnetToCancel.CallId).MaxTimeToClose > AdminManager.Now) | assigmnetToCancel.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assignment not open or expaierd");

        DO.Assignment assigmnetToUP = new DO.Assignment
        {
            Id = assigmnetToCancel.Id,
            CallId = assigmnetToCancel.CallId,
            VolunteerId = assigmnetToCancel.VolunteerId,
            TimeStart = assigmnetToCancel.TimeStart,
            TimeEnd = AdminManager.Now,
            TypeEndTreat = ismanager ? DO.TypeEnd.ManagerCancel : DO.TypeEnd.SelfCancel,
        };
        try
        {
            _dal.Assignment.Update(assigmnetToUP);
            CallManager.Observers.NotifyItemUpdated(assigmnetToUP.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5




        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDeleteNotPossibleException("cannot delete in DO");
        }


    }

    public void CancelTreat(int idVol, int idAssig)
    {
        DO.Assignment assigmnetToCancel = _dal.Assignment.Read(idAssig) ?? throw new BO.BlDeleteNotPossibleException("there is no assigment with this ID");
        bool ismanager = false;
        if (assigmnetToCancel.VolunteerId != idVol)
        {
            if (_dal.Volunteer.Read(idVol).Job == DO.Role.Manager)
                ismanager = true;
            else throw new BO.BlDeleteNotPossibleException("the volunteer is not manager or not in this call");
        }
        if (assigmnetToCancel.TypeEndTreat != null || (_dal.Call.Read(assigmnetToCancel.CallId).MaxTimeToClose > AdminManager.Now) | assigmnetToCancel.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assignment not open or expaired");

        DO.Assignment assigmnetToUP = new DO.Assignment
        {
            Id = assigmnetToCancel.Id,
            CallId = assigmnetToCancel.CallId,
            VolunteerId = assigmnetToCancel.VolunteerId,
            TimeStart = assigmnetToCancel.TimeStart,
            TimeEnd = AdminManager.Now,
            TypeEndTreat = ismanager ? DO.TypeEnd.ManagerCancel : DO.TypeEnd.SelfCancel,
        };
        try
        {
            _dal.Assignment.Update(assigmnetToUP);
            CallManager.Observers.NotifyItemUpdated(assigmnetToUP.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlDeleteNotPossibleException("cannot delete in DO");
        }
    }
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int id, BO.CallType? type, BO.ClosedCallInListField? sortBy)
    {
        // Retrieve all calls from the DAL
        var allCalls = _dal.Call.ReadAll();

        // Retrieve all assignments from the DAL
        var allAssignments = _dal.Assignment.ReadAll();

        // Filter by volunteer ID and closed status (calls that have an end treatment type)
        IEnumerable<BO.ClosedCallInList> filteredCalls = from call in allCalls
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
        {
            DO.Assignment assigmnetToClose = _dal.Assignment.Read(idAssig) ?? throw new BO.BlDeleteNotPossibleException("there is no assigment with this ID");
            if (assigmnetToClose.VolunteerId != idVol)
            {
                throw new BO.BlWrongInputException("the volunteer is not treat in this assignment");
            }
            BO.Call bocall = Read(assigmnetToClose.CallId);
            if (assigmnetToClose.TypeEndTreat != null || (bocall.Status != BO.CallStatus.Open && bocall.Status != BO.CallStatus.OpenRisk) || assigmnetToClose.TimeEnd != null)
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
                CallManager.Observers.NotifyItemUpdated(assignmentToUP.Id);  //stage 5
                CallManager.Observers.NotifyListUpdated();  //stage 5

            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw new BO.BlDeleteNotPossibleException("cannot update in DO");
            }
        }

    }

    public void Update(BO.Call boCall)
    {
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

