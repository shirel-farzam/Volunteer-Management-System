using DalApi;
using System.Reflection;

static class StaticClass
{
    private static IDal s_dal = Factory.Get; //stage 4
    public static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in t.GetType().GetProperties())
            str += "\n" + item.Name + ": " + item.GetValue(t, null);
        return str;
    }
    //public static bool IsUserExists(string username)
    //{
    //    var user = _dal.User.ReadByUsername(username);
    //    return user != null;
    //}
    public static bool IsPasswordCorrect(string storedPassword, string inputPassword)
    {
        // Assuming passwords are stored in an encrypted format
        // Compare the entered password with the stored one
        return storedPassword == inputPassword; // Here, encryption logic can be added if needed
    }
}
