using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PL
{
    class ConvertDistanceToColor : IValueConverter
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
    class ConvertRoleToColor : IValueConverter
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
