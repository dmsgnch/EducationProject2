using Windows.UI.Xaml.Controls;
using EducationProject2.ViewModels;

namespace EducationProject2.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            DataContext = new MainPageViewModel();
        }
    }
}