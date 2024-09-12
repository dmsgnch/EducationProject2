using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
                }
            }
        }

        #region Services

        private StorageServiceBase<Person> StorageService { get; set; }

        #endregion

        #region Commands

        public RelayCommand AddPersonCommand { get; }
        public RelayCommandAsync DeletePersonCommand { get; }
        public RelayCommandAsync SelectFileStorageCommand { get; }
        public RelayCommandAsync SelectMongoDbStorageCommand { get; }
        public RelayCommand EnableDataGridEditModeCommand { get; }
        public RelayCommand DisableDataGridEditModeCommand { get; }

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

                    if (value) SelectFileStorageCommand.Execute(null);
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

                    if (value) SelectMongoDbStorageCommand.Execute(null);
                }
            }
        }

        #endregion
        
        #region Edit visibility variations

        private readonly EditVisibility nonEditModeVisibility = new EditVisibility(Visibility.Visible, Visibility.Collapsed);
        private readonly EditVisibility editModeVisibility = new EditVisibility(Visibility.Collapsed, Visibility.Visible);

        #endregion
        
        public MainPageViewModel()
        {
            AddPersonCommand = new RelayCommand((param) => AddPerson(), CanAddPerson);
            DeletePersonCommand = new RelayCommandAsync(async (param) => await DeletePersonAsync((Person)param));
            
            SelectFileStorageCommand = new RelayCommandAsync(async (param) => await SelectJsonFileStorageAsync());
            SelectMongoDbStorageCommand = new RelayCommandAsync(async (param) => await SelectMongoDbStorageAsync());
            
            EnableDataGridEditModeCommand = new RelayCommand( (param) => EnableEditDataGridMode((Person)param));
            DisableDataGridEditModeCommand = new RelayCommand( (param) => DisableEditDataGridMode((Person)param));

            SelectDefaultSaveType();
        }

        private void SelectDefaultSaveType()
        {
            IsFileSaveChecked = true;
        }

        #region Commands functionality

        #region Storage type selection

        private async Task SelectJsonFileStorageAsync()
        {
            StorageService = new JsonStorageService<Person>();

            await InitializePersons();
        }

        private async Task SelectMongoDbStorageAsync()
        {
            StorageService = new MongoDbStorageService<Person>("Persons");

            await InitializePersons();
        }

        #endregion

        #region Add person

        private void AddPerson()
        {
            var newPerson = new Person(FirstName, LastName);
            DisableEditDataGridMode(newPerson);
            
            newPerson.PropertyChanged += OnPersons_Changed;
            Persons.Add(newPerson);

            ClearPersonInputFields();
        }

        private void ClearPersonInputFields()
        {
            FirstName = String.Empty;
            LastName = String.Empty;
        }

        private bool CanAddPerson() => !String.IsNullOrWhiteSpace(FirstName) && !String.IsNullOrWhiteSpace(LastName);

        #endregion
        
        #region Delete person
        
        private async Task DeletePersonAsync(Person person)
        {
            if (await IsDeletingConfirmedAsync())
            {
                person.PropertyChanged -= OnPersons_Changed;
                Persons.Remove(person);
            }
        }

        private async Task<bool> IsDeletingConfirmedAsync()
        {
            ContentDialog deleteFileDialog = new ContentDialog()
            {
                Title = "Confirm action",
                Content = "Are you really want to delete the person?",
                PrimaryButtonText = "Ok",
                SecondaryButtonText = "Cancel"
            };

            return await deleteFileDialog.ShowAsync() is ContentDialogResult.Primary;
        }
        
        #endregion
        
        #region Toggle edit mode

        private void EnableEditDataGridMode(Person person)
        {
            DisableEditModeAllInCollection(Persons);
            
            person.EditVisibility = editModeVisibility;
        }
        
        private void DisableEditDataGridMode(Person person)
        {
            person.EditVisibility = nonEditModeVisibility;
        }
        
        #endregion
        
        #endregion

        #region Persons list functionality

        private async Task InitializePersons()
        {
            var loadedPersons = await LoadPersonsFromFileAsync();
            
            DisableEditModeAllInCollection(loadedPersons);
            AddOnPropertyChangeHandlerToPersons(loadedPersons);

            Persons = loadedPersons;
            Persons.CollectionChanged += OnPersons_Changed;
        }

        private void DisableEditModeAllInCollection(ICollection<Person> personsCollection)
        {
            foreach (var person in personsCollection)
            {
                DisableEditDataGridMode(person);
            }
        }

        private void AddOnPropertyChangeHandlerToPersons(ICollection<Person> personsCollection)
        {
            foreach (var person in personsCollection)
            {
                person.PropertyChanged += OnPersons_Changed;
            }
        }

        private async void OnPersons_Changed(object sender, EventArgs e)
        {
            if (!(e is PropertyChangedEventArgs) || 
                (e is PropertyChangedEventArgs prArg && prArg.PropertyName != nameof(EditVisibility)))
            {
                await SavePersonsToFileAsync();
            }
        }

        #endregion

        #region Persons saving/loading
        
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

        private async Task SavePersonsToFileAsync()
        {
            await semaphore.WaitAsync();

            try
            {
                await StorageService.SaveAsync(Persons.ToList());
                
                if (Debugger.IsAttached)
                {
                    MessageHelper.ShowToastNotification("Data successfully saved");
                }
            }
            catch (Exception ex)
            {
                await MessageHelper.GetInfoDialog($"{ex.Message}", "Error").ShowAsync();
            }
            finally
            {
                semaphore.Release();
            }
        }
        
        private async Task<ObservableCollection<Person>> LoadPersonsFromFileAsync()
        {
            await semaphore.WaitAsync();
            
            List<Person> loadedPersons = new List<Person>();

            try
            {
                loadedPersons = await StorageService.LoadAsync();
            }
            catch (Exception ex)
            {
                await MessageHelper.GetInfoDialog($"{ex.Message}", "Error").ShowAsync();
            }
            finally
            {
                semaphore.Release();
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