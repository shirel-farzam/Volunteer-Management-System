using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerInListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        public IEnumerable<BO.VolunteerInList> VolunteerInList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerInListProperty); }
            set { SetValue(VolunteerInListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerInListProperty =
        DependencyProperty.Register("VolunteerInList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerInListWindow), new PropertyMetadata(null));

        public BO.VolunteerInList? SelectedVolunteer { get; set; }
        public BO.CallType? VolunteerFilter { get; set; }
        public BO.VolunteerInListField VolunteerSort { get; set; } = BO.VolunteerInListField.Id;

        public int Id { get; set; }
        private Window _previousWindow; // Variable to store a reference to the previous window
        public VolunteerInListWindow(int bossdId, Window previousWindow)
        {
            InitializeComponent();
            Id = bossdId;
            DataContext = this;
           
            _previousWindow = previousWindow;
        }


        private void Volunteer_Sort(object sender, SelectionChangedEventArgs e) { QueryVolunteerList(); }
        
       
        private void Volunteer_Filter(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is BO.CallType selectedFilter)
            {
                VolunteerFilter = selectedFilter;
                VolunteerInList = s_bl?.Volunteer.ReadAll(null, VolunteerSort, VolunteerFilter) ?? Enumerable.Empty<BO.VolunteerInList>();
            }
        }

        private void QueryVolunteerList()
         => VolunteerInList = (VolunteerSort == BO.VolunteerInListField.Id) ?
         s_bl?.Volunteer.ReadAll(null, null)! : s_bl?.Volunteer.ReadAll(null, VolunteerSort,VolunteerFilter)!;

        private void VolunteerListObserver() /*=> QueryVolunteerList();*/
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    QueryVolunteerList();
                });

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // טוען את הנתונים כאשר החלון נטען
            //QueryVolunteerList();

            // מוסיף את הצופה למעקב אחרי שינויים ברשימה
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
        }


        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerListObserver);

        private void lsvVolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow.VolunteerWindow(SelectedVolunteer.Id).Show();
        }

        private void AddVolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow.VolunteerWindow().Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to delete this volunteer?", "Reset Confirmation",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.DeleteVolunteer(SelectedVolunteer.Id);
                }
                catch (BO.BlDeleteNotPossibleException ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private bool isFiltered = false;

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isFiltered)
                {
                    // If list is filtered, reset filter and show the full list
                    VolunteerInList = s_bl?.Volunteer.ReadAll(null, VolunteerSort)!;
                    isFiltered = false;
                    ((Button)sender).Content = "Filter active volunteer";
                }
                else
                {
                    // If list is not filtered, filter for active volunteers
                    VolunteerInList = s_bl?.Volunteer.ReadAll(true, VolunteerSort)!;
                    isFiltered = true;
                    ((Button)sender).Content = "Show all volunteers";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
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
