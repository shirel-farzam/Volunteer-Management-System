using System;
using System.Windows;

namespace PL.Admin
{
    /// <summary>
    /// Interaction logic for AdminTransition.xaml
    /// </summary>
    public partial class AdminTransition : Window
    {
        public AdminTransition()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Navigate to the Volunteer Screen
        /// </summary>
        private void NavigateToVolunteerScreen_Click(object sender, RoutedEventArgs e)
        {
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
           
           new MainWindow().Show();
            this.Close();
           
        }
    }
}
