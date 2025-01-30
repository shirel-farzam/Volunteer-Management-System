using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace PL.VolunteerWindow
{
    public partial class VolunteerWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        public string ButtonText { get; set; }
        public int Id { get; set; }

        public BO.Volunteer Volunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set
            {
                SetValue(CurrentVolunteerProperty, value);
                OnPropertyChanged(nameof(Volunteer));
            }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerWindow),
                new PropertyMetadata(null));

        public IEnumerable<BO.Distance> DistanceTypes =>
            Enum.GetValues(typeof(BO.Distance)).Cast<BO.Distance>();

        public IEnumerable<BO.Role> Roles =>
            Enum.GetValues(typeof(BO.Role)).Cast<BO.Role>();

        public VolunteerWindow(int id = 0)
        {
            Id = id;
            ButtonText = Id == 0 ? "Add" : "Update";
            DataContext = this;

            InitializeComponent();

            try
            {
                Volunteer = (Id != 0)
                    ? s_bl.Volunteer.Read(Id)
                    : new BO.Volunteer()
                    {
                        Id = 0,
                        FullName = string.Empty,
                        PhoneNumber = string.Empty,
                        Email = string.Empty,
                        FullAddress = null,
                        Password = null,
                        Latitude = null,
                        Longitude = null,
                        Job = BO.Role.Volunteer,
                        Active = false,
                        TypeDistance = BO.Distance.Aerial,
                        TotalHandledCalls = 0,
                        TotalCanceledCalls = 0,
                        TotalExpiredCalls = 0,
                        CurrentCall = null
                    };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void VolunteerObserver()
        {
            //try
            //{
            //    int id = Volunteer!.Id;
            //    Volunteer = null;
            //    Volunteer = s_bl.Volunteer.Read(id);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error reloading volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                try
                    {
                        int id = Volunteer!.Id;
                        Volunteer = null;
                        Volunteer = s_bl.Volunteer.Read(id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reloading volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Volunteer != null && Volunteer.Id != 0)
            {
                s_bl.Volunteer.AddObserver(Volunteer.Id, VolunteerObserver);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (Volunteer != null && Volunteer.Id != 0)
            {
                s_bl.Volunteer.RemoveObserver(Volunteer.Id, VolunteerObserver);
            }
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (Id == 0)
            {
                AddVolunteer();
            }
            else
            {
                UpdateVolunteer();
            }
        }

        private void AddVolunteer()
        {
            try
            {
                var volunteer = Volunteer;
                s_bl.Volunteer.AddVolunteer(volunteer);
                refresh();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
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

        private void UpdateVolunteer()
        {
            try
            {
                var volunteer = Volunteer;

                s_bl.Volunteer.Update(volunteer.Id, volunteer);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    refresh();
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
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

        private void refresh()
        {
            s_bl.Volunteer.ReadAll(null, null);
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
    }
}
