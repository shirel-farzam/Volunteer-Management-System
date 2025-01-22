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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerInListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

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
            //UpdateVolunteerList();  // Load the volunteer list without any filter initially
            _previousWindow = previousWindow;
        }

        private void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryVolunteerList();
        }

        private void Volunteer_Sort(object sender, SelectionChangedEventArgs e) { QueryVolunteerList(); }
        //{
        //    VolunteerSort = (BO.VolunteerInListField)(((ComboBox)sender).SelectedItem);
        //    VolunteerInList = s_bl?.Volunteer.ReadAll(null, VolunteerSort, VolunteerFilter)!;

        //}
        //private void Volunteer_Sort(object sender, SelectionChangedEventArgs e)
        //{
        //    // בדוק אם sender הוא ComboBox
        //    if (sender is ComboBox comboBox)
        //    {
        //        // בדוק אם SelectedItem אינו null
        //        if (comboBox.SelectedItem is BO.VolunteerInListField selectedSort)
        //        {
        //            VolunteerSort = selectedSort;

        //            // בדוק אם הסביבה s_bl מאופסת כראוי
        //            if (s_bl?.Volunteer != null)
        //            {
        //                VolunteerInList = s_bl.Volunteer.ReadAll(null, VolunteerSort, VolunteerFilter) ?? Enumerable.Empty<BO.VolunteerInList>();
        //            }
        //            else
        //            {
        //                MessageBox.Show("The business logic object (s_bl) is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("No valid sort field was selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("The sender is not a valid ComboBox.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}

        //private void Volunteer_Filter(object sender, SelectionChangedEventArgs e)
        //{
        //    //VolunteerFilter = (BO.CallType)(((ComboBox)sender).SelectedItem);
        //    //VolunteerInList = s_bl?.Volunteer.ReadAll(null, VolunteerSort, VolunteerFilter)!;
        //    VolunteerFilter = (BO.CallType)(((ComboBox)sender).SelectedItem);
        //    VolunteerInList = s_bl?.Volunteer.ReadAll(null, VolunteerSort)!;

        //}
        private void Volunteer_Filter(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is BO.CallType selectedFilter)
            {
                VolunteerFilter = selectedFilter;
                VolunteerInList = s_bl?.Volunteer.ReadAll(null, VolunteerSort, VolunteerFilter) ?? Enumerable.Empty<BO.VolunteerInList>();
            }
        }


        //private void QueryVolunteerList()
        //{
        //VolunteerInList = (VolunteerSort == BO.VolunteerInListField.Id) ?
        //    s_bl?.Volunteer.ReadAll(null, null)! :
        //    s_bl?.Volunteer.ReadAll(null,VolunteerSort ,VolunteerFilter)!;
        //}
        private void QueryVolunteerList()
         => VolunteerInList = (VolunteerSort == BO.VolunteerInListField.Id) ?
         s_bl?.Volunteer.ReadAll(null, null)! : s_bl?.Volunteer.ReadAll(null, VolunteerSort,VolunteerFilter)!;


        private void VolunteerListObserver() => QueryVolunteerList();

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
//}
//    /// <summary>
//    /// Interaction logic for VolunteerListWindow.xaml
//    /// </summary>
//    public partial class VolunteerListWindow : Window
//    {
//        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

//        public IEnumerable<BO.VolunteerInList> VolunteerList
//        {
//            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
//            set { SetValue(VolunteerListProperty, value); }
//        }

//        public static readonly DependencyProperty VolunteerListProperty =
//        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

//        public BO.VolunteerInList? SelectedVolunteer { get; set; }

//        public BO.EVolunteerInList VolunteerInList { get; set; } = BO.EVolunteerInList.Id;
//        public int Id { get; set; }
//        public VolunteerListWindow(int bossdId)
//        {
//            Id = bossdId;
//            InitializeComponent();
//        }

//        private void VolunteerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        { QueryVolunteerList(); }

//        private void VolunteerFilter(object sender, SelectionChangedEventArgs e)
//        {
//            VolunteerInList = (BO.EVolunteerInList)(((ComboBox)sender).SelectedItem);
//            VolunteerList = s_bl?.Volunteers.GetVolunteerList(null, VolunteerInList)!;
//        }

//        private void QueryVolunteerList()
//        => VolunteerList = (VolunteerInList == BO.EVolunteerInList.Id) ?
//        s_bl?.Volunteers.GetVolunteerList(null, null)! : s_bl?.Volunteers.GetVolunteerList(null, VolunteerInList)!;

 
   
//    }
//}

