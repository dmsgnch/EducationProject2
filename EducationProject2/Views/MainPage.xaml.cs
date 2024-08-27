using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EducationProject2.Models;
using EducationProject2.ViewModels;

namespace EducationProject2.Views
{
    public sealed partial class MainPage : Page
    {
        private readonly MainPageViewModel _mainPageViewModel;
        
        public MainPage()
        {
            InitializeComponent();

            _mainPageViewModel = new MainPageViewModel();

            DataContext = _mainPageViewModel;
        }
        
        private void OnDeletePersonButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Person person)
            {
                _mainPageViewModel.DeletePersonCommand.Execute(person);
            }
        }
    }
}