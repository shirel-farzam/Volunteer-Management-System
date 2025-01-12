using BO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PL.VolunteerScreens
{
    public partial class CallHistory : Window
    {
        public ObservableCollection<ClosedCallInList> ClosedCallsList { get; set; }

        public CallHistory(int volunteerId)
        {
            InitializeComponent();
            LoadClosedCalls(volunteerId);
            DataContext = this;
        }

        private void LoadClosedCalls(int volunteerId)
        {
            try
            {
                var closedCalls = BlApi.Factory.Get().Call.GetClosedCallsByVolunteer(volunteerId);
                ClosedCallsList = new ObservableCollection<ClosedCallInList>(closedCalls);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת הקריאות: {ex.Message}");
            }
        }
    }
}
