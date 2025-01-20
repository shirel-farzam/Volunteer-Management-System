using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
        public int ManagerId { get; set; }

        
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

        public VolunteerMainWindow(int id = 0, int manId = 0)
        {
            
            InitializeComponent();
            ManagerId = manId;
            DataContext = this;
            try
            {
                CurrentVolunteer = s_bl.Volunteer.Read(ManagerId); /*(id != 0) ? s_bl.Volunteer.Read(id)! : new BO.Volunteer() { Id = 0, FullName = "", PhoneNumber = "", Email = "", TypeDistance = BO.Distance.Aerial, Job = BO.Role.Volunteer, Active = false };*/
                if (CurrentVolunteer.CurrentCall != null)
                    Call = s_bl.Call.Read(CurrentVolunteer.CurrentCall.CallId);
                else 
                    Call = null;
            }
            catch (BO.BlDoesNotExistException ex)
            {
                CurrentVolunteer = null;
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            
            s_bl.Volunteer.AddObserver(ManagerId, VolunteerObserver);
            if (Call != null)
                s_bl.Call.AddObserver(ManagerId, CallObserver);
           
            }
        private void CallObserver()
        => QueryCall();
        private void QueryCall()
        {
            QueryVolunteer();
            if(Call != null)
            {
                if(CurrentVolunteer.CurrentCall==null||CurrentVolunteer.CurrentCall.CallId!=Call.Id)
                    s_bl.Call.RemoveObserver(Call.Id, CallObserver);
                if (CurrentVolunteer.CurrentCall != null && Call!=null && CurrentVolunteer.CurrentCall.CallId != Call.Id)
                    s_bl.Call.AddObserver(CurrentVolunteer.CurrentCall.CallId, CallObserver);
            }
            else
                Call = null;
            if(CurrentVolunteer.CurrentCall!=null)
                Call=s_bl.Call.Read(CurrentVolunteer.CurrentCall.CallId);   

        }
        private void QueryVolunteer()
        {
            CurrentVolunteer = s_bl.Volunteer.Read(ManagerId);
        }
        private void VolunteerObserver()
        {
            //int id = CurrentVolunteer!.Id;
            //CurrentVolunteer = null;
            //CurrentVolunteer = s_bl.Volunteer.Read(id);
            QueryCall();
        }
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.Update(CurrentVolunteer.Id, CurrentVolunteer!);
                MessageBox.Show($" Successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private void BtHistory_Click(object sender, RoutedEventArgs e)
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);
            if(Call!=null)
                s_bl.Call.AddObserver(Call.Id, CallObserver);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.Id, VolunteerObserver);
            if (Call != null)
                s_bl.Call.RemoveObserver(Call.Id, CallObserver);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new ChooseCallToTreat(ManagerId,this).Show();
        }

        // Event handler for "End Current Call" button
        private void EndCurrentCall_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to close this call?", "Reset Confirmation",
                                           MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.CloseTreat(ManagerId, CurrentVolunteer.CurrentCall.Id);
                    MessageBox.Show($"Call was successfully Closed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (BO.BlDeleteNotPossibleException ex)
                {
                    MessageBox.Show(ex.Message, "Close Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Close Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }


        }

        // Event handler for "Cancel Call" button
        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to cancel this call?", "Reset Confirmation",
                                          MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.CancelTreat(ManagerId, CurrentVolunteer.CurrentCall.Id);
                    MessageBox.Show($"Call was successfully Canceld!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (BO.BlDeleteNotPossibleException ex)
                {
                    MessageBox.Show(ex.Message, "Cancel Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Cancel Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }

        }


    }
}
