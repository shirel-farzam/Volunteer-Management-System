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
                Call = (Id != 0)
                    ? s_bl.Call.Read(Id)
                    : new BO.Call()
                    {
                        Id = 0,
                        Type = CallType.None,
                        Description = string.Empty,
                        FullAddress = string.Empty,
                        OpenTime = DateTime.Now,
                        MaxEndTime = null,
                        Status = CallStatus.Open,

                    };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CallObserver()
        {
            try
            {
                int id = Call!.Id;
                Call = null;
                Call = s_bl.Call.Read(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reloading Call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Call != null && Call.Id != 0)
            {
                s_bl.Call.AddObserver(Call.Id, CallObserver);

            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (Call != null && Call.Id != 0)
            {
                s_bl.Call.RemoveObserver(Call.Id, CallObserver);
            }
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
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
              //  refresh();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Call added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
            Close();
        }

        private void UpdateCall()
        {
            try
            {
                var call = Call;

                s_bl.Call.Update(call);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    refresh();
                    MessageBox.Show("Call updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
            s_bl.Call.Read(0);
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