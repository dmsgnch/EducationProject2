using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using EducationProject2.Commands;
using EducationProject2.Models;
using EducationProject2.Services;
using EducationProject2.Services.Abstract;

namespace EducationProject2.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Person> _persons;

        public ObservableCollection<Person> Persons
        {
            get => _persons;
            set
            {
                _persons = value;
                OnPropertyChanged();
                SubscribeToPersonsPropertiesChanged();
            }
        }

        #region Commands

        public RelayCommand AddPersonCommand { get; }
        public ICommand DeletePersonCommand { get; }

        #endregion

        #region Binding params

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

        #endregion

        public MainPageViewModel()
        {
            InitializePersons();

            AddPersonCommand = new RelayCommand((param) => AddPerson(), CanAddPerson);
            DeletePersonCommand = new RelayCommand((param) => DeletePerson(param as Person));
        }

        #region Persons list functionality

        private async void InitializePersons()
        {
            Persons = await Task.Run(LoadPersonsFromFileOrNullAsync) ?? new ObservableCollection<Person>();
            Persons.CollectionChanged += OnPersons_Changed;
        }

        private void SubscribeToPersonsPropertiesChanged()
        {
            foreach (var person in Persons)
            {
                person.PropertyChanged += OnPersons_Changed;
            }
        }

        private async void OnPersons_Changed(object sender, EventArgs e)
        {
            await SavePersonsToFileAsync();
        }

        #endregion

        #region Commands functionality

        private void AddPerson()
        {
            var newPerson = new Person(FirstName, LastName);
            newPerson.PropertyChanged += OnPersons_Changed;
            
            Persons.Add(newPerson);

            ClearPersonInputFields();
        }

        private void ClearPersonInputFields()
        {
            FirstName = String.Empty;
            LastName = String.Empty;
        }

        private void DeletePerson(Person person)
        {
            person.PropertyChanged -= OnPersons_Changed;
            
            Persons.Remove(person);
        }

        private bool CanAddPerson() => !String.IsNullOrWhiteSpace(FirstName) && !String.IsNullOrWhiteSpace(LastName);

        #endregion

        private async Task SavePersonsToFileAsync()
        {
            if (Persons is null) return;

            JsonFileSaverServiceBase jsonFileSaver = new JsonFileSaverService();

            await jsonFileSaver.SaveToFileAsync(Persons);
        }

        private async Task<ObservableCollection<Person>> LoadPersonsFromFileOrNullAsync()
        {
            JsonFileLoaderService jsonLoader = new JsonFileLoaderService();

            return await jsonLoader.GetFileDataOrNullAsync<ObservableCollection<Person>>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}