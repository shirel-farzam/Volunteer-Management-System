using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace PL.CallWindow
{
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public string ButtonText { get; set; }

        public int Id { get; set; }

        public BO.Call Call
        {
            get { return (BO.Call)GetValue(CurrentCallProperty); }
            set
            {
                SetValue(CurrentCallProperty, value);
                OnPropertyChanged(nameof(Call));
            }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("Call", typeof(BO.Call), typeof(CallWindow),
                new PropertyMetadata(null));

        public ObservableCollection<BO.CallAssignmentInList> CallAssignments { get; set; } = new ObservableCollection<BO.CallAssignmentInList >();

        public IEnumerable<BO.Distance> DistanceTypes =>
            Enum.GetValues(typeof(BO.Distance)).Cast<BO.Distance>();

        public IEnumerable<BO.Role> Roles =>
            Enum.GetValues(typeof(BO.Role)).Cast<BO.Role>();

        public CallWindow(int id = 0)
        {
            Id = id;
            ButtonText = Id == 0 ? "Add" : "Update";
            DataContext = this;
            InitializeComponent();
            try
            {
                if (Id != 0)
                {
                    Call = s_bl.Call.Read(Id);
                    LoadCallAssignments();
                }
                else
                {
                    Call = new BO.Call
                    {
                        Id = 0,
                        Type = CallType.None,
                        Description = string.Empty,
                        FullAddress = string.Empty,
                        Latitude = null,
                        Longitude = null
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCallAssignments()
        {
            try
            {
                var assignments = s_bl.Call.GetCallAssignments(Id);
                CallAssignments.Clear();
                foreach (var assignment in assignments)
                {
                    CallAssignments.Add(assignment);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading assignments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VolunteerObserver()
        {
            try
            {
                int id = Call!.Id;
                Call = null;
                Call = s_bl.Call.Read(id);
                LoadCallAssignments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reloading volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Call != null && Call.Id != 0)
            {
                s_bl.Volunteer.AddObserver(Call.Id, VolunteerObserver);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (Call != null && Call.Id != 0)
            {
                s_bl.Volunteer.RemoveObserver(Call.Id, VolunteerObserver);
            }
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCallFields()) return;

            if (Id == 0)
            {
                AddCall();
            }
            else
            {
                UpdateCall();
            }
        }

        private void AddCall()
        {
            try
            {
                s_bl.Call.AddCall(Call);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Call added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private void UpdateCall()
        {
            try
            {
                if (!CanUpdateCall())
                {
                    MessageBox.Show("Cannot update call in its current status.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                s_bl.Call.Update(Call);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Call updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private bool ValidateCallFields()
        {
            if (string.IsNullOrWhiteSpace(Call.Description))
            {
                MessageBox.Show("Description cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Call.FullAddress))
            {
                MessageBox.Show("Address cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool CanUpdateCall()
        {
            return Call.Status is CallStatus.Open or CallStatus.OpenRisk ||
                   (Call.Status is CallStatus.InProgress or CallStatus.InProgressRisk && Call.MaxEndTime != null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void cbDistanceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.Distance selectedDistanceType)
            {
                Console.WriteLine($"Selected DistanceType: {selectedDistanceType}");
            }
        }

        private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.Role selectedRole)
            {
                Console.WriteLine($"Selected Role: {selectedRole}");
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
