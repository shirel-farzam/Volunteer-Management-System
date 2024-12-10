using BO;
using DalApi;
using System.Text.RegularExpressions;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static BO.VolunteerInList ConvertDOToBOInList(DO.Volunteer doVolunteer)
    {
        // שליפת הקריאות הקשורות למתנדב
        var assignments = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();

        // חישוב מספר הקריאות שטופלו
        int sumCalls = assignments.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated);

        // חישוב מספר הקריאות שבוטלו
        int sumCanceled = assignments.Count(ass => ass.TypeEndTreat == DO.TypeEnd.SelfCancel);

        // חישוב מספר הקריאות שזמן הטיפול בהן פג
        int sumExpired = assignments.Count(ass => ass.TypeEndTreat == DO.TypeEnd.ExpiredCancel);

        // חישוב הקריאות שנמצאות בהמתנה (לא הסתיימו)
        int? currentCallId = assignments.Count(ass => ass.TimeEnd == null);

        // יצירת אובייקט BO.VolunteerInList והחזרת המידע המומר
        return new BO.VolunteerInList
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.FullName,
            Active = doVolunteer.Active,
            TotalCallsHandled = sumCalls,
            TotalCallsCanceled = sumCanceled,
            TotalCallsExpired = sumExpired
        };
    }
    internal static void CheckId(int id)
    {
        // Convert the integer ID to a string to process individual digits
        string idString = id.ToString();

        // Ensure the ID is exactly 9 digits long
        if (idString.Length != 9)
        {
            throw new BO.BlWrongItemtException($"this ID {id} does not possible");
        }

        int sum = 0;

        // Iterate through each digit in the ID
        for (int i = 0; i < 9; i++)
        {
            // Convert the character to its numeric value
            int digit = idString[i] - '0';

            // Determine the multiplier: 1 for odd positions, 2 for even positions
            int multiplier = (i % 2) + 1;

            // Multiply the digit by the multiplier
            int product = digit * multiplier;

            // If the result is two digits, sum the digits (e.g., 14 -> 1 + 4)
            if (product > 9)
            {
                product = product / 10 + product % 10;
            }

            // Add the processed digit to the total sum
            sum += product;
        }

        // תעודת זהות תקינה אם סכום ספרות הביקורת מתחלק ב-10
        if (sum % 10 != 0)
        {
            throw new BO.BlWrongItemtException($"this ID {id} does not possible");
        }
    }
    internal static void CheckFormat(BO.Volunteer boVolunteer)
    {
        /// <summary>
        /// Validate the ID of the volunteer.
        /// The ID must be a positive integer and consist of 8 to 9 digits.
        /// </summary>
        if (boVolunteer.Id <= 0 || boVolunteer.Id.ToString().Length < 8 || boVolunteer.Id.ToString().Length > 9)
        {
            throw new ArgumentException("Invalid ID. It must be 8-9 digits.");
        }

        /// <summary>
        /// Validate the FullName field.
        /// The name must not be null, empty, or consist of only whitespace.
        /// </summary>
        if (string.IsNullOrWhiteSpace(boVolunteer.FullName))
        {
            throw new ArgumentException("FullName cannot be null or empty.");
        }

        /// <summary>
        /// Validate the PhoneNumber field.
        /// The phone number must be exactly 10 digits and start with 0.
        /// </summary>
        if (!Regex.IsMatch(boVolunteer.PhoneNumber, @"^0\d{9}$"))
        {
            throw new ArgumentException("PhoneNumber must be 10 digits and start with 0.");
        }

        /// <summary>
        /// Validate the Email field.
        /// The email must match the standard email format.
        /// </summary>
        if (!Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("Invalid Email format.");
        }

        /// <summary>
        /// Validate the MaxReading field.
        /// If provided, it must be a positive number.
        /// </summary>
        if (boVolunteer.MaxReading.HasValue && boVolunteer.MaxReading.Value <= 0)
        {
            throw new ArgumentException("MaxReading must be a positive number.");
        }

        /// <summary>
        /// Validate the Latitude field.
        /// If provided, it must be between -90 and 90 (inclusive).
        /// </summary>
        if (boVolunteer.Latitude.HasValue && (boVolunteer.Latitude.Value < -90 || boVolunteer.Latitude.Value > 90))
        {
            throw new ArgumentException("Latitude must be between -90 and 90.");
        }

        /// <summary>
        /// Validate the Longitude field.
        /// If provided, it must be between -180 and 180 (inclusive).
        /// </summary>
        if (boVolunteer.Longitude.HasValue && (boVolunteer.Longitude.Value < -180 || boVolunteer.Longitude.Value > 180))
        {
            throw new ArgumentException("Longitude must be between -180 and 180.");
        }

        /// <summary>
        /// Add any additional validation checks here if needed in the future.
        /// </summary>
    }

    internal static void CheckAddress(BO.Volunteer boVolunteer)
    {
        // בדיקה אם הכתובת ריקה
        if (string.IsNullOrWhiteSpace(boVolunteer.FullAddress))
            throw new BO.BlWrongItemtException("Address cannot be empty.");

        // בדיקת תקינות קואורדינטות (אם נדרשות)
        if (boVolunteer.Latitude.HasValue && (boVolunteer.Latitude < -90 || boVolunteer.Latitude > 90))
            throw new BO.BlWrongItemtException("Latitude must be between -90 and 90.");

        if (boVolunteer.Longitude.HasValue && (boVolunteer.Longitude < -180 || boVolunteer.Longitude > 180))
            throw new BO.BlWrongItemtException("Longitude must be between -180 and 180.");

        // בדיקה נוספת (אופציונלית) אם רוצים לוודא שהכתובת כוללת פרטים מסוימים (לדוגמה: רחוב, עיר וכו')
        var addressParts = boVolunteer.FullAddress.Split(',');
        if (addressParts.Length < 2)
            throw new BO.BlWrongItemtException("Address must include both street and city.");

        // וידוא שכל חלק בכתובת אינו ריק
        foreach (var part in addressParts)
        {
            if (string.IsNullOrWhiteSpace(part))
                throw new BO.BlWrongItemtException("Address contains empty parts. Please provide full details.");
        }
    }
    internal static void CheckPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new BO.BlWrongItemtException("Phone number cannot be empty.");

        // בדיקה שמספר הטלפון מכיל רק ספרות ואורכו מתאים
        if (!Regex.IsMatch(phoneNumber, @"^\d{10}$"))
            throw new BO.BlWrongItemtException("Phone number must be exactly 10 digits.");

        // בדיקת תקינות קידומת (למשל בישראל)
        if (!phoneNumber.StartsWith("05"))
            throw new BO.BlWrongItemtException("Phone number must start with a valid prefix (e.g., 05 for Israel).");
    }
    internal static void CheckEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BO.BlWrongItemtException("Email cannot be empty.");

        // שימוש ב-Regex לבדיקת תקינות של כתובת דוא"ל
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new BO.BlWrongItemtException("Email format is invalid. Please provide a valid email address.");
    }
