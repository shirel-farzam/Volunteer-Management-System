using BO;
using DalApi;
using DO;
using System.Text.RegularExpressions;

using Helpers;
internal static class VolunteerManager
{
    private static IDal _dal = Factory.Get; //stage 4
    internal static void CheckFormat(BO.Volunteer boVolunteer)
    {
        try
        {
            CheckPhonnumber(boVolunteer.PhoneNumber);
            CheckEmail(boVolunteer.Email);
            Tools.IsAddressValid(boVolunteer.FullAddress);
        }
        catch (BO.BlWrongItemtException ex)
        {
            throw new BO.BlWrongItemtException($"the item have logic problem", ex);
        }

    }
    
    internal static void CheckLogic(BO.Volunteer boVolunteer, BO.Volunteer existingVolunteer, bool isManager)
    {
        // Validate ID
        if (!IsValidIsraeliID(boVolunteer.Id))
        {
            throw new BO.Incompatible_ID("Invalid ID: The ID does not pass validation.");
        }

        // Check if the Job was changed
        if (boVolunteer.Job != existingVolunteer.Job)
        {
            if (!isManager)
            {
                throw new BO.BlPermissionException("Only a manager is authorized to update the volunteer's Job.");
            }
        }

        // Check if the password was changed
        if (boVolunteer.Password != existingVolunteer.Password && !isManager)
        {
            throw new BO.BlIncorrectPasswordException("Only the volunteer or a manager can update the password.");
        }

        // Check if the active status was changed
        if (boVolunteer.Active != existingVolunteer.Active)
        {
            if (!isManager)
            {
                throw new BO.BlGeneralException("Only a manager is authorized to change the active status.");
            }
        }

        // Add additional checks here for other properties if necessary
    }
    internal static void CheckLogic(BO.Volunteer boVolunteer, BO.Volunteer existingVolunteer, int requesterId, DO.Job requesterJob)
    {
        // Check if the Job has changed - only a manager can update the volunteer's Job
        if (boVolunteer.Job != existingVolunteer.Job)
        {
            if (requesterJob != DO.Role.Manager)
            {
                throw new BO.BlPermissionException("Only a manager is authorized to update the volunteer's Job.");
            }
        }

        // Check if the password has changed - only the volunteer or a manager can update the password
        if (boVolunteer.Password != existingVolunteer.Password)
        {
            if (requesterId != boVolunteer.Id && requesterJob != DO.Role.Manager)
            {
                throw new BO.BlPermissionException("Only the volunteer or a manager can update the password.");
            }
        }

        // Check if the active status has changed - only a manager can change the active status
        if (boVolunteer.Active != existingVolunteer.Active)
        {
            if (requesterJob != DO.Role.Manager)
            {
                throw new BO.BlPermissionException("Only a manager is authorized to change the active status.");
            }
        }

        // Add additional checks if there are any other restricted fields
    }
    internal static void CheckPassword(string password)
    {
        // דרישות בסיסיות
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("הסיסמה לא יכולה להיות ריקה או null.");
        }

        // אורך מקסימלי של 5 תווים
        if (password.Length > 5)
        {
            throw new ArgumentException("הסיסמה לא יכולה להכיל יותר מ-5 תווים.");
        }

        // לפחות אות אחת גדולה
        if (!password.Any(char.IsUpper))
        {
            throw new ArgumentException("הסיסמה חייבת לכלול לפחות אות אחת גדולה.");
        }

        // לפחות אות אחת קטנה
        if (!password.Any(char.IsLower))
        {
            throw new ArgumentException("הסיסמה חייבת לכלול לפחות אות אחת קטנה.");
        }

        // לפחות מספר אחד
        if (!password.Any(char.IsDigit))
        {
            throw new ArgumentException("הסיסמה חייבת לכלול לפחות מספר אחד.");
        }

