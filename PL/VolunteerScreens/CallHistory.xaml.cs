using BO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.VolunteerScreens
{
    public partial class CallHistory : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _previousWindow;
       // private volatile DispatcherOperation? _observerOperation = null; //stage 7

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

        // Filter
        private void Call_Filter(object sender, SelectionChangedEventArgs e)
        {
            // Update Filter
            ClosedCallInList = (BO.ClosedCallInListField)(((ComboBox)sender).SelectedItem);
            
            queryClosedCallList();
        }
        //private void ClosedCallListObserver() //stage 7
        //{
        //    if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
        //        _observerOperation = Dispatcher.BeginInvoke(() =>
        //        {
        //            queryClosedCallList();
        //        });
        //}

        private void queryClosedCallList()
        {
           
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
