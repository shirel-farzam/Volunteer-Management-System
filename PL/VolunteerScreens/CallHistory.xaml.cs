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

        public CallHistory(int id)
        {
            IdVolunteer = id;
            InitializeComponent();
            DataContext = this;
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
        }

        // מימוש נוסף למימוש סינון לפי סוג הקריאה
        private void CallType_Filter(object sender, SelectionChangedEventArgs e)
        {
            TypeCallInList = (BO.CallType?)(((ComboBox)sender).SelectedItem);
            queryClosedCallList();
        }
    }
}
