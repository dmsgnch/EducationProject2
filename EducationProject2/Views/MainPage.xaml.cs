using System;
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
        
        private async void OnDeletePersonButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Person person)
            {
                ContentDialog deleteFileDialog = new ContentDialog()
                {
                    Title = "Confirm action",
                    Content = "Are you really want to delete the person?",
                    PrimaryButtonText = "Ok",
                    SecondaryButtonText = "Cancel"
                };

                ContentDialogResult result = await deleteFileDialog.ShowAsync();
            
                if (result is ContentDialogResult.Primary)
                {
                    MainPageViewModel.DeletePersonCommand.Execute(person);
                }
            }
        }
    }
}