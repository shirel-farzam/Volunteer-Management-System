using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using DO;

namespace PL.Call

{
    public partial class CallInListWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private BO.CallStatus _selectedCallStatus = BO.CallStatus.None;

        public BO.CallStatus SelectedCallStatus
        {
            get => _selectedCallStatus;
            set
            {
                if (_selectedCallStatus != value)
                {
                    _selectedCallStatus = value;
                    OnPropertyChanged(nameof(SelectedCallStatus));
                    RefreshCallList(); // רענן את רשימת הקריאות
                }
            }
        }

        private IEnumerable<BO.CallInList> _callInList;
        public IEnumerable<BO.CallInList> CallInList
        {
            get => _callInList;
            set
            {
                if (_callInList != value)
                {
                    _callInList = value;
                    OnPropertyChanged(nameof(CallInList)); // עדכן את ה-UI אם הרשימה השתנתה
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public CallInListWindow()
        {
            InitializeComponent();
            DataContext = this;
            RefreshCallList();
        }
        private void RefreshCallList()
        {
            try
            {
                CallInList = queryCallList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load call list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update the SelectedCallStatus when ComboBox selection changes
            if (sender is ComboBox comboBox && comboBox.SelectedItem is BO.CallStatus selectedFilter)
            {
                SelectedCallStatus = selectedFilter;
                RefreshCallList();
            }
        }

        // Function to filter the list of calls
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




        // Filter the calls when the filter button is clicked
        private void FilterCalls_Click(object sender, RoutedEventArgs e)
        {
            // Update the CallList property with filtered results
            CallInList = queryCallList();
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

                        RefreshCallList();

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
            //try
            //{
            //    var bl = BlApi.Factory.Get().Call;
            //    bl.DeleteCall(selectedCall.CallId); // Delete the volunteer
            //    RefreshCallList();  // Refresh the list
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"An error occurred while deleting the volunteer: {ex.Message}",
            //                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
        private void callListObserver()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                RefreshCallList();  // Refresh the list when notified of changes
            });
        }
        // Add observer on window load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BlApi.Factory.Get().Call.AddObserver(callListObserver);
        }

        // Remove observer on window close
        private void Window_Closed(object sender, EventArgs e)
        {
            BlApi.Factory.Get().Call.RemoveObserver(callListObserver);
        }

    }

}


