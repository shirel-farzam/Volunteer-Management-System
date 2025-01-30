using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace PL.CallWindow
{
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

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

        BO.CallStatus Status
        {
            get => (BO.CallStatus)GetValue(CallStatusProperty);
            init => SetValue(CallStatusProperty, value);
        }
        public static readonly DependencyProperty CallStatusProperty =
         DependencyProperty.Register(nameof(CallStatus), typeof(BO.CallStatus), typeof(CallWindow));

        public IEnumerable<BO.Distance> DistanceTypes =>
            Enum.GetValues(typeof(BO.Distance)).Cast<BO.Distance>();

        public IEnumerable<BO.Role> Roles =>
            Enum.GetValues(typeof(BO.Role)).Cast<BO.Role>();

        public CallWindow(int id = 0)
        {
            Id = id;
            ButtonText = Id == 0 ? "Add" : "Update";
            DataContext = this;
            try
            {
                Call = (id != 0) ? s_bl.Call.Read(id)! : new BO.Call() { Id = 0, Type = BO.CallType.None, Description = "", FullAddress = "", Latitude = 0, Longitude = 0, OpenTime = DateTime.Now };
                Status = id == 0 ? BO.CallStatus.Open : Call.Status;
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Call = null;
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            s_bl.Call.AddObserver(Call!.Id, CallObserver);
            InitializeComponent();
        }

        private void CallObserver()
        {

            //int id = Call!.Id;
            //Call = null;
            //Call = s_bl.Call.Read(id);
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    int id = Call!.Id;
                    Call = null;
                    Call = s_bl.Call.Read(id);
                });
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
            
                if (ButtonText == "Add")
                    try
                    {
                        s_bl.Call.AddCall(Call!);
                        MessageBox.Show($"Call was successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    catch (BO.BlAlreadyExistsException ex)
                    {
                        MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                else
                    try
                    {
                        s_bl.Call.Update(Call!);
                        MessageBox.Show($"Call {Call?.Id} was successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
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