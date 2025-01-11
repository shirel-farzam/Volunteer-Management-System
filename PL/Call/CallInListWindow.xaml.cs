using BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;


namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallInListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallInListProperty); }
            set { SetValue(CallInListProperty, value); }
        }

        public static readonly DependencyProperty CallInListProperty =
            DependencyProperty.Register("CallInList", typeof(IEnumerable<BO.CallInList>), typeof(CallInListWindow), new PropertyMetadata(null));
        public CallInListWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        public BO.CallStatus SelectedCallStatus { get; set; } = BO.CallStatus.Open;
        private void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change and update the SelectedFilter property
            if (sender is ComboBox comboBox && comboBox.SelectedItem is CallStatus selectedFilter)
            {
                SelectedCallStatus = selectedFilter; // Update the SelectedFilter property
            }
        }
        private IEnumerable<BO.CallInList> queryCallList()
        {
            IEnumerable<BO.CallInList> calls;

            switch (SelectedCallStatus)
            {
                case CallStatus.Open:
                    // קריאות פתוחות, ממויינות לפי סטטוס
                    calls = BlApi.Factory.Get().Call.GetCallInLists(CallInListField.Status, null, CallInListField.Status)
                              .Where(c => c.Status == CallStatus.Open)
                              .OrderBy(c => c.Status)
                              .ThenBy(c => c.CallId);
                    break;

                case CallStatus.InProgress:
                    // קריאות בטיפול, ממויינות לפי סטטוס
                    calls = BlApi.Factory.Get().Call.GetCallInLists(CallInListField.Status, null, CallInListField.Status)
                              .Where(c => c.Status == CallStatus.InProgress)
                              .OrderBy(c => c.Status)
                              .ThenBy(c => c.CallId);
                    break;

                case CallStatus.Closed:
                    // קריאות סגורות, ממויינות לפי סטטוס
                    calls = BlApi.Factory.Get().Call.GetCallInLists(CallInListField.Status, null, CallInListField.Status)
                              .Where(c => c.Status == CallStatus.Closed)
                              .OrderBy(c => c.Status)
                              .ThenBy(c => c.CallId);
                    break;

                case CallStatus.Expired:
                    // קריאות שפג תוקפן, ממויינות לפי סטטוס
                    calls = BlApi.Factory.Get().Call.GetCallInLists(CallInListField.Status, null, CallInListField.Status)
                              .Where(c => c.Status == CallStatus.Expired)
                              .OrderBy(c => c.Status)
                              .ThenBy(c => c.CallId);
                    break;

                case CallStatus.OpenRisk:
                    // קריאות פתוחות בסיכון, ממויינות לפי סטטוס
                    calls = BlApi.Factory.Get().Call.GetCallInLists(CallInListField.Status, null, CallInListField.Status)
                              .Where(c => c.Status == CallStatus.OpenRisk)
                              .OrderBy(c => c.Status)
                              .ThenBy(c => c.CallId);
                    break;

                case CallStatus.InProgressRisk:
                    // קריאות בטיפול בסיכון, ממויינות לפי סטטוס
                    calls = BlApi.Factory.Get().Call.GetCallInLists(CallInListField.Status, null, CallInListField.Status)
                              .Where(c => c.Status == CallStatus.InProgressRisk)
                              .OrderBy(c => c.Status)
                              .ThenBy(c => c.CallId);
                    break;

                default:
                    // ללא סינון לפי סטטוס
                    calls = BlApi.Factory.Get().Call.GetCallInLists(null, null, null)
                              .OrderBy(c => c.Status)
                              .ThenBy(c => c.CallId);
                    break;
            }

            return calls;
        }
        private void CallsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCallStatus != null)
                new CallWindow().Show();
        }


    }

}
