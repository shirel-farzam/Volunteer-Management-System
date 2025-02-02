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
using System.Windows.Threading;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _previousWindow; // Variable to store a reference to the previous window
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private volatile DispatcherOperation? _observerOperationCon = null; //stage 7
        private volatile DispatcherOperation? _observerOperationCall = null; //stage 7

        public int Id { get; set; }
        // public static int OpenWindowCount { get; private set; } = 0;
        public static Boolean IsOpen { get; set; }=false;
        public MainWindow(int Manegr,Window previousWindow)
        {
            if (IsOpen)
                throw new Exception("There already is one manager in the system");
            else IsOpen = true;
             InitializeComponent();
            _previousWindow = previousWindow;
            Id = Manegr;
            DataContext = this;

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

        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow));
        public bool IsRuning
        {
            get { return (bool)GetValue(IsRuningProperty); }
            set { SetValue(IsRuningProperty, value); }
        }
        public static readonly DependencyProperty IsRuningProperty =
            DependencyProperty.Register("IsRuning", typeof(bool), typeof(MainWindow));

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
            // Display confirmation dialog
            var result = MessageBox.Show(
                "Are you sure you want to initialize the database? This action may overwrite existing data.",
                "Initialize Database Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            // Check the user's response
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Admin.initialization();
                    MessageBox.Show("Data initialization completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error initializing database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // User selected "No", do nothing
                MessageBox.Show("Data initialization canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        // Handles the "Reset Database" button click
        private void ResetDatabase_Click(object sender, RoutedEventArgs e)
        {
            // Display confirmation dialog
            var result = MessageBox.Show(
                "Are you sure you want to reset the database? This action cannot be undone.",
                "Reset Database Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            // Check the user's response
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Admin.Reset();
                    MessageBox.Show("Data reset completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error resetting database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // User selected "No", do nothing
                MessageBox.Show("Data reset canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void btnVolunteer_Click(object sender, RoutedEventArgs e)
        {
            //s_bl.Admin.initialization();
            new VolunteerInListWindow(Id,this).Show();
        }
        private void clockObserver()
        {
            //CurrentTime = s_bl.Admin.GetClock();
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                });

        }
        private void configObserver()
        {
            //MaxRange = s_bl.Admin.GetMaxRange();
            if (_observerOperationCon is null || _observerOperationCon.Status == DispatcherOperationStatus.Completed)
                _observerOperationCon = Dispatcher.BeginInvoke(() =>
                {
                    MaxRange = s_bl.Admin.GetMaxRange();
                });
        }
        private void callObserver()
        {
            //MaxRange = s_bl.Admin.GetMaxRange();
            if (_observerOperationCall is null || _observerOperationCall.Status == DispatcherOperationStatus.Completed)
                _observerOperationCall = Dispatcher.BeginInvoke(() =>
                {
                    CountCall = s_bl.Call.CountCall();
                });
        }
        private void Window_Closed(object sender, EventArgs e)
        {
           // OpenWindowCount--; // הפחתת המונה כאשר החלון נסגר
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            IsOpen = false; 

            if (IsRuning)
            {
                s_bl.Admin.StopSimulator(); // עצירת סימולטור
                IsRuning = false; // עדכון מצב
            }


        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            s_bl.Admin.SetClock(now);

            CurrentTime = s_bl.Admin.GetClock();
            MaxRange = s_bl.Admin.GetMaxRange();
            CountCall = s_bl.Call.CountCall();
            Interval = 50;
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Call.AddObserver(callObserver);

            InitializeClock();

        }
        private void ListCall_Click(object sender, RoutedEventArgs e)
        {
              new  CallInListWindow(Id,this).Show(); 
        }
        private void StartSimulator_Click(object sender, RoutedEventArgs e)
        {
            if (IsRuning)
            {
                s_bl.Admin.StopSimulator();
                IsRuning = false;
            }
            else
            {
                s_bl.Admin.StartSimulator(Interval);
                IsRuning = true;
            }

        }
        //private void BackButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_previousWindow != null)
        //    {
        //        _previousWindow.Show(); // Show the previous window
        //        this.Hide(); // Close the current window
        //    }
        //    else
        //    {
        //        MessageBox.Show("Previous window is null!");
        //    }
        //}
        private DispatcherTimer _timer;

        private void InitializeClock()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // עדכון כל שנייה
            };
            _timer.Tick += (sender, args) =>
            {
                var hebrewCulture = new System.Globalization.CultureInfo("he-IL");
                hebrewCulture.DateTimeFormat.Calendar = new System.Globalization.HebrewCalendar();
                CurrentTime = DateTime.Now; // עדכון הזמן
            };
            _timer.Start();
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
