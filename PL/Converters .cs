using BO;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PL
{
    // Converter for setting IsReadOnly based on ID
    //public class ConvertUpdateToTrueKey : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        // If ID is not zero (not in add mode), make it read-only
    //        return value != null && (int)value != 0;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value;
    //    }
    //}

    //// Converter for Visibility based on ID
    //public class ConvertUpdateToVisibleKey : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        // If ID exists (in update mode), show the element
    //        return value != null && (int)value != 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    //    } 

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value;
    //    }
    //}

    //// Converter for converting Enum to a user-friendly string
    //public class RoleEnumToStringConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is Enum)
    //        {
    //            return value.ToString(); // Convert Enum to a user-friendly string
    //        }
    //        return string.Empty; // Default: empty string
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (parameter is Type enumType && Enum.IsDefined(enumType, value))
    //        {
    //            return Enum.Parse(enumType, value.ToString());
    //        }
    //        throw new InvalidOperationException("Cannot convert back to Enum.");
    //    }
    //}
    ////public class VolunteerInListFieldCollectionConverter : IValueConverter
    ////{
    ////    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    ////    {
    ////        // אם value הוא אוסף של VolunteerInListField, אז החזר את זה כקולקציה
    ////        if (value is IEnumerable<VolunteerInListField> collection)
    ////        {
    ////            return collection;
    ////        }
    ////        return null;
    ////    }

    ////    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    ////    {
    ////        // אם יש צורך להמיר חזרה (ConvertBack), תחזיר null או ערך ברירת מחדל
    ////        return null;
    ////    }
    ////}
    //public class VolunteerInListFieldCollectionConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {

    //        return Enum.GetValues(typeof(VolunteerInListField)).Cast<VolunteerInListField>().ToList();
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    //// Converter for setting background color based on Role Enum
    //public class RoleEnumToBackgroundConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        BO.Role role = (BO.Role)value;

    //        switch (role)
    //        {
    //            case BO.Role.Manager:
    //                return System.Windows.Media.Brushes.LightGoldenrodYellow; // Manager color
    //            case BO.Role.Volunteer:
    //                return System.Windows.Media.Brushes.LightSkyBlue; // Volunteer color
    //            default:
    //                return System.Windows.Media.Brushes.Transparent; // Default color
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value; // No back conversion needed
    //    }
    //}

    //// Converter for setting background color based on VolunteerInListField Enum
    //public class VolInListEnumToBackgroundConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        BO.VolunteerInListField vol = (BO.VolunteerInListField)value;

    //        switch (vol)
    //        {
    //            case BO.VolunteerInListField.Id:
    //                return System.Windows.Media.Brushes.LightGoldenrodYellow; // ID
    //            case BO.VolunteerInListField.FullName:
    //                return System.Windows.Media.Brushes.LightSkyBlue; // FullName
    //            case BO.VolunteerInListField.Active:
    //                return System.Windows.Media.Brushes.LightGreen; // Active
    //            case BO.VolunteerInListField.TotalCallsCanceled:
    //                return System.Windows.Media.Brushes.Orange; // TotalCallsCanceled
    //            case BO.VolunteerInListField.TotalCallsExpired:
    //                return System.Windows.Media.Brushes.PaleVioletRed; // TotalCallsExpired
    //            case BO.VolunteerInListField.TotalCallsHandled:
    //                return System.Windows.Media.Brushes.LightCyan; // TotalCallsHandled
    //            case BO.VolunteerInListField.CurrentCallId:
    //                return System.Windows.Media.Brushes.Plum; // CurrentCallId
    //            case BO.VolunteerInListField.CurrentCallType:
    //                return System.Windows.Media.Brushes.LightSalmon; // CurrentCallType
    //            case BO.VolunteerInListField.None:
    //                return System.Windows.Media.Brushes.Gray; // None
    //            default:
    //                return System.Windows.Media.Brushes.Transparent; // Default
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value; // No back conversion needed
    //    }
    //}

    //// Converter for setting background color based on Distance Enum
    //public class DistanceTypeEnumToBackgroundConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        BO.Distance distance_type = (BO.Distance)value;

    //        switch (distance_type)
    //        {
    //            case BO.Distance.Aerial:
    //                return System.Windows.Media.Brushes.LightYellow; // Aerial distance
    //            case BO.Distance.Walking:
    //                return System.Windows.Media.Brushes.LightCoral; // Walking distance
    //            case BO.Distance.Driving:
    //                return System.Windows.Media.Brushes.LightSteelBlue; // Driving distance
    //            default:
    //                return System.Windows.Media.Brushes.Transparent; // Default
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value; // No back conversion needed
    //    }
    //}
    //public class ConvertUpdateToTrue : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        // אם ה-Id לא אפס (לא מצב הוספה), אז לא ניתן לשנות
    //        return value != null && (int)value != 0;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value;
    //    }
    //}
    public class ConvertDistanceToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.Distance Dis = (BO.Distance)value;
            switch (Dis)
            {
                case BO.Distance.Aerial:
                    return Brushes.Yellow;
                case BO.Distance.Walking:
                    return Brushes.Orange;
                case BO.Distance.Driving:
                    return Brushes.Green;
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConvertRoleToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.Role Job = (BO.Role)value;
            switch (Job)
            {
                case BO.Role.Manager:
                    return Brushes.Blue;
                case BO.Role.Volunteer:
                    return Brushes.Orange;
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConvertUpdateToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // אם ה-Id לא אפס (לא מצב הוספה), אז לא ניתן לשנות
            return value != null && (int)value != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
