using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EducationProject2.Commands;
using EducationProject2.Components.Helpers;
using EducationProject2.Models;
using EducationProject2.Services;
using EducationProject2.Services.Abstract;

namespace EducationProject2.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Person> persons = new ObservableCollection<Person>();

        public ObservableCollection<Person> Persons
        {
            get => persons;
            set
            {
                if (!value.Equals(persons))
                {
                    persons = value;
                    OnPropertyChanged();
                    SubscribeToPersonsPropertiesChanged();
                }
            }
        }

        #region Services

        private StorageServiceBase<Person> StorageService { get; set; }

        #endregion

        #region Commands

        public RelayCommand AddPersonCommand { get; }
        public RelayCommand DeletePersonCommand { get; }
        public RelayCommandAsync SaveInFileCommand { get; }
        public RelayCommandAsync SaveInDatabaseCommand { get; }

        #endregion

        #region Binding params

        private string firstName;

        public string FirstName
        {
            get => firstName;
            set
            {
                if (!value.Equals(firstName))
                {
                    firstName = value;
                    OnPropertyChanged();
                    AddPersonCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string lastName;

        public string LastName
        {
            get => lastName;
            set
            {
                if (!value.Equals(lastName))
                {
                    lastName = value;
                    OnPropertyChanged();
                    AddPersonCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool isFileSaveChecked = false;

        public bool IsFileSaveChecked
        {
            get => isFileSaveChecked;
            set
            {
                if (!value.Equals(isFileSaveChecked))
                {
                    isFileSaveChecked = value;
                    OnPropertyChanged();

                    if (value) SaveInFileCommand.Execute(null);
                }
            }
        }

        private bool isDbSaveChecked = false;

        public bool IsDbSaveChecked
        {
            get => isDbSaveChecked;
            set
            {
                if (!value.Equals(isDbSaveChecked))
                {
                    isDbSaveChecked = value;
                    OnPropertyChanged();

                    if (value) SaveInDatabaseCommand.Execute(null);
                }
            }
        }

        #endregion

        public MainPageViewModel()
        {
            AddPersonCommand = new RelayCommand((param) => AddPerson(), CanAddPerson);
            DeletePersonCommand = new RelayCommand((param) => DeletePerson((Person)param));
            SaveInFileCommand = new RelayCommandAsync(async (param) => await SetJsonSavingAsync());
            SaveInDatabaseCommand = new RelayCommandAsync(async (param) => await SetDbSavingAsync());

            SelectDefaultSaveType();
        }

        private void SelectDefaultSaveType()
        {
            IsFileSaveChecked = true;
        }

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

        private async Task SetJsonSavingAsync()
        {
            StorageService = new JsonStorageService<Person>();

            await InitializePersons();
        }

        private async Task SetDbSavingAsync()
        {
            StorageService = new MongoDbStorageService<Person>("Persons");

            await InitializePersons();
        }

        #endregion

        #region Persons list functionality

        private async Task InitializePersons()
        {
            Persons = await LoadPersonsFromFileAsync();
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

        #region Persons saving/loading

        private async Task SavePersonsToFileAsync()
        {
            try
            {
                await StorageService.SaveAsync(Persons.ToList());
            }
            catch (Exception ex)
            {
                await MessageHelper.GetInfoDialog($"{ex.Message}", "Error").ShowAsync();
            }
        }

        private async Task<ObservableCollection<Person>> LoadPersonsFromFileAsync()
        {
            List<Person> loadedPersons = new List<Person>();

            try
            {
                loadedPersons = await StorageService.LoadAsync();
            }
            catch (Exception ex)
            {
                await MessageHelper.GetInfoDialog($"{ex.Message}", "Error").ShowAsync();
            }

            return new ObservableCollection<Person>(loadedPersons);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}