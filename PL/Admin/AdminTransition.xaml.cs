using System;
using System.Windows;
using PL.Volunteer;
using PL.VolunteerScreens;
using PL.VolunteerWindow;


namespace PL.Admin
{
    /// <summary>
    /// Interaction logic for AdminTransition.xaml
    /// </summary>
    public partial class AdminTransition : Window
    {
        private Window _previousWindow; // Variable to store a reference to the previous window

        public AdminTransition(int id, Window previousWindow)
        {
            InitializeComponent();
           Id = id;
            _previousWindow = previousWindow;
            this.DataContext = this;
        }

        public int Id { get; set; }
        private void NavigateToVolunteerScreen_Click(object sender, RoutedEventArgs e)
        {

            new VolunteerMainWindow(Id).Show();

            // יצירת חלון חדש למסך המתנדבים ופתיחתו
            //VolunteerScreenWindow volunteerWindow = new VolunteerScreenWindow(); // ודא שהחלון קיים
            //volunteerWindow.Show();
            //this.Close(); // סגירת חלון הנוכחי אם נדרש
        }

        /// <summary>
        /// Navigate to the Main Management Screen
        /// </summary>
        private void NavigateToManagementScreen_Click(object sender, RoutedEventArgs e)
        {

           new MainWindow(Id,this).Show();
            this.Hide();
           
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
