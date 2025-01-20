using BO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.VolunteerScreens
{
    public partial class CallHistory : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _previousWindow;
        public IEnumerable<BO.ClosedCallInList> ClosedCallList
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(ClosedCallListProperty); }
            set { SetValue(ClosedCallListProperty, value); }
        }

        public static readonly DependencyProperty ClosedCallListProperty =
            DependencyProperty.Register("ClosedCallList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(CallHistory), new PropertyMetadata(null));

        public BO.ClosedCallInList? SelectedClosedCall { get; set; }

        public BO.ClosedCallInListField ClosedCallInList { get; set; }

        public int IdVolunteer { get; set; }

        public BO.CallType? TypeCallInList { get; set; }

        public CallHistory(int id, Window previousWindow)
        {
            IdVolunteer = id;
            InitializeComponent();
            DataContext = this;
            _previousWindow = previousWindow;
            queryClosedCallList();
        }

        private void cbVSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryClosedCallList();
        }

        // מימוש סינון הקריאות לפי סוג הקריאה
        private void Call_Filter(object sender, SelectionChangedEventArgs e)
        {
            // מבצע עדכון של משתנה פילטר הקריאות
            ClosedCallInList = (BO.ClosedCallInListField)(((ComboBox)sender).SelectedItem);
            // שולף את הקריאות המבוקשות לפי הפילטרים שנבחרו
            queryClosedCallList();
        }

        // מתודה לשליפת הקריאות לפי הפילטרים שנבחרו
        private void queryClosedCallList()
        {
            // שליפת הקריאות לפי המתודות של ה-API הפנימי
            ClosedCallList = s_bl?.Call.GetClosedCallsByVolunteer(IdVolunteer, TypeCallInList, ClosedCallInList) ?? Enumerable.Empty<BO.ClosedCallInList>();
            if (ClosedCallList.Any())
            {
                Console.WriteLine("Data found:");
                foreach (var call in ClosedCallList)
                {
                    Console.WriteLine($"Id: {call.Id}, Type: {call.CallType}, Address: {call.FullAddress}");
                }
            }
            else
            {
                Console.WriteLine("No data found!");
            }
        }

        private void CallType_Filter(object sender, SelectionChangedEventArgs e)
        {
            TypeCallInList = (BO.CallType?)(((ComboBox)sender).SelectedItem);
            queryClosedCallList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_previousWindow != null)
            {
                _previousWindow.Show(); // Show the previous window
                this.Hide(); // Close the current window
            }
            else
            {
                MessageBox.Show("Previous window is null!");
            }
        }
    }
}
