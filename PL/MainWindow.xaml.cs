using BO;
using PL.Volunteer;
using PL.Call;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _previousWindow; // Variable to store a reference to the previous window
        public int Id { get; set; }

        public MainWindow(int Manegr,Window previousWindow)
        {
            InitializeComponent();
            _previousWindow= this;
            Id = Manegr;
            this.DataContext = this;

        }

        public TimeSpan MaxRange
        {
            get { return (TimeSpan)GetValue(MaxRangeProperty); }
            set { SetValue(MaxRangeProperty, value); }
        }

        public static readonly DependencyProperty MaxRangeProperty =
            DependencyProperty.Register("MaxRange", typeof(TimeSpan), typeof(MainWindow),
                new PropertyMetadata(TimeSpan.FromHours(1), OnRiskRangeChanged));
        
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        public int[] CountCall
        {
            get { return (int[])GetValue(CountCallProperty); }
            set { SetValue(CountCallProperty, value); }
        }
        public static readonly DependencyProperty CountCallProperty =
        DependencyProperty.Register("CountCall", typeof(int[]), typeof(MainWindow));


        // Event handler when the RiskRange property changes
        private static void OnRiskRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MainWindow window && e.NewValue is TimeSpan newRange)
            {
                s_bl.Admin.Definition(newRange); // Update Risk Range via business logic
            }
        }


        // Handles the "Add One Minute" button click
        // Event handler for "Add One Minute" button click
        private void AddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.MINUTE);
        }

        // Event handler for "Add One Hour" button click
        private void AddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.HOUR);
        }

        // Event handler for "Add One Day" button click
        private void AddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.DAY);
        }

        // Event handler for "Add One Month" button click
        private void AddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.MONTH);
        }

        // Event handler for "Add One Year" button click
        private void AddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.YEAR);
        }

        // Handles the "Update Risk Range" button click
        private void UpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.Definition(MaxRange); // Update the Risk Range in business logic
           MessageBox.Show($"Risk Range updated to: {MaxRange}"); // Show confirmation message
        }

        // Handles the "Initialize Database" button click
        private void InitializeDatabase_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.initialization();
            MessageBox.Show($"Data Intilization Completed Succsessfully "); // Show confirmation message
           
        }

        // Handles the "Reset Database" button click
        private void ResetDatabase_Click(object sender, RoutedEventArgs e)
        {
            
            s_bl.Admin.Reset();
            MessageBox.Show($"Data Rest Completed Succsessfully "); // Show confirmation message
        }

        private void btnVolunteer_Click(object sender, RoutedEventArgs e)
        {
            //s_bl.Admin.initialization();
            new VolunteerInListWindow().Show();
        }
        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }
        private void configObserver()
        {
            MaxRange = s_bl.Admin.GetMaxRange();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);

        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            
            CurrentTime = s_bl.Admin.GetClock();
            MaxRange = s_bl.Admin.GetMaxRange();
            CountCall = s_bl.Call.CountCall();

            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);

        }
        private void ListCall_Click(object sender, RoutedEventArgs e)
        {
              new  CallInListWindow().Show(); 
        }
        private void StartSimulator_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _previousWindow.Show(); // Show the previous window
            this.Close(); // Close the current window
        }


    }
}
