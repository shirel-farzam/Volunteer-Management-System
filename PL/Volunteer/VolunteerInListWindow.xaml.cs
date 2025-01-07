    using BO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.ComponentModel;
    using System.Windows.Controls;
    using DalTest;
    using PL.VolunteerWindow;
    using System.Windows.Input;

    namespace PL.Volunteer
    {
        public partial class VolunteerInListWindow : Window, INotifyPropertyChanged
        {
            private VolunteerInListField _selectedFilter = VolunteerInListField.None;  // Default to None (no filter)
            public BO.VolunteerInList? SelectedVolunteer { get; set; }

            // SelectedFilter property
            public VolunteerInListField SelectedFilter
            {
                get { return _selectedFilter; }
                set
                {
                    if (_selectedFilter != value)
                    {
                        _selectedFilter = value;
                        OnPropertyChanged(nameof(SelectedFilter));  // Notify the UI of the property change
                        UpdateVolunteerList();  // Update the list when the filter changes
                    }
                }
            }

            // VolunteerInList property (DependencyProperty)
            public IEnumerable<BO.VolunteerInList> VolunteerInList
            {
                get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerInListFieldListProperty); }
                set { SetValue(VolunteerInListFieldListProperty, value); }
            }

            public static readonly DependencyProperty VolunteerInListFieldListProperty =
                DependencyProperty.Register(
                    "VolunteerInList",typeof(IEnumerable<BO.VolunteerInList>),
                    typeof(VolunteerInListWindow),
                    new PropertyMetadata(null));

            // Constructor
            public VolunteerInListWindow()
            {
                InitializeComponent();
                DataContext = this;
                UpdateVolunteerList();  // Load the volunteer list without any filter initially
            }

            // Handle ComboBox selection change event to update the filter
            private void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                // Handle selection change and update the SelectedFilter property
                if (sender is ComboBox comboBox && comboBox.SelectedItem is VolunteerInListField selectedFilter)
                {
                    SelectedFilter = selectedFilter; // Update the SelectedFilter property
                }
            }

            // Update the volunteer list based on the selected filter
            private void UpdateVolunteerList()
            {
                try
                {
                    IEnumerable<BO.VolunteerInList> volunteers = queryVolunteerList();
                    VolunteerInList = volunteers;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while loading the volunteer list: {ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Filtering logic based on the selected filter
            private IEnumerable<BO.VolunteerInList> queryVolunteerList()
            {
                IEnumerable<BO.VolunteerInList> volunteers;

                switch (SelectedFilter)
                {
                    case VolunteerInListField.Id:
                        volunteers = BlApi.Factory.Get().Volunteer.ReadAll(null, VolunteerInListField.Id).OrderBy(v => v.Id);
                        break;
                    case VolunteerInListField.FullName:
                        volunteers = BlApi.Factory.Get().Volunteer.ReadAll(null, VolunteerInListField.FullName).OrderBy(v => v.FullName);
                        break;
                    case VolunteerInListField.Active:
                        volunteers = BlApi.Factory.Get().Volunteer.ReadAll(true, VolunteerInListField.Active).Where(v => v.Active);
                        break;
                case VolunteerInListField.CurrentCallType:
                    volunteers = BlApi.Factory.Get().Volunteer.ReadAll(true, VolunteerInListField.CurrentCallType).OrderBy(v => v.CurrentCallType);
                    break;
                case VolunteerInListField.None:  // No filter (default)
                        volunteers = BlApi.Factory.Get().Volunteer.ReadAll(null, null);
                        break;
                    default:
                        volunteers = BlApi.Factory.Get().Volunteer.ReadAll(null, null);
                        break;
                }

                return volunteers;
            }

            // INotifyPropertyChanged implementation
            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            // Add Volunteer button click event
            private void AddVolunteerButton_Click(object sender, RoutedEventArgs e)
            {
                new VolunteerWindow.VolunteerWindow().Show();
            }

            // Double-click on volunteer list to view details
            private void lsvVolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                if (SelectedVolunteer != null)
                    new VolunteerWindow.VolunteerWindow(SelectedVolunteer.Id).Show();
            }

            // Delete Volunteer by ID
            private void DeleteVolunteer(int volunteerId)
            {
                try
                {
                    var bl = BlApi.Factory.Get().Volunteer;
                    bl.DeleteVolunteer(volunteerId); // Delete the volunteer
                    UpdateVolunteerList();  // Refresh the list
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the volunteer: {ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Delete button click event
            private void DeleteButton_Click(object sender, RoutedEventArgs e)
            {
                if (sender is Button button && button.Tag is int volunteerId)
                {
                    var result = MessageBox.Show("Are you sure you want to delete this volunteer?",
                        "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        DeleteVolunteer(volunteerId); // Call the delete method
                    }
                }
            }

            private void Window_Loaded(object sender, RoutedEventArgs e)
            {
                BlApi.Factory.Get().Volunteer.AddObserver(volunteerListObserver);
            }

            private void Window_Closed(object sender, EventArgs e)
            {
                BlApi.Factory.Get().Volunteer.RemoveObserver(volunteerListObserver);
            }

            // Observer method to refresh volunteer list
            private void volunteerListObserver()
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateVolunteerList();  // Refresh the volunteer list when notified of changes
                });
            }
        }
    }
