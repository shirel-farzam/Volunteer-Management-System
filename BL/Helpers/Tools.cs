using System.Reflection;

static class StaticClass
{
    public static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in t.GetType().GetProperties())
            str += "\n" + item.Name + ": " + item.GetValue(t, null);
        return str;
    }
}
