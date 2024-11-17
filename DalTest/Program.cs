using Dal;
using DalApi;
using DO;


namespace DalTest; 
    public enum OPTION
{
    EXIT,                         // יציאה מתפריט ראשית 
    SHOWICALL,                    //הצגת תת-תפריט עבור ישות
    SHOWIASSIGNMENT,              //הצגת תת-תפריט עבור ישות
    SHOWIVOLUNTEER,               //הצגת תת-תפריט עבור ישות
    ExitProgram,                  // יציאה מהתפריט הראשי ומהתוכנית
    DisplayVolunteerSubMenu,      // הצגת תפריט המשנה למתנדבים
    DisplayCallSubMenu,           // הצגת תפריט המשנה לשיחות
    DisplayAssignmentSubMenu,     // הצגת תפריט המשנה למשימות
    InitializeData,               // אתחול הנתונים בבסיס הנתונים
    DisplayAllData,               // הצגת כל הנתונים בבסיס הנתונים
    ResetDatabaseAndConfig        // איפוס בסיס הנתונים והקונפיגורציה
}


    internal class Program
    {
        private static IAssignment ? s_dalIAssignment = new AssignmentImplementation();
        private static ICall? s_dalICall = new CallImplementation();
        private static IVolunteer? s_dalIVolunteer = new VolunteerImplementation();
        private static IConfig? s_dalIConfig = new ConfigImplementation();





        static void Main(string[] args)
        {

            try
            {
                Initialization.Do(s_dalIAssignment, s_dalICall, s_dalIVolunteer, s_dalIConfig);
          
                if (IAssignment.Any()) 
                {
                    Console.WriteLine("There are assignments.");
                }



            }
            catch (Exception ex)
            {
                // תפיסת חריגות והדפסתן
                Console.WriteLine(ex.Message);
            }
        }
    }
}
