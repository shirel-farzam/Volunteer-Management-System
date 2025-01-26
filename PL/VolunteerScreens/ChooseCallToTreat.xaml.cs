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

namespace PL.VolunteerScreens
{
    /// <summary>
    /// Interaction logic for ChooseCallToTreat.xaml
    /// </summary>
    public partial class ChooseCallToTreat : Window
    {
        // Static reference to the BL API
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _previousWindow; // Variable to store a reference to the previous window

        // Property to store the selected field to sort by
        public BO.OpenCallInListField? OpenCallInList { get; set; } = BO.OpenCallInListField.Id;

        // Property to store the selected call
        public BO.OpenCallInList? SelectedCall { get; set; }

        // Property to store the volunteer ID
        public int VolunteerId { get; set; }

        // Property to bind and get/set the list of open calls
        public IEnumerable<BO.OpenCallInList> OpenCallList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallListProperty); }
            set { SetValue(OpenCallListProperty, value); }
        }

        // DependencyProperty to bind the OpenCallList
        public static readonly DependencyProperty OpenCallListProperty =
            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(ChooseCallToTreat), new PropertyMetadata(null));

        // Property to store the selected call type for filtering
        public BO.CallType? TypeCallInList { get; set; }

        // Constructor for initializing the window and setting the volunteer ID
        public ChooseCallToTreat(int id, Window previousWindow)
        {
            VolunteerId = id;
            _previousWindow = previousWindow;
            InitializeComponent();
            DataContext = this; // Set the data context for binding
        }

        // Event handler for filtering the call list by selected field
        private void Call_Filter(object sender, SelectionChangedEventArgs e)
        {
            OpenCallInList = (BO.OpenCallInListField)(((ComboBox)sender).SelectedItem);
            OpenCallList = s_bl?.Call.GetOpenCallsForVolunteer(VolunteerId, TypeCallInList, OpenCallInList)!;
        }

        // Event handler to filter the call list by the selected call type
        private void CallSelector_Filter(object sender, SelectionChangedEventArgs e)
        {

            QueryCallList();
            //if (SelectedCall != null)
            //{
            //    MessageBox.Show(SelectedCall.Description, $"Description {SelectedCall.Id}", MessageBoxButton.OK);
            //}
        }


        // Method to query the list of open calls based on selected filters
        private void QueryCallList()
        {
            // If no specific filter is selected, fetch all open calls
            OpenCallList = (OpenCallInList == BO.OpenCallInListField.Id) ?
            s_bl?.Call.GetOpenCallsForVolunteer(VolunteerId, null, null)! :
            s_bl?.Call.GetOpenCallsForVolunteer(VolunteerId, TypeCallInList, OpenCallInList)!;
        }

        // Method to observe changes in the call list and refresh the data
        private void CallListObserver() => QueryCallList();

        // Event handler for when the window is loaded, adding the call list observer
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(CallListObserver);

        // Event handler for when the window is closed, removing the call list observer
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(CallListObserver);

        // Event handler for when the selection in the data grid changes
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Display the description of the selected call
            MessageBox.Show(SelectedCall.Description, $"Description {SelectedCall.Id}", MessageBoxButton.OK);
        }

        // Event handler for choosing a call for treatment
        //private void BtnChoose_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        // Try to choose the selected call for treatment
        //        s_bl.Call.ChooseCallForTreat(VolunteerId, SelectedCall.Id);
        //        // Show a success message
        //        MessageBox.Show($"Call {SelectedCall.Id} was successfully Chosen!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //        this.Close(); // Close the window
        //    }
        //    catch (BO.BlAlreadyExistsException ex)
        //    {
        //        // Show a failure message if the call is already chosen
        //        MessageBox.Show(ex.Message, "Choose Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //        this.Close(); // Close the window
        //    }
        //    catch (Exception ex)
        //    {
        //        // Show a generic failure message
        //        MessageBox.Show(ex.Message, "Choose Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //    }
        //}
        private void BtnChoose_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BO.OpenCallInList call)
            {
                try
                {
                    s_bl.Call.ChooseCallForTreat(VolunteerId, call.Id);
                    MessageBox.Show($"Call {call.Id} was successfully chosen!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("No valid call found for this action.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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