using BO;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ChooseCallToTreat.xaml
    /// </summary>
    public partial class ChooseCallToTreat : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.OpenCallInListField OpenCallInList { get; set; } = BO.OpenCallInListField.Id;
    public BO.OpenCallInList? SelectedCall { get; set; }
    public int VolunteerId { get; set; }

    public IEnumerable<BO.OpenCallInList> OpenCallList
    {
        get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallListProperty); }
        set { SetValue(OpenCallListProperty, value); }
    }

        public static readonly DependencyProperty OpenCallListProperty =
            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(ChooseCallToTreat), new PropertyMetadata(null));

        public BO.CallType? TypeCallInList { get; set; }

        public ChooseCallToTreat(int id)
        {
            VolunteerId = id;
            //InitializeComponent();
            try
            {
                SelectedCalls = s_bl.Volunteer.Read(VolunteerId);
                if (SelectedCalls.CurrentCall != null)
                    Call = s_bl.Call.Read(SelectedCalls.CurrentCall.CallId);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                SelectedCalls = null;
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            this.DataContext = this;
            s_bl.Volunteer.AddObserver(UserId, VolunteerObserver);
            if (Call != null)
                s_bl.Call.AddObserver(UserId, CallObserver);
            InitializeComponent();
        }

        private void Call_Filter(object sender, SelectionChangedEventArgs e)
        {
            // עדכון פילטר הסינון
            OpenCallInList = (BO.OpenCallInListField)(((ComboBox)sender).SelectedItem);

            // שליפת הקריאות עם הפילטר החדש
            OpenCallList = s_bl?.Call.GetOpenCallsForVolunteer(VolunteerId, TypeCallInList, OpenCallInList)!;
        }

        private void CallSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
            //QueryCallList();
            QueryCall();
    }

    //    private void QueryCallList()
    //    {

    //        OpenCallList = (OpenCallInList == BO.OpenCallInListField.Id) ?
    //            s_bl?.Call.GetOpenCallsForVolunteer(VolunteerId, null, null)! :
    //            s_bl?.Call.GetOpenCallsForVolunteer(VolunteerId, TypeCallInList, OpenCallInList)!;
    //    }

    //    private void CallListObserver() => QueryCallList();

    //    private void Window_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        // הוספת צופה לקריאות
    //        s_bl.Call.AddObserver(CallListObserver);
    //    }

    //    private void Window_Closed(object sender, EventArgs e)
    //    {
    //                    s_bl.Call.RemoveObserver(CallListObserver);
    //    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // הצגת תיאור הקריאה הנבחרת
        if (SelectedCall != null)
        {
            MessageBox.Show(SelectedCall.Description, $"Description {SelectedCall.Id}", MessageBoxButton.OK);
        }
    }

    //    private void BtnChoose_Click(object sender, RoutedEventArgs e)
    //    {
    //        try
    //        {
    //            // בחירת קריאה לטיפול
    //            if (SelectedCall != null)
    //            {
    //                s_bl.Call.ChooseCallForTreat(VolunteerId, SelectedCall.Id);
    //                MessageBox.Show($"Call {SelectedCall.Id} was successfully Chosen!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    //                this.Close();
    //            }
    //            else
    //            {
    //                MessageBox.Show("No call selected", "Choose Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
    //            }
    //        }
    //        catch (BO.BlAlreadyExistsException ex)
    //        {
    //            // טיפול במקרים של קיומה של קריאה כזו כבר
    //            MessageBox.Show(ex.Message, "Choose Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
    //            this.Close();
    //        }
    //        catch (Exception ex)
    //        {
    //            // טיפול בשגיאות כלליות
    //            MessageBox.Show(ex.Message, "Choose Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
    //        }
    //    }
    //}
  
        public BO.Volunteer? SelectedCalls
        {
            get { return (BO.Volunteer?)GetValue(SelectedCallsProperty); }
            set { SetValue(SelectedCallsProperty, value); }
        }

        public static readonly DependencyProperty SelectedCallsProperty =
            DependencyProperty.Register("SelectedCalls", typeof(BO.Volunteer), typeof(ChooseCallToTreat), new PropertyMetadata(null));

        public BO.Call? Call
        {
            get { return (BO.Call?)GetValue(CallPropertyForChooseCallToTreat); }
            set { SetValue(CallPropertyForChooseCallToTreat, value); }
        }

        /// <summary>
        /// Dependency property for SelectedCalls
        /// </summary>
        public static readonly DependencyProperty CallPropertyForChooseCallToTreat =
            DependencyProperty.Register("Call", typeof(BO.Call), typeof(ChooseCallToTreat), new PropertyMetadata(null));


        public int UserId { get; set; }

        //public ChooseCallToTreat(int id = 0)
        //{

        //    UserId = id;

        //    try
        //    {
        //        SelectedCalls = s_bl.Volunteer.Read(UserId);
        //        if (SelectedCalls.CurrentCall != null)
        //            Call = s_bl.Call.Read(SelectedCalls.CurrentCall.CallId);
        //    }
        //    catch (BO.BlDoesNotExistException ex)
        //    {
        //        SelectedCalls = null;
        //        MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //        this.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //    }

        //    this.DataContext = this;
        //    s_bl.Volunteer.AddObserver(UserId, VolunteerObserver);
        //    if (Call != null)
        //        s_bl.Call.AddObserver(UserId, CallObserver);
        //    InitializeComponent();
        //}

        private void CallObserver()
            => QueryCall();

        private void QueryVolunteer()
        { SelectedCalls = s_bl.Volunteer.Read(UserId); }

        private void QueryCall()
        {
            QueryVolunteer();
            if (Call != null)
            {
                if (SelectedCalls.CurrentCall == null || SelectedCalls.CurrentCall.CallId != Call.Id)
                {
                    s_bl.Call.RemoveObserver(Call.Id, CallObserver);

                }

            }
            if (SelectedCalls.CurrentCall != null && Call != null && SelectedCalls.CurrentCall.CallId != Call.Id)
            {
                s_bl.Call.AddObserver(SelectedCalls.CurrentCall.CallId, CallObserver);
            }
            else

                Call = null;
            if (SelectedCalls.CurrentCall != null)
            {

                Call = s_bl.Call.Read(SelectedCalls.CurrentCall.CallId);
            }
        }

        private void VolunteerObserver()
        {
            //SelectedCalls = s_bl.Volunteers.Read(UserId);
            QueryCall();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedCalls!.Id != 0)
                s_bl.Volunteer.AddObserver(SelectedCalls!.Id, VolunteerObserver);
            if (Call != null)
                s_bl.Call.AddObserver(Call.Id, CallObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(SelectedCalls!.Id, VolunteerObserver);
            if (Call != null)
                s_bl.Call.RemoveObserver(Call.Id, CallObserver);
        }

        private void btnCallsHistory_Click(object sender, RoutedEventArgs e)
        {
            new CallHistory(UserId, this).Show();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.Update(SelectedCalls.Id, SelectedCalls!);
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

        private void BtnChoose_Click(object sender, RoutedEventArgs e)
        {
            new ChooseCallToTreat(UserId).Show();

        }

        private void btnClosed_Call(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.CloseTreat(UserId, SelectedCalls.CurrentCall.Id);
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

        private void btnCansel_Call(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.CancelTreat(UserId, SelectedCalls.CurrentCall.Id);
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
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            var MainWindow = Application.Current.MainWindow;
            if (MainWindow != null)
            {
                MainWindow.Show();
            }

            // סגור את החלון הנוכחי
            this.Close();
        }

        

    }
}
