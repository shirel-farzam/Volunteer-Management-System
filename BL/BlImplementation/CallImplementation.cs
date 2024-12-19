namespace BlImplementation;
using BlApi;
using BO;
using Helpers;


internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddCall(BO.Call boCall)
    {
        CallManager.IsValideCall(boCall);
        CallManager.IsLogicCall(boCall);
        DO.Call doCall = CallManager.BOConvertDO_Call(boCall);
        try
        {
            _dal.Call.Create(doCall); 
        }
        catch (Exception ex)
        {
            throw new BO.BlWrongInputException("Failed to add the new call.", ex);
        }
    }
    public void ChooseCallForTreat(int volunteerId, int callId)
    {
        DO.Volunteer vol = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlNullPropertyException($"there is no volunteer with this ID {volunteerId}");
        BO.Call bocall = Read(callId) ?? throw new BO.BlNullPropertyException($"there is no call with this ID {callId}");
        if (bocall.Status == BO.CallStatus.Open || bocall.Status == BO.CallStatus.Expired)
            throw new BO.BlAlreadyExistsException($"the call is open or expired callId is={callId}");
        DO.Assignment assignmentToCreate = new DO.Assignment
        {
            Id = 0,
            CallId = callId,
            VolunteerId = volunteerId,
            TimeStart = ClockManager.Now,
            TimeEnd = null,
            TypeEndTreat = null
        };
        try
        {
            _dal.Assignment.Create(assignmentToCreate);
        }
        catch (DO.DalDeletionImpossible)
        { throw new BO.BlAlreadyExistsException("impossible to create"); }

    }

    public void DeleteCall(int callId)
    {

        try
        {
            if (Read(callId).Status == BO.CallStatus.Open)
            {
                _dal.Call.Delete(callId);
                return;
            }
            throw new BO.BlDeleteNotPossibleException($"Call with id={callId} can not be deleted");
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with id={callId} does not exist");
        }
    }

    public int[] CountCall()
    {
        var calls = _dal.Call.ReadAll();

        // קיבוץ לפי סוג סיום טיפול מתוך Assignment
        var groupedCalls = calls
            .SelectMany(call => _dal.Assignment.ReadAll(ass => ass.CallId == call.Id))  
            .GroupBy(ass => (int)ass.TypeEndTreat) 
            .ToDictionary(group => group.Key, group => group.Count()); 

        int maxTypeEnd = groupedCalls.Keys.Any() ? groupedCalls.Keys.Max() : 0;

        int[] result = new int[maxTypeEnd + 1];

        foreach (var kvp in groupedCalls)
        {
            result[kvp.Key] = kvp.Value;
        }

        return result;
    }
    public BO.Call Read(int callId)
    {
        {
            Func<DO.Assignment, bool> func = item => item.CallId == callId;
            IEnumerable<DO.Assignment> dataOfAssignments = _dal.Assignment.ReadAll(func);


            var doCall = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist");
            return new()
            {
                Id = callId,
                Type = (BO.CallType)doCall.Type,
                Description = doCall.Description,
                FullAddress = doCall.FullAddress,
                Latitude = doCall.Latitude,
                Longitude = doCall.Longitude,
                OpenTime = doCall.TimeOpened,
                MaxEndTime = doCall.MaxTimeToClose,
                // Status = CallManager.GetCallStatus(dataOfAssignments, doCall.MaxTimeToClose),
                Status = CallManager.GetCallStatus(doCall),
                CallAssignments = dataOfAssignments.Select(assign => new BO.CallAssignmentInList
                {
                    VolunteerId = assign.VolunteerId,
                    VolunteerName = _dal.Volunteer.Read(callId).FullName,
                    StartTime = assign.TimeStart,
                    EndTime = assign.TimeEnd,
                    CompletionType = (BO.AssignmentCompletionType)assign.TypeEndTreat,
                }).ToList()
            };
            throw new Exception();

        }
    }

    public IEnumerable<BO.CallInList> GetCallInLists(BO.CallInListField? filter, object? obj, BO.CallInListField? sortBy)
    {
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new BO.BlNullPropertyException("There are not calls int database");
        IEnumerable<BO.CallInList> boCallsInList = _dal.Call.ReadAll().Select(call => CallManager.ConvertDOCallToBOCallInList(call));
        if (filter != null && obj != null)
        {
            switch (filter)
            {
                case BO.CallInListField.Id:
                    boCallsInList.Where(item => item.Id == (int)obj).Select(item => item);
                    break;

                case BO.CallInListField.CallId:
                    boCallsInList.Where(item => item.CallId == (int)obj).Select(item => item);
                    break;

                case BO.CallInListField.Type:
                    boCallsInList.Where(item => item.Type == (BO.CallType)obj).Select(item => item);
                    break;

                case BO.CallInListField.OpeningTime:
                    boCallsInList.Where(item => item.OpeningTime == (DateTime)obj).Select(item => item);
                    break;

                case BO.CallInListField.TimeToFinish:
                    boCallsInList.Where(item => item.TimeToFinish == (TimeSpan)obj).Select(item => item);
                    break;

                case BO.CallInListField.LastVolunteerName:
                    boCallsInList.Where(item => item.LastVolunteerName == (string)obj).Select(item => item);
                    break;

                case BO.CallInListField.TreatmentDuration:
                    boCallsInList.Where(item => item.TreatmentDuration == (TimeSpan)obj).Select(item => item);
                    break;

                case BO.CallInListField.Status:
                    boCallsInList.Where(item => item.Status == (BO.CallStatus)obj).Select(item => item);
                    break;

                case BO.CallInListField.TotalAssignments:
                    boCallsInList.Where(item => item.TotalAssignments == (int)obj).Select(item => item);
                    break;
            }
        }
        if (sortBy == null)
            sortBy = BO.CallInListField.CallId;
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
        return boCallsInList;
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? type = null, BO.ClosedCallInListField? sortField = null)
    {

        IEnumerable<DO.Call> previousCalls = _dal.Call.ReadAll(null);
        List<BO.ClosedCallInList>? Calls = new List<BO.ClosedCallInList>();

        Calls.AddRange(from item in previousCalls
                       let DataCall = Read(item.Id)
                       where DataCall.Status == BO.CallStatus.Closed && DataCall.CallAssignments?.Any() == true
                       let lastAssugnment = DataCall.CallAssignments.OrderBy(c => c.StartTime).Last()
                       select CallManager.ConvertDOCallToBOCloseCallInList(item, lastAssugnment));
        IEnumerable<BO.ClosedCallInList>? closedCallInLists = Calls.Where(call => call.Id == volunteerId);
        if (type != null)
        {
            closedCallInLists.Where(c => c.CallType == type).Select(c => c);
        }
        if (sortField == null)
        {
            sortField = BO.ClosedCallInListField.Id;
        }
        switch (sortField)
        {
            case BO.ClosedCallInListField.Id:
                closedCallInLists.OrderBy(item => item.Id);
                break;
            case BO.ClosedCallInListField.CallType:
                closedCallInLists.OrderBy(item => item.CallType);
                break;
            case BO.ClosedCallInListField.FullAddress:
                closedCallInLists.OrderBy(item => item.FullAddress);
                break;
            case BO.ClosedCallInListField.OpeningTime:
                closedCallInLists.OrderBy(item => item.OpeningTime);
                break;
            case BO.ClosedCallInListField.EntryTime:
                closedCallInLists.OrderBy(item => item.EntryTime);
                break;
            case BO.ClosedCallInListField.CompletionTime:
                closedCallInLists.OrderBy(item => item.CompletionTime);
                break;
            case BO.ClosedCallInListField.CompletionType:
                closedCallInLists.OrderBy(item => item.CompletionType);
                break;

        }
        return closedCallInLists;

    }
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
        if (assigmnetToCancel.TypeEndTreat != null || (_dal.Call.Read(assigmnetToCancel.CallId).MaxTimeToClose > ClockManager.Now) | assigmnetToCancel.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assignment not open or expaierd");

        DO.Assignment assigmnetToUP = new DO.Assignment
        {
            Id = assigmnetToCancel.Id,
            CallId = assigmnetToCancel.CallId,
            VolunteerId = assigmnetToCancel.VolunteerId,
            TimeStart = assigmnetToCancel.TimeStart,
            TimeEnd = ClockManager.Now,
            TypeEndTreat = ismanager ? DO.TypeEnd.ManagerCancel : DO.TypeEnd.SelfCancel,
        };
        try
        {
            _dal.Assignment.Update(assigmnetToUP);
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
        if (assigmnetToCancel.TypeEndTreat != null || (_dal.Call.Read(assigmnetToCancel.CallId).MaxTimeToClose > ClockManager.Now) | assigmnetToCancel.TimeEnd != null)
            throw new BO.BlDeleteNotPossibleException("The assignment not open or expaired");

        DO.Assignment assigmnetToUP = new DO.Assignment
        {
            Id = assigmnetToCancel.Id,
            CallId = assigmnetToCancel.CallId,
            VolunteerId = assigmnetToCancel.VolunteerId,
            TimeStart = assigmnetToCancel.TimeStart,
            TimeEnd = ClockManager.Now,
            TypeEndTreat = ismanager ? DO.TypeEnd.ManagerCancel : DO.TypeEnd.SelfCancel,
        };
        try
        {
            _dal.Assignment.Update(assigmnetToUP);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlDeleteNotPossibleException("cannot delete in DO");
        }
    }

    public void CloseTreat(int idVol, int idAssig)
    {
        throw new NotImplementedException();
    }

    public void Update(Call boCall)
    {
        double[] coordinate = VolunteerManager.GetCoordinates(boCall.FullAddress);
        double latitude = coordinate[0];
        double longitude = coordinate[1];

        boCall.Latitude = latitude;
        boCall.Longitude = longitude;

        CallManager.IsLogicCall(boCall);

        DO.Call doCall = new
                    (
                    boCall.Id,
                    (DO.CallType)boCall.Type,
                    boCall.Description,
                    boCall.FullAddress,
                    boCall.Latitude ?? 0.0, 
                    boCall.Longitude ?? 0.0, 
                    boCall.OpenTime,
                    boCall.MaxEndTime
                    );
        try
        {
            _dal.Call.Update(doCall);
        }
        catch (DO.DalDeletionImpossible ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={boCall.Id} does Not exist", ex);
        }
    }
}
