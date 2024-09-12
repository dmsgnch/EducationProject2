using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace EducationProject2.Models
{
    [Serializable]
    public class EditVisibility
    {
        public Visibility NonEditComponentVisibility { get; set; }
        public Visibility EditComponentVisibility { get; set; }

        public EditVisibility()
        { }

        public EditVisibility(Visibility nonEditVisibility, Visibility editVisibility)
        {
            NonEditComponentVisibility = nonEditVisibility;
            EditComponentVisibility = editVisibility;
        }
            
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}