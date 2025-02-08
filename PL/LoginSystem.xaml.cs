using BO;
using PL.Admin;
using PL.VolunteerScreens;
using PL.VolunteerWindow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL
{
    public partial class LoginSystem : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private static bool _isAdminLoggedIn = false;

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
        private void LoginSystem_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetDayMode(); // הפעלת מצב יום לאחר טעינת החלון
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while setting the mode: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                Role role = s_bl.Volunteer.Login(Id, Password);

                if (role == Role.Volunteer)
                {
                     MessageBox.Show("Welcome, Volunteer!");
                        new VolunteerMainWindow(Id).Show(); 
                 
                }
                else
                {
                   
                   MessageBox.Show("Welcome, Admin!");
                        new AdminTransition(Id, this).Show(); 
                   
                }
        
            }
            catch (BO.BlNullPropertyException ex)
            {
                
                MessageBox.Show("The ID you entered does not exist. Please try again.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlWrongInputException ex)
            {
               
                
                MessageBox.Show("The password you entered is incorrect. Please try again.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Password = string.Empty;
            }
            catch (Exception ex)
            {
               
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        private bool isDayMode = true;

        private void SwitchMode_Click(object sender, RoutedEventArgs e)
        {
            if (isDayMode)
            {
                SetNightMode();
            }
            else
            {
                SetDayMode();
            }
            isDayMode = !isDayMode;
        }
        // שחרור מנהל בעת יציאה
        //public static void ReleaseAdmin()
        //{
        //    _isAdminLoggedIn = false;
        //}
        private DispatcherTimer _timer;

        private void ShowInstructions_Click(object sender, RoutedEventArgs e)
        {
            // Hide the Button immediately after click
            ((Button)sender).Visibility = Visibility.Collapsed;

            // Find the Grid's children
            var parentGrid = (Grid)((Button)sender).Parent;

            // Display the Ellipse and TextBlock
            foreach (var child in parentGrid.Children)
            {
                if (child is Ellipse ellipse)
                {
                    ellipse.Visibility = Visibility.Visible;
                }
                else if (child is TextBlock textBlock)
                {
                    textBlock.Visibility = Visibility.Visible;
                }
            }
        }


        private void enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BO.Volunteer currentVolunteer = null;
                try
                {
                    currentVolunteer = s_bl.Volunteer.Read(Id);
                }
                catch (BO.BlDoesNotExistException ex)
                {

                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                if (currentVolunteer != null)
                {
                    if (currentVolunteer!.Password != Password)
                        MessageBox.Show("wrong password!", "Error", MessageBoxButton.OK);
                    else
                    {
                       
                        int numericId = Id;

                        if (currentVolunteer.Job == BO.Role.Manager)
                            try
                            {
                                MessageBox.Show("Welcome, Admin!");
                                new AdminTransition(numericId,this).Show();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
                            }


                        else
                            {MessageBox.Show("Welcome, Volunteer!");
                            new VolunteerMainWindow(numericId).Show();
}
                    }
                }
            }
        }
        private void SetDayMode()
        {
            Uri dayModeUri = new Uri("Themes/ColorLight.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Clear();  // מוחק את המשאבים הקיימים
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = dayModeUri });

        }

        private void SetNightMode()
        {
            Uri nightModeUri = new Uri("Themes/DarkColor.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Clear();  // מוחק את המשאבים הקיימים
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = nightModeUri });
        }
    }

}


