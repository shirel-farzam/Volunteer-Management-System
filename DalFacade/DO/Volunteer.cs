
namespace DO;
/// <summary>
/// Student Entity represents a student with all its props
/// </summary>
/// <param name="Id">Personal unique ID of the student (as in national id card)</param>
/// <param name="Name">Private Name of the student</param>
/// <param name="RegistrationDate">Registration date of the student into the graduation program</param>
/// <param name="Alias">student’s alias name (default empty)</param>
/// <param name="IsActive">whether the student is active in studies (default true)</param>
/// <param name="BirthDate">student’s birthday (default empty)</param>

public record Volunteer
(

     int Id,
     string FullName,
     string PhoneNumber,
     string Email,
     Distance TypeDistance,
     Role Job,
     bool Active,
     string? Password = null,
     string? FullAddress = null,
     double? Latitude = null,
     double? Longitude = null,
     double? MaxReading = null

)
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Volunteer() : this(0, "", "", "", default(Distance), default(Role), false) { }
    public void Create(T item)
    {
        int newId = DataSource.Config.GetNextId();
        if (volunteers.Any(existingVolunteer => existingVolunteer.Id == volunteer.Id && volunteer.Id != 0) // עכשיו אני רוצה להוסיף אותו למאגר רק אם הוא לא נמצא שם כבר זא שאם הוא נמצא ישלח לי חריגה בנוסף אם יש בפנים ערך ברירת מחדל זה רק חריגה אז הוא בודק זאת  
            // מתבצעת בדיקה על רשימת באמצעות  עוברת על כל האובייקטים ברשימה ובודקת אם לפחות אחד מהם עונה על תנאי מסוים.

            throw new InvalidOperationException($"מתנדב עם תעודת זהות {volunteer.Id} כבר קיים במערכת.");

        volunteers.Add(newVolunteer);// מוסיפה 
        
    } 