internal static void CheckPassword(string? password)
{
    if (password == null)
        throw new BO.BlWrongItemtException("Password cannot be null.");

    // בדיקה שאורך הסיסמה הוא לפחות 8 תווים
    if (password.Length < 8)
        throw new BO.BlWrongItemtException("Password must be at least 8 characters long.");

    // בדיקה שהסיסמה מכילה לפחות אות גדולה
    if (!Regex.IsMatch(password, @"[A-Z]"))
        throw new BO.BlWrongItemtException("Password must contain at least one uppercase letter.");

    // בדיקה שהסיסמה מכילה לפחות אות קטנה
    if (!Regex.IsMatch(password, @"[a-z]"))
        throw new BO.BlWrongItemtException("Password must contain at least one lowercase letter.");

    // בדיקה שהסיסמה מכילה לפחות ספרה
    if (!Regex.IsMatch(password, @"\d"))
        throw new BO.BlWrongItemtException("Password must contain at least one digit.");

        // בדיקה שהסיסמה מכילה לפחות תו מיוחד
        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]"))
            throw new BO.BlWrongItemtException("Password must contain at least one special character.");
}
    internal static void CheckLogic(BO.Volunteer boVolunteer)
    {
        try
        {
            CheckId(boVolunteer.Id);
            CheckPhoneNumber(boVolunteer.PhoneNumber);
            CheckEmail(boVolunteer.Email);
            CheckPassword(boVolunteer.Password);
            CheckAddress(boVolunteer);

        }
        catch (BO.BlWrongItemtException ex)
        {
            throw new BO.BlWrongItemtException($"the item have logic problem", ex);
        }

    }
}
