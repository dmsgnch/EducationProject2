using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EducationProject2.Models
{
    [Serializable]
    public class Person : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            } 
        }

        public string LastName
        {
            get => _lastName;
            set => _lastName = value;
        }

        public Person()
        { }

        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}