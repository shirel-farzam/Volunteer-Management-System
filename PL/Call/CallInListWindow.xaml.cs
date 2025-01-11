using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace PL.Call

{
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

        public BO.CallStatus SelectedCallStatus { get; set; } = BO.CallStatus.Open;

        public CallInListWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update the SelectedCallStatus when ComboBox selection changes
            if (sender is ComboBox comboBox && comboBox.SelectedItem is BO.CallStatus selectedFilter)
            {
                SelectedCallStatus = selectedFilter;
            }
        }

        // Function to filter the list of calls
        private IEnumerable<BO.CallInList> queryCallList()
        {
            IEnumerable<BO.CallInList> calls;

            // Get CallId filter from TextBox
            var callIdText = ((TextBox)this.FindName("CallIdTextBox")).Text;

            switch (SelectedCallStatus)
            {
                case BO.CallStatus.Open:
                    calls = s_bl.Call.GetCallInLists(BO.CallInListField.Status, null, BO.CallInListField.Status)
                             .Where(c => c.Status == BO.CallStatus.Open);
                    break;

                case BO.CallStatus.InProgress:
                    calls = s_bl.Call.GetCallInLists(BO.CallInListField.Status, null, BO.CallInListField.Status)
                             .Where(c => c.Status == BO.CallStatus.InProgress);
                    break;

                case BO.CallStatus.Closed:
                    calls = s_bl.Call.GetCallInLists(BO.CallInListField.Status, null, BO.CallInListField.Status)
                             .Where(c => c.Status == BO.CallStatus.Closed);
                    break;

                case BO.CallStatus.Expired:
                    calls = s_bl.Call.GetCallInLists(BO.CallInListField.Status, null, BO.CallInListField.Status)
                             .Where(c => c.Status == BO.CallStatus.Expired);
                    break;

                case BO.CallStatus.OpenRisk:
                    calls = s_bl.Call.GetCallInLists(BO.CallInListField.Status, null, BO.CallInListField.Status)
                             .Where(c => c.Status == BO.CallStatus.OpenRisk);
                    break;

                case BO.CallStatus.InProgressRisk:
                    calls = s_bl.Call.GetCallInLists(BO.CallInListField.Status, null, BO.CallInListField.Status)
                             .Where(c => c.Status == BO.CallStatus.InProgressRisk);
                    break;

                default:
                    calls = s_bl.Call.GetCallInLists(null, null, null);
                    break;
            }

            // Apply CallId filter if provided
            if (!string.IsNullOrEmpty(callIdText))
            {
                calls = calls.Where(c => c.CallId.ToString().Contains(callIdText));
            }

            // Order the results
            calls = calls.OrderBy(c => c.Status).ThenBy(c => c.CallId);

            return calls;
        }

        // Filter the calls when the filter button is clicked
        private void FilterCalls_Click(object sender, RoutedEventArgs e)
        {
            // Update the CallList property with filtered results
            CallList = queryCallList();
        }

        private void CallsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCallStatus != null)
                new CallWindow().Show();
        }

        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is BO.CallInList selectedCall)
                {
                    int callId = selectedCall.CallId;

                    // Retrieve call data and cancel assignment logic
                    var call = s_bl.Call.Read(callId);
                    if (call == null)
                    {
                        MessageBox.Show("Call not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var currentAssignment = call.CallAssignments?.FirstOrDefault(a => a.CompletionType == null);
                    if (currentAssignment == null)
                    {
                        MessageBox.Show("No active assignment found for this call.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var volunteer = s_bl.Volunteer.Read(currentAssignment.VolunteerId);
                    if (volunteer == null)
                    {
                        MessageBox.Show("Volunteer not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Cancel the assignment
                    currentAssignment.CompletionType = BO.AssignmentCompletionType.Canceled;
                    s_bl.Call.Update(call);

                    MessageBox.Show($"Assignment for call {callId} has been canceled and email sent to volunteer {volunteer.FullName}.",
                                     "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Unable to retrieve call information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanDeleteCall(BO.CallInList call)
        {
            // בדוק אם הקריאה בסטטוס Open וטרם הוקצתה למתנדב
            return call.Status == BO.CallStatus.Open;
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is BO.CallInList selectedCall)
                {
                    // בדוק אם ניתן למחוק את הקריאה
                    if (CanDeleteCall(selectedCall))
                    {
                        // מחיקת הקריאה מהמאגר
                        s_bl.Call.DeleteCall(selectedCall.CallId);

                        // עדכון הרשימה
                        CallList = queryCallList();

                        MessageBox.Show($"Call {selectedCall.CallId} has been deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("This call cannot be deleted. It must be Open and not assigned to any volunteer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

}


