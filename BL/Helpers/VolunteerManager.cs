using BlImplementation;
using DalApi;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Helpers;

internal class VolunteerManager
{
    private static readonly IDal s_dal = Factory.Get;
    internal static readonly ObserverManager Observers = new(); // stage 5

    internal static BO.VolunteerInList ConvertDOToBOInList(DO.Volunteer doVolunteer)
    {
        List<DO.Assignment>? calls;
        lock (AdminManager.BlMutex) // stage 7
            calls = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();

            int totalCallsHandled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.Treated);
            int totalCallsCanceled = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.SelfCancel);
            int totalCallsExpired = calls.Count(ass => ass.TypeEndTreat == DO.TypeEnd.ExpiredCancel);
            int? currentCallId = calls.FirstOrDefault(ass => ass.TimeEnd == null)?.Id;

            return new BO.VolunteerInList
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
        {
            List<DO.Assignment>? calls;
            lock (AdminManager.BlMutex) // stage 7
                calls=s_dal.Assignment.ReadAll(ass => ass.VolunteerId == doVolunteer.Id).ToList();
            DO.Assignment? currentAssignment = calls.Find(ass => ass.TimeEnd == null);
            if (currentAssignment == null) return null;
            DO.Call? currentCall;
            lock (AdminManager.BlMutex) // stage 7
                currentCall=s_dal.Call.Read(currentAssignment.CallId);
            if (currentCall == null) return null;

            double[] coordinates = GetCoordinatesFromAddress(doVolunteer.FullAddress);
            double latitude = coordinates[0];
            double longitude = coordinates[1];

            AdminImplementation admin = new AdminImplementation();
            BO.CallStatus status;
            if (currentCall.MaxTimeToClose - AdminManager.Now <= admin.GetMaxRange())
                status = BO.CallStatus.OpenRisk;
            else
                status = BO.CallStatus.InProgress;

            return new BO.CallInProgress
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
    }

    internal static void CheckFormat(BO.Volunteer boVolunteer)
    {
        // Validate the ID of the volunteer.
        if (boVolunteer.Id <= 0 || boVolunteer.Id.ToString().Length < 8 || boVolunteer.Id.ToString().Length > 9)
        {
            throw new BO.BlWrongItemException($"Invalid ID {boVolunteer.Id}. It must be 8-9 digits.");
        }

        // Validate the FullName field.
        if (string.IsNullOrWhiteSpace(boVolunteer.FullName) || !Regex.IsMatch(boVolunteer.FullName, @"^[a-zA-Z\s]+$"))
        {
            throw new BO.BlWrongItemException($"FullName {boVolunteer.FullName} cannot be null, empty, or contain invalid characters.");
        }

        // Validate the PhoneNumber field.
        if (string.IsNullOrWhiteSpace(boVolunteer.PhoneNumber))
        {
            throw new BO.BlWrongItemException($"PhoneNumber {boVolunteer.PhoneNumber} cannot be null or empty.");
        }

        if (boVolunteer.PhoneNumber.Length != 10 || !boVolunteer.PhoneNumber.All(char.IsDigit) || boVolunteer.PhoneNumber[0] != '0')
        {
            throw new BO.BlWrongItemException($"Invalid PhoneNumber {boVolunteer.PhoneNumber}.");
        }

        // Validate the Email field.
        if (!Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new BO.BlWrongItemException("Invalid Email format.");
        }

        if (boVolunteer.MaxReading.HasValue && boVolunteer.MaxReading.Value <= 0)
        {
            throw new BO.BlWrongItemException($"MaxReading {boVolunteer.MaxReading} must be a positive number.");
        }

        if (boVolunteer.Latitude.HasValue && (boVolunteer.Latitude.Value < -90 || boVolunteer.Latitude.Value > 90))
        {
            throw new BO.BlWrongItemException("Latitude must be between -90 and 90.");
        }

        if (boVolunteer.Longitude.HasValue && (boVolunteer.Longitude.Value < -180 || boVolunteer.Longitude.Value > 180))
        {
            throw new BO.BlWrongItemException($"Longitude {boVolunteer.Longitude} must be between -180 and 180.");
        }
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
        catch (BO.BlWrongItemException ex)
        {
            throw new BO.BlWrongItemException("The item has a logic problem", ex);
        }
    }

    internal static void CheckId(int id)
    {
        string idString = id.ToString();
        if (idString.Length != 9)
        {
            throw new BO.BlWrongItemException($"This ID {id} is not valid.");
        }

        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = idString[i] - '0';
            int multiplier = (i % 2) + 1;
            int product = digit * multiplier;
            sum += product > 9 ? product - 9 : product;
        }

        if (sum % 10 != 0)
        {
            throw new BO.BlWrongItemException($"This ID {id} is not valid.");
        }
    }

    internal static void CheckPhoneNumber(string phoneNumber)
    {
        if (phoneNumber.Length != 10 || !phoneNumber.All(char.IsDigit) || phoneNumber[0] != '0')
        {
            throw new BO.BlWrongItemException($"Invalid phone number {phoneNumber}.");
        }
    }

    internal static void CheckEmail(string email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailPattern))
        {
            throw new BO.BlWrongItemException($"Invalid email {email}.");
        }
    }

    internal static void CheckPassword(string password)
    {
        if (password.Length < 5)
            throw new BO.BlWrongItemException("Password must be at least 5 characters long.");

        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            throw new BO.BlWrongItemException("Password must contain at least one letter and one digit.");
    }

    private const string ApiKey = "pk.3d8d3ac902d00ffcd65fdf9a26ec253c";
    private const string LocationIqBaseUrl = "https://us1.locationiq.com/v1/search.php";

    public static double[] GetCoordinatesFromAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("The provided address is empty or invalid.", nameof(address));
        }

        using var client = new HttpClient();
        string requestUrl = $"{LocationIqBaseUrl}?key={ApiKey}&q={Uri.EscapeDataString(address)}&format=json";
        HttpResponseMessage response = client.GetAsync(requestUrl).Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error making geocoding request: {response.StatusCode}");
        }

        string jsonResponse = response.Content.ReadAsStringAsync().Result;
        JsonDocument document = JsonDocument.Parse(jsonResponse);
        JsonElement root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
        {
            JsonElement firstResult = root[0];

            if (firstResult.TryGetProperty("lat", out JsonElement latElement) &&
                firstResult.TryGetProperty("lon", out JsonElement lonElement))
            {
                double latitude = double.Parse(latElement.GetString()!, CultureInfo.InvariantCulture);
                double longitude = double.Parse(lonElement.GetString()!, CultureInfo.InvariantCulture);

                return new[] { latitude, longitude };
            }
        }

        throw new Exception("Latitude or Longitude is missing in the response.");
    }

    internal static void CheckAddress(BO.Volunteer volunteer)
    {
        double[] coordinates = GetCoordinatesFromAddress(volunteer.FullAddress);
        if (coordinates[0] != volunteer.Latitude || coordinates[1] != volunteer.Longitude)
            throw new BO.BlWrongItemException("Coordinates do not match.");
    }

    internal static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadius = 6371000; // meters
        double lat1Rad = lat1 * Math.PI / 180;
        double lon1Rad = lon1 * Math.PI / 180;
        double lat2Rad = lat2 * Math.PI / 180;
        double lon2Rad = lon2 * Math.PI / 180;

        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadius * c;
    }
    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;

    //internal static void SimulateCallRegistrationAndGrade() //stage 7
    //{
    //    Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";

    //    LinkedList<int> VolunteerToUpdate = new(); //stage 7
    //    List<DO.Volunteer> doStudList;

    //    lock (AdminManager.BlMutex) //stage 7
    //        doStudList = s_dal.Volunteer.ReadAll(st => st.Active == true).ToList();

    //    foreach (var doVolunteer in doStudList)
    //    {
    //        int VolunteerId = 0;
    //        lock (AdminManager.BlMutex) //stage 7
    //        {
    //            BO.studentYear = GetVolunteerCurrentYear(doVolunteer.RegistrationDate);

    //            //the above method, includes network requests to compute the distances
    //            //between courses address and current student address
    //            //these network requests are done synchronically
    //            var coursesNotRegistered = CourseManager.GetUnRegisteredCoursesForStudent(doStudent.Id, studentYear);

    //            int cntNotRegCourses = coursesNotRegistered.Count();
    //            if (cntNotRegCourses != 0)
    //            {
    //                int courseId = coursesNotRegistered.Skip(s_rand.Next(0, cntNotRegCourses)).First()!.Id;
    //                LinkManager.LinkStudentToCourse(doStudent.Id, courseId);
    //                studentId = doStudent.Id;
    //            }

    //            //simulate setting grade of course for selected student
    //            var coursesRegistered =
    //                s_dal.Course.ReadAll(course => LinkManager.IsStudentLinkedToCourse(doStudent.Id, course.Id) && course.InYear == (DO.Year)studentYear);
    //            int cntRegCourses = coursesRegistered.Count();
    //            if (cntRegCourses != 0)
    //            {
    //                int courseId = coursesRegistered.Skip(s_rand.Next(0, cntRegCourses)).First()!.Id;
    //                LinkManager.UpdateCourseGradeForStudent(doStudent.Id, courseId, Math.Round(s_rand.NextDouble() * 100, 2));
    //                studentId = doStudent.Id;
    //            }

    //            if (studentId != 0)
    //                studentsToUpdate.AddLast(doStudent.Id);
    //        } //lock
    //    }

    //    foreach (int id in studentsToUpdate)
    //        Observers.NotifyItemUpdated(id);
    //}


}
