using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EducationProject2.Components.Helpers;
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
            Button button = sender as Button;
            if (button is null) throw new ArgumentException($"Sender is not Button! Sender is {sender.GetType().Name}");
            
            var currentRow = VisualHelper.FindParent<DataGridRow>(button);
            var personsDataGrid = VisualHelper.FindParent<DataGrid>(currentRow);
            
            personsDataGrid.SelectedItem = currentRow.DataContext;
            personsDataGrid.BeginEdit();
        }
    }
}