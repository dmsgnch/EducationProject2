using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EducationProject2.Components.Helpers;
using EducationProject2.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Xaml.Interactivity;

namespace EducationProject2.Components.Behaviors
{
    public class BeginEditModeButtonClickBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += OnButtonClick;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Click -= OnButtonClick;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var pressedButton = VisualHelper.GetButtonFromObject(sender);
            
            var currentRow = VisualHelper.FindParent<DataGridRow>(pressedButton);
            var personsDataGrid = VisualHelper.FindParent<DataGrid>(currentRow);
            
            personsDataGrid.SelectedItem = currentRow.DataContext;
            
            ((MainPageViewModel)personsDataGrid.DataContext).EnableDataGridEditModeCommand.Execute(personsDataGrid.SelectedItem);
        }
    }
}