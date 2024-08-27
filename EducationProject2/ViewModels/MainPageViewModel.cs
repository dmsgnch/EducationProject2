using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EducationProject2.Commands;
using EducationProject2.Models;

namespace EducationProject2.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Person> Persons { get; set; } = new ObservableCollection<Person>();
        
        public RelayCommand AddPersonCommand { get; }

        private string _firstName;

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
                AddPersonCommand.RaiseCanExecuteChanged();
            }
        }
        
        private string _lastName;

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
                AddPersonCommand.RaiseCanExecuteChanged();
            }
        }
        

        public MainPageViewModel()
        {
            AddPersonCommand = new RelayCommand((param) => AddPerson(), CanAddPerson);
        }

        private void AddPerson()
        {
            Persons.Add(new Person(FirstName, LastName));

            ClearPersonInputFields();
        }

        private void ClearPersonInputFields()
        {
            FirstName = String.Empty;
            LastName = String.Empty;
        }

        private bool CanAddPerson() => !String.IsNullOrWhiteSpace(FirstName) && !String.IsNullOrWhiteSpace(LastName);
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}