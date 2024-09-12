using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Controls;
using EducationProject2.Components.Helpers;
using EducationProject2.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Xaml.Interactivity;

namespace EducationProject2.Components.Behaviors
{
    public class CancelEditModeButtonClickBehavior : Behavior<Button>
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
            
            var person = pressedButton.DataContext;
            var currentRow = VisualHelper.FindParent<DataGridRow>(pressedButton);
            var personsDataGrid = VisualHelper.FindParent<DataGrid>(currentRow);

            var cells = VisualHelper.GetCellsInCurrentRow(personsDataGrid, currentRow);
            cells.ForEach(RevertPreviousTextBoxesValues);
            
            ((MainPageViewModel)personsDataGrid.DataContext).DisableDataGridEditModeCommand.Execute(person);
        }
        
        private void RevertPreviousTextBoxesValues(DataGridCell cell)
        {
            if (cell.Content is DockPanel dockPanel)
            {
                if (dockPanel.FindChild<TextBox>() is TextBox textBox && dockPanel.FindChild<TextBlock>() is TextBlock textBlock)
                {
                    textBox.Text = textBlock.Text;
                }
            }
        }
    }
}