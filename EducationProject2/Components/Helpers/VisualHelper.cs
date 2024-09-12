using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace EducationProject2.Components.Helpers
{
    public static class VisualHelper
    {
        public static List<DataGridCell> GetCellsInCurrentRow(DataGrid myDataGrid, DataGridRow currentRow)
        {
            return myDataGrid.Columns.Select(c => FindParent<DataGridCell>(c.GetCellContent(currentRow))).ToList();
        }
        
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                if (child is T parent)
                {
                    return parent;
                }
                
                child = VisualTreeHelper.GetParent(child);

                if (child is null) throw new InvalidOperationException($"Cant find {typeof(T).Name} parent");
            }
        }

        public static Button GetButtonFromObject(object buttonObj)
        {
            return buttonObj as Button ?? throw new ArgumentException($"Sender is not Button! Sender is {buttonObj.GetType().Name}");
        }
    }
}