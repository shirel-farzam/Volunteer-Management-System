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

        public BO.ClosedCallInList? SelectedColsedCall { get; set; }

        public BO.ClosedCallInListField ClosedCallInList { get; set; }

        public int VolunteerId { get; set; }

        public BO.CallType? TypeCallInList { get; set; }

        public CallHistory(int id)
        {
            VolunteerId = id;
            InitializeComponent();
            DataContext = this;

        }

        private void cbVSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryClosedCallList();
        }

        private void Call_Filter(object sender, SelectionChangedEventArgs e)
        {
            ClosedCallInList = (BO.ClosedCallInListField)(((ComboBox)sender).SelectedItem);
            ClosedCallList = s_bl?.Call.GetClosedCallsByVolunteer(VolunteerId, TypeCallInList, ClosedCallInList)!;
        }


        private void queryClosedCallList()
    => ClosedCallList = s_bl?.Call.GetClosedCallsByVolunteer(VolunteerId, TypeCallInList, ClosedCallInList)!;

        private void CallSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
        private void BtnChoose_Click(object sender, RoutedEventArgs e)
        {

        }

    }

}
