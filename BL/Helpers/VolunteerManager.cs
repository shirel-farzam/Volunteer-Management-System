using BlImplementation;
using DalApi;
using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Helpers;

internal class VolunteerManager
{
    private static IDal s_dal = Factory.Get;

    
    internal static BO.VolunteerInList ConvertDOToBOInList(DO.Volunteer doVolunteer)
    {
        var calls = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();

        int totalCallsHandled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated);
        int totalCallsCanceled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.SelfCancel);
        int totalCallsExpired = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.ExpiredCancel);
        int? currentCallId = calls.FirstOrDefault(ass => ass.TimeEnd == null)?.Id;

        return new BO.VolunteerInList()
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.FullName,
            Active = doVolunteer.Active,
            TotalCallsHandled = totalCallsHandled,
            TotalCallsCanceled = totalCallsCanceled,
            TotalCallsExpired = totalCallsExpired,
            CurrentCallId = currentCallId
        };
    }
    internal static BO.CallInProgress GetCallIn(DO.Volunteer doVolunteer)
    {
        var calls = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();
        DO.Assignment? currentAssignment = calls.Find(ass => ass.TimeEnd == null);
        if (currentAssignment == null) return null;

        DO.Call? currentCall = s_dal.Call.Read(currentAssignment.CallId);
        if (currentCall == null) return null;

        double[] coordinates = GetCoordinates(doVolunteer.FullAddress);
        double latitude = coordinates[0];
        double longitude = coordinates[1];

        AdminImplementation admin = new AdminImplementation();
        BO.CallStatus status;
        if (currentCall.MaxTimeToClose - ClockManager.Now <= admin.GetMaxRange())
            status = BO.CallStatus.OpenRisk;
        else
            status = BO.CallStatus.InProgress;

        return new BO.CallInProgress()
        {
            Id = currentAssignment.Id,
            CallId = currentAssignment.CallId,
            CallType = (BO.CallType)currentCall.Type,
            Description = currentCall.Description,
            FullAddress = currentCall.FullAddress,
            OpeningTime = currentCall.TimeOpened,
            MaxCompletionTime = currentCall.MaxTimeToClose,
            EntryTime = currentAssignment.TimeStart,
            DistanceFromVolunteer = CalculateDistance(currentCall.Latitude, currentCall.Longitude, latitude, longitude),
            Status = status
        };
    }

    internal static void CheckFormat(BO.Volunteer boVolunteer)
    {
        /// <summary>
        /// Validate the ID of the volunteer.
        /// The ID must be a positive integer and consist of 8 to 9 digits.
        /// </summary>
        if (boVolunteer.Id <= 0 || boVolunteer.Id.ToString().Length < 8 || boVolunteer.Id.ToString().Length > 9)
        {
            throw new BO.BlWrongItemException($"Invalid ID {boVolunteer.Id}. It must be 8-9 digits.");
        }
        /// <summary>
        /// Validate the FullName field.
        /// The name must not be null, empty, or consist of only whitespace.
        /// </summary>
        if (string.IsNullOrWhiteSpace(boVolunteer.FullName) || !Regex.IsMatch(boVolunteer.FullName, @"^[a-zA-Z\s]+$"))
        {
            throw new BO.BlWrongItemException($"FullName {boVolunteer.FullName} cannot be null, empty, or contain invalid characters.");
        }

        /// <summary>
        /// Validate the PhoneNumber field.
        /// The phone number must be exactly 10 digits and start with 0.
        /// </summary>

        if (string.IsNullOrWhiteSpace(boVolunteer.FullName))
        {
            throw new BO.BlWrongItemException($"FullName {boVolunteer.FullName} cannot be null or empty.");
        }

        if (boVolunteer.FullName.Any(c => !Char.IsLetter(c) && !Char.IsWhiteSpace(c)))
        {
            throw new BO.BlWrongItemException($"FullName {boVolunteer.FullName} contains invalid characters.");
        }

        /// <summary>
        /// Validate the Email field.
        /// The email must match the standard email format.
        /// </summary>
        if (!Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new BO.BlWrongItemException("Invalid Email format.");
        }

       
        if (boVolunteer.MaxReading.HasValue)
        {
            if (!double.TryParse(boVolunteer.MaxReading.Value.ToString(), out double maxReadingValue) || maxReadingValue <= 0)
            {
                throw new BO.BlWrongItemException($"MaxReading {boVolunteer.MaxReading} must be a positive number.");
            }
        }


        if (boVolunteer.Latitude.HasValue && (boVolunteer.Latitude.Value < -90 || boVolunteer.Latitude.Value > 90))
        {
            throw new BO.BlWrongItemException("Latitude must be between -90 and 90.");
        }

        /// <summary>
        /// Validate the Longitude field.
        /// If provided, it must be between -180 and 180 (inclusive).
        /// </summary>
        if (boVolunteer.Longitude.HasValue && (boVolunteer.Longitude.Value < -180 || boVolunteer.Longitude.Value > 180))
        {
            throw new BO.BlWrongItemException($"Longitude {boVolunteer.Longitude} must be between -180 and 180.");
        }

        /// <summary>
        /// Add any additional validation checks here if needed in the future.
        /// </summary>
    }
    internal static void CheckLogic(BO.Volunteer boVolunteer)
    {
        try
        {
            CheckId(boVolunteer.Id);
            CheckPhonnumber(boVolunteer.PhoneNumber);
            CheckEmail(boVolunteer.Email);
            CheckPassword(boVolunteer.Password);
            CheckAddress(boVolunteer);

        }
        catch (BO.BlWrongItemException ex)
        {
            throw new BO.BlWrongItemException($"the item have logic problem", ex);
        }
    }
    /// <summary>
    /// Validates an Israeli ID number.
    /// Throws an exception if the ID is invalid.
    /// </summary>
    /// <param name="id">The ID number as an integer.</param>
    /// <exception cref="ArgumentException">Thrown if the ID is not valid.</exception>
    internal static void CheckId(int id)
    {
        // Convert the integer ID to a string to process individual digits
        string idString = id.ToString();

        // Ensure the ID is exactly 9 digits long
        if (idString.Length != 9)
        {
            throw new BO.BlWrongItemException($"this ID {id} does not posssible");
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

        //chach id digit
        if (sum % 10 != 0)
        {
            throw new BO.BlWrongItemException($"this ID {id} does not posssible");
        }
    }
    /// <summary>
    /// Validates if a given phone number represents a valid mobile number.
    /// The number must be exactly 10 digits long, consist only of digits, 
    /// and start with the digit '0'.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate, as a string.</param>
    /// <exception cref="ArgumentException">Thrown if the phone number is not valid.</exception>
    internal static void CheckPhonnumber(string phoneNumber)
    {
        // Check if the string length is exactly 10 characters
        if (phoneNumber.Length != 10)
        {
            throw new BO.BlWrongItemException($"The phone number {phoneNumber}must contain exactly 10 digits.");
        }

        // Check if all characters are digits
        foreach (char c in phoneNumber)
        {
            if (!char.IsDigit(c))
            {
                throw new BO.BlWrongItemException($"The phone number {phoneNumber} must contain digits only.");
            }
        }

        // Check if the first character is '0'
        if (phoneNumber[0] != '0')
        {
            throw new BO.BlWrongItemException($"A valid mobile phone number{phoneNumber} must start with '0'.");
        }

    }
    /// <summary>
    /// Validates whether the given string is a valid email address.
    /// The email address must match a standard email format (e.g., username@domain.com).
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <exception cref="ArgumentException">Thrown if the email address is not valid.</exception>
    internal static void CheckEmail(string email)
    {
        // Regular expression pattern for a valid email address
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // Check if the email matches the pattern
        if (!Regex.IsMatch(email, emailPattern))
        {
            throw new BO.BlWrongItemException($"The provided email {email} address is not valid.");
        }
    }
    /// <summary>
    /// Validates if a given password is strong.
    /// A strong password must:
    /// - Be at least 5 characters long.
    /// - Contain at least one letter.
    /// - Contain at least one digit.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the password is not strong (does not meet the criteria).
    /// </exception>
    internal static void CheckPassword(string password)
    {
        // Check if the password is at least 5 characters long
        if (password.Length < 5)
            throw new BO.BlWrongItemException($"Password {password} must be at least 5 characters long.");

        // Flags to check for at least one letter and one digit
        bool hasLetter = false;
        bool hasDigit = false;

        // Iterate over each character in the password
        foreach (char c in password)
        {
            if (char.IsLetter(c))
                hasLetter = true;
            if (char.IsDigit(c))
                hasDigit = true;

            // If both conditions are met, the password is strong
            if (hasLetter && hasDigit)
                return;
        }

        // If the password does not contain both a letter and a digit, throw an exception
        if (!hasLetter)
            throw new BO.BlWrongItemException($"Password {password} must contain at least one letter.");
        if (!hasDigit)
            throw new BO.BlWrongItemException($"Password{password} must contain at least one digit.");
    }

    /// <summary>
    /// This method takes an address as input and returns an array with the latitude and longitude.
    /// The request is synchronous, meaning it waits for the response before continuing.
    /// </summary>
    /// <param name="address">The address to be geocoded</param>

    /// <returns>A double array containing the latitude and longitude</returns>
    private const string ApiKey = "pk.3d8d3ac902d00ffcd65fdf9a26ec253c";
    public static double[] GetCoordinates(string address)
    {
        // Check if the address is empty or null
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be empty or null.", nameof(address));
        }

        // Build the API URL using the access key and the address
        string url = $"https://us1.locationiq.com/v1/search.php?key={Uri.EscapeDataString(ApiKey)}&q={Uri.EscapeDataString(address)}&format=json";

        // Create an HTTP request
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        // Add a User-Agent header to avoid server blocking
        request.Headers.Add("User-Agent", "MyCSharpApp/1.0");

        try
        {
            // Send the request and get the response
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // Check if the response status is OK (200)
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Error in request: {response.StatusCode}");
                }

                // Read the response body as a string
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string jsonResponse = reader.ReadToEnd();

                    // Deserialize the response into an array of objects
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var results = JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, options);

                    // Check if any results were found
                    if (results == null || results.Length == 0)
                    {
                        throw new Exception("No coordinates found for the given address.");
                    }

                    // Return the coordinates
                    return new double[] { double.Parse(results[0].Lat), double.Parse(results[0].Lon) };
                }
            }
        }
        catch (WebException ex)
        {
            // Handle network-related errors
            throw new Exception("Error sending web request: " + ex.Message);
        }
        catch (Exception ex)
        {
            // Handle general errors
            throw new Exception("General error: " + ex.Message);
        }
    }



    // Data structure to match the JSON response from the API
    public class LocationResult
    {
        public string Lat { get; set; } // Latitude of the location
        public string Lon { get; set; } // Longitude of the location
    }

    internal static void CheckAddress(BO.Volunteer volunteer)
    {
        double[] cordinates = GetCoordinates(volunteer.FullAddress);
        if (cordinates[0] != volunteer.Latitude || cordinates[1] != volunteer.Longitude)
            throw new BO.BlWrongItemException($"not math cordinates");
    }


    internal static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadius = 6371000; // Earth's radius in meters

        // Convert latitude and longitude from degrees to radians
        double lat1Rad = lat1 * Math.PI / 180;
        double lon1Rad = lon1 * Math.PI / 180;
        double lat2Rad = lat2 * Math.PI / 180;
        double lon2Rad = lon2 * Math.PI / 180;

        // Differences in latitude and longitude
        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        // Haversine formula
        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Final distance in meters
        return EarthRadius * c;
    }

}