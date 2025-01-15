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
            QueryCallList();
        }

        private void QueryCallList()
        {
            // אם לא נבחר פילטר, תבצע שליפה של כל הקריאות
            OpenCallList = (OpenCallInList == BO.OpenCallInListField.Id) ?
                s_bl?.Call.GetOpenCallsForVolunteer(VolunteerId, null, null)! :
                s_bl?.Call.GetOpenCallsForVolunteer(VolunteerId, TypeCallInList, OpenCallInList)!;
        }

        private void CallListObserver() => QueryCallList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // הוספת צופה לקריאות
            s_bl.Call.AddObserver(CallListObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // הסרת הצופה
            s_bl.Call.RemoveObserver(CallListObserver);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // הצגת תיאור הקריאה הנבחרת
            if (SelectedCall != null)
            {
                MessageBox.Show(SelectedCall.Description, $"Description {SelectedCall.Id}", MessageBoxButton.OK);
            }
        }

        private void BtnChoose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // בחירת קריאה לטיפול
                if (SelectedCall != null)
                {
                    s_bl.Call.ChooseCallForTreat(VolunteerId, SelectedCall.Id);
                    MessageBox.Show($"Call {SelectedCall.Id} was successfully Chosen!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No call selected", "Choose Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (BO.BlAlreadyExistsException ex)
            {
                // טיפול במקרים של קיומה של קריאה כזו כבר
                MessageBox.Show(ex.Message, "Choose Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.Close();
            }
            catch (Exception ex)
            {
                // טיפול בשגיאות כלליות
                MessageBox.Show(ex.Message, "Choose Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
