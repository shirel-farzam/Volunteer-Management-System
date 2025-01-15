using PL.Volunteer;
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
    /// Interaction logic for VolnteerMainWindow.xaml
    /// </summary>
    public partial class VolunteerMainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        /// <summary>
        /// Dependency property for CurrentVolunteer
        /// </summary>
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerMainWindow), new PropertyMetadata(null));

        public BO.Call? Call
        {
            get { return (BO.Call?)GetValue(CallProperty); }
            set { SetValue(CallProperty, value); }
        }

        /// <summary>
        /// Dependency property for CurrentVolunteer
        /// </summary>
        public static readonly DependencyProperty CallProperty =
            DependencyProperty.Register("Call", typeof(BO.Call), typeof(VolunteerMainWindow), new PropertyMetadata(null));

        public VolunteerMainWindow(int id)
        {
            CurrentVolunteer = s_bl.Volunteer.Read(id);
            //Call = null;
            if (CurrentVolunteer.CurrentCall != null)
            {

                Call = s_bl.Call.Read(CurrentVolunteer.CurrentCall.CallId);
            }
            InitializeComponent();
            DataContext = this;
        }
        /// <summary>
        /// Handles the click event for the "Update" button
        /// </summary>
        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentVolunteer != null)
                {
                    // Update the volunteer details in the database
                    s_bl.Volunteer.Update(CurrentVolunteer.Id,CurrentVolunteer);

                    // Notify the user
                    MessageBox.Show("Volunteer details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No volunteer selected to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors during the update
                MessageBox.Show($"Failed to update volunteer details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Display a message box with a Yes/No question
            var result = MessageBox.Show(
                "Do you want to open the call history?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            // Check if the user clicked "Yes"
            if (result == MessageBoxResult.Yes)
            {
                new CallHistory(CurrentVolunteer.Id,this).Show();
            }
            //else
            //{
            //    MessageBox.Show("Call history was not opened.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }

        //private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        //{
        //    // Update the volunteer details in the database
        //    s_bl.Volunteer.Update(CurrentVolunteer.Id, CurrentVolunteer);
        //    // Notify the user
        //    MessageBox.Show("Volunteer details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        //}
    }
}
