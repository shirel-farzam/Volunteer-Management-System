using BO;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PL
{

    public class BoolConvertIsCallInProsses : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (value == null)
            {
                return false;
            }
           
            if (value is BO.Volunteer myValue) // החלף את MyType בשם הסוג המתאים
            {
                return myValue.CurrentCall == null && myValue.Active;

            }
            return true;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class InverseNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value is null, return Visible; otherwise, return Collapsed
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not implemented.");
        }
    }
    public class ConvertDistanceToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.Distance Dis = (BO.Distance)value;
            switch (Dis)
            {
                case BO.Distance.Aerial:
                    return Brushes.Blue;
                case BO.Distance.Walking:
                    return Brushes.MediumPurple;
                case BO.Distance.Driving:
                    return Brushes.SaddleBrown;
                default:
                    return Brushes.LightGreen;
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
                    return Brushes.DarkGoldenrod;
                case BO.Role.Volunteer:
                    return Brushes.Silver;
                default:
                    return Brushes.SkyBlue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConvertCallTypeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.CallType type = (BO.CallType)value;
            switch (type)
            {
                case BO.CallType.FoodTransport:
                    return Brushes.LightCoral; // אדום בהיר
                case BO.CallType.FoodPreparation:
                    return Brushes.LightGreen; // ירוק בהיר
                case BO.CallType.InventoryCheck:
                    return Brushes.LightBlue; // כחול בהיר
                case BO.CallType.None:
                    return Brushes.Plum; // צהוב בהיר
                default:
                    return Brushes.Gray; // סגול עדין
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConvertTypeCAllToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AssignmentCompletionType type)
            {
                return type switch
                {
                    AssignmentCompletionType.AdminCanceled => Brushes.Red, // אדום
                    AssignmentCompletionType.Expired => Brushes.LightYellow, // צהוב בהיר
                    AssignmentCompletionType.Canceled => Brushes.LightBlue, // כחול בהיר
                    AssignmentCompletionType.Completed => Brushes.MediumPurple,
                    _ => Brushes.Gray // ברירת מחדל
                };
            }
            return Brushes.Gray; // במקרה של ערך לא מזוהה
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
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Stop" : "Start";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
   

public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue; // הפיכת הערך הבוליאני
        }
        return false; // ברירת מחדל במקרה של שגיאה
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // לא נחוץ במקרה הזה
    }
}
    public class CanDeleteCallConverter : IValueConverter
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (s_bl.Call.CanDelete((int)value))
            {
                return Visibility.Visible; // הפיכת הערך הבוליאני
            }
            return Visibility.Collapsed; // ברירת מחדל במקרה של שגיאה
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // לא נחוץ במקרה הזה
        }
    }
    public class CanDeleteVolunteerConverter : IValueConverter
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (s_bl.Volunteer.CanDelete((int)value))
            {
                return Visibility.Visible; // הפיכת הערך הבוליאני
            }
            return Visibility.Collapsed; // ברירת מחדל במקרה של שגיאה
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // לא נחוץ במקרה הזה
        }
    }

}
