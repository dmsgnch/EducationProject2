using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EducationProject2.Models;
using EducationProject2.ViewModels;

namespace EducationProject2.Views
{
    public sealed partial class MainPage : Page
    {
        internal MainPageViewModel MainPageViewModel { get; }

        public MainPage()
        {
            InitializeComponent();

            MainPageViewModel = new MainPageViewModel();

            DataContext = MainPageViewModel;
        }
        
        private void OnDeletePersonButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Person person)
            {
                MainPageViewModel.DeletePersonCommand.Execute(person);
            }
        }
    }
}