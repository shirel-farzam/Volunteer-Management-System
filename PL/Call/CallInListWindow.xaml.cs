using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using DO;
using PL.CallWindow;

namespace PL.Call
{
    public partial class CallInListWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _previousWindow; // Variable to store a reference to the previous window
        public IEnumerable<BO.CallInList> CallInList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallInList", typeof(IEnumerable<BO.CallInList>), typeof(CallInListWindow), new PropertyMetadata(null));

        public BO.CallInList? SelectedCallforupdate { get; set; }

        public BO.CallStatus? SelectedCallStatus { get; set; }

        public BO.CallInListField CallInListField { get; set; } = BO.CallInListField.Id;

        public int Id { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public CallInListWindow(int id = 0, Window previousWindow=null)
        {
            Id = id;
            InitializeComponent();
            DataContext = this;
            _previousWindow = previousWindow;
        }

        private void CallSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryCallList();
        }

        private void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallInListField = (BO.CallInListField)(((ComboBox)sender).SelectedItem);
            CallInList = s_bl?.Call.GetCallInLists(BO.CallInListField.Status, SelectedCallStatus, CallInListField)!;
        }

        private void QueryCallList()
        {
            CallInList = (CallInListField == BO.CallInListField.Id)
                ? s_bl?.Call.GetCallInLists(null, null, null)!
                : s_bl?.Call.GetCallInLists(BO.CallInListField.Status, SelectedCallStatus, CallInListField)!;
        }
        private void CallListObserver() => QueryCallList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(CallListObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(CallListObserver);
        }

        private void CallsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCallforupdate?.CallId != null)
            {
                new CallWindow.CallWindow(SelectedCallforupdate.CallId).Show();
            }
            else
            {
                MessageBox.Show("No call selected for editing.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddCallButton_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow.CallWindow().Show();
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to delete this call?", "Reset Confirmation",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.DeleteCall(SelectedCallforupdate.CallId);
                }
                catch (BO.BlDeleteNotPossibleException ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to cancel this assignment?", "Reset Confirmation",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.CancelTreat(Id, (int)SelectedCallforupdate.Id);
                }
                catch (BO.BlDeleteNotPossibleException ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_previousWindow != null)
            {
                _previousWindow.Show(); // Show the previous window
                this.Hide(); // Close the current window
            }
            else
            {
                MessageBox.Show("Previous window is null!");
            }
        }
    }
}
