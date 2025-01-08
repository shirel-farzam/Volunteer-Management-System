using BO;
using PL.Admin;
using PL.VolunteerScreens;
using PL.VolunteerWindow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PL
{
    public partial class LoginSystem : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public Role role { get; set; }
        public int Id
        {
            get { return (int)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(LoginSystem), new PropertyMetadata(0));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(LoginSystem), new PropertyMetadata(""));

        // Event handler without sender
        private void OnPasswordChanged(object _, RoutedEventArgs __)
        {
            // Iterate over children to find the PasswordBox
            PasswordBox passwordBox = GetPasswordBox();
            if (passwordBox != null)
            {
                // Update the Password property when the PasswordBox content changes
                Password = passwordBox.Password;
            }
        }

        // Helper method to locate PasswordBox in the UI hierarchy
        private PasswordBox GetPasswordBox()
        {
            return FindVisualChild<PasswordBox>(this);
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T correctlyTyped)
                {
                    return correctlyTyped;
                }
                var descendant = FindVisualChild<T>(child);
                if (descendant != null)
                {
                    return descendant;
                }
            }
            return null;
        }

        public LoginSystem()
        {
            InitializeComponent();
            DataContext = this;
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // הנחת עבודה: s_bl הוא האובייקט שמטפל בלוגיקה העסקית
                // הפונקציה Login מחזירה Role (Volunteer או Admin)
                Role role = s_bl.Volunteer.Login(Id, Password);

                if (role == Role.Volunteer)
                {
                    MessageBox.Show("Welcome, Volunteer!");
                    new VolunteerMainWindow(Id).Show();
                }
                else
                {
                    MessageBox.Show("Welcome, Admin!");
                    new AdminTransition().Show();
                }

            }
            catch (BO.BlNullPropertyException ex)
            {
                // טיפול במקרה שבו ה-ID לא קיים
                MessageBox.Show("The ID you entered does not exist. Please try again.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlWrongInputException ex)
            {
                // טיפול במקרה שבו הסיסמה שגויה
                
                MessageBox.Show("The password you entered is incorrect. Please try again.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Password = string.Empty;
            }
            catch (Exception ex)
            {
                // טיפול במקרה של שגיאה כללית
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        


    }

}