        // לפחות סימן מיוחד
        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            throw new ArgumentException("הסיסמה חייבת לכלול לפחות סימן מיוחד (למשל: !, @, #, $, וכו').");
        }
    }


    public static bool IsValidIsraeliID(int id)
    {
        // Convert the integer to a string to validate length
        string idStr = id.ToString();

        // Ensure the ID is exactly 9 digits
        if (idStr.Length != 9)
            return false;

        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            // Extract each digit
            int digit = idStr[i] - '0'; // Convert character to integer

            // Multiply alternately by 1 or 2
            int multiplied = digit * (i % 2 + 1);

            // If the result is greater than 9, sum its digits (e.g., 18 -> 1 + 8)
            sum += (multiplied > 9) ? multiplied - 9 : multiplied;
        }

        // The ID is valid if the total sum is divisible by 10
        return sum % 10 == 0;
    }

    internal static void CheckPhonnumber(string PhoneNumber)
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber) || !Regex.IsMatch(PhoneNumber, @"^0\d{9}$"))
        {
            throw new ArgumentException("PhoneNumber must be a 10-digit number starting with 0.");
        }
    }

    internal static void CheckEmail(string Email)
    {
        if (!Regex.IsMatch(Email, @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z](([\.\-]?)(?![\.\-])))[0-9a-zA-Z]@))([0-9a-zA-Z][\-0-9a-zA-Z][0-9a-zA-Z]\.)+[a-zA-Z]{2,}$"))
        {
            throw new ArgumentException("Invalid Email format.");
        }

    }

    public static BO.Volunteer GetVolunteer(int id)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(id) ?? throw new BlDoesNotExistException("Error: Volunteer ID not found.");

        return new BO.Volunteer
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.FullName,
            PhoneNumber = doVolunteer.PhoneNumber,
            Email = doVolunteer.Email,
            FullAddress = doVolunteer.FullAddress,
            Password = doVolunteer.Password,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            Job = (BO.Role)doVolunteer.Job, // Enum conversion between DO and BO
            Active = doVolunteer.Active,
            TypeDistance = (BO.Distance)doVolunteer.D, // Assuming `D` is a DistanceType enum in DO
            Distance = (BO.DistanceType)doVolunteer.TypeDistance, // Assuming this is another property in DO

            // Calculating the totals for the volunteer
            TotalHandledCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.TimeEnd == AssignmentCompletionType.TreatedOnTime),
            TotalExpiredCalls = _dal.Assignment.ReadAll().Count(a => a.VolunteerId == doVolunteer.Id && a.TimeEnd == AssignmentCompletionType.Expired),

            // Getting current call
            CurrentCall = CallManager.GetCallInProgress(doVolunteer.Id), // Retrieve the current active call for the volunteer
        };
    }


    public static DO.Volunteer BOconvertDO(BO.Volunteer Volunteer)
    {
        return new DO.Volunteer
        {
            Id = Volunteer.Id,
            FullName = Volunteer.FullName,
            PhoneNumber = Volunteer.PhoneNumber,
            Email = Volunteer.Email,
            Password = Volunteer.Password,
            Job = (DO.Role)Volunteer.Job,
            TypeDistance = (DO.Distance)Volunteer.TypeDistance,
            Active = Volunteer.Active,
            FullAddress = Volunteer.FullAddress,
            Latitude = Volunteer.Latitude,
            Longitude = Volunteer.Longitude
        };
    }


    // GetVolunteerInList and helper methods for each field
    public static BO.VolunteerInList GetVolunteerInList(int VolunteerId)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("Error: Volunteer ID not found.");

        var doAssignment = _dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == VolunteerId && a.TimeEnd == null);
        if (doAssignment == null)
        {
            throw new BlDoesNotExistException("No active assignment found for this volunteer.");
        }

        var doCall = _dal.Call.ReadAll().FirstOrDefault(c => c.Id == doAssignment.CallId);
        if (doCall == null)
        {
            throw new BlDoesNotExistException("No call found for this assignment.");
        }

        return new BO.VolunteerInList
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.FullName,
            Active = doVolunteer.Active,
            TotalCallsHandled = Tools.TotalHandledCalls(VolunteerId),
            TotalCallsCanceled = Tools.TotalCallsCancelledhelp(VolunteerId),
            TotalCallsExpired = Tools.TotalCallsExpiredelo(VolunteerId),
            CurrentCallId = Tools.CurrentCallIdhelp(VolunteerId),
            CurrentCallType = Tools.CurrentCallType(VolunteerId)
        };
    }

    // GetClosedCallInList 
    public static BO.ClosedCallInList GetClosedCallInList(int VolunteerId)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("Error ID");

        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.TimeEnd == null).FirstOrDefault();
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

        return new BO.ClosedCallInList
        {
            Id = doAssignment.Id,
            CallType = (BO.CallType)doCall.Type,
            FullAddress = doCall.FullAddress,
            OpeningTime = doCall.TimeOpened, // Using the OpeningTime from DO.Call
            EntryTime = doAssignment.TimeStart,
            CompletionTime = doAssignment.TimeEnd,
            CompletionType = (BO.AssignmentCompletionType?)doAssignment.TypeEndTreat
        };
    }


    //GetOpenCallInList
    public static BO.OpenCallInList GetOpenCallInList(int VolunteerId)
    {
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(VolunteerId) ?? throw new BlDoesNotExistException("Error ID");

        var doAssignment = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == VolunteerId && a.TimeEnd == null).FirstOrDefault();
        var doCall = _dal.Call.ReadAll().Where(c => c.Id == doAssignment!.CallId).FirstOrDefault();

        if (Tools.IsAddressValid(doVolunteer.FullAddress).Result == false)
        {
            throw new BlInvalidaddress("Invalid address of Volunteer");
        }
        double LatitudeVolunteer = Tools.GetLatitudeAsync(doVolunteer.FullAddress).Result;
        double LongitudeVolunteer = Tools.GetLongitudeAsync(doVolunteer.FullAddress).Result;

        return new BO.OpenCallInList
        {
            Id = doAssignment.Id,
            CallType = (BO.CallType)doCall.Type,
            FullAddress = doCall.FullAddress,
            OpeningTime = doCall.TimeOpened,
            MaxCompletionTime = doCall.MaxTimeToClose,
            DistanceFromVolunteer = CallManager.CalculateAirDistance(doCall.Latitude, doCall.Longitude, LatitudeVolunteer, LongitudeVolunteer),
        };
    }
}