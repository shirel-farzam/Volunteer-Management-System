using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using DalTest;

namespace PL.Volunteer
{
    public partial class VolunteerInListWindow : Window, INotifyPropertyChanged
    {
        private VolunteerInListField _selectedFilter = VolunteerInListField.None;  // Default to None (no filter)

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
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolInListProperty); }
            set { SetValue(VolInListProperty, value); }
        }

        public static readonly DependencyProperty VolInListProperty =
            DependencyProperty.Register(
                "VolunteerInList",
                typeof(IEnumerable<BO.VolunteerInList>),
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
    }
}
