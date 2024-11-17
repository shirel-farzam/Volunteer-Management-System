namespace Dal;

internal static class DataSource
{
    // List to store Volunteer objects
    internal static List<DO.Volunteer> Volunteers { get; } = new();

    // List to store Call objects
    internal static List<DO.Call> Calls { get; } = new();

    // List to store Assignment objects
    internal static List<DO.Assignment> Assignments { get; } = new();
}
