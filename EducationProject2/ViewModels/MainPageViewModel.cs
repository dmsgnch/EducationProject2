using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using EducationProject2.Commands;
using EducationProject2.Components.Helpers;
using EducationProject2.Models;
using EducationProject2.Services;
using EducationProject2.Services.Abstract;
using Microsoft.Toolkit.Uwp.UI.Controls;

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
        public RelayCommand EditPersonCommand { get; }
        public RelayCommandAsync SavePersonCommand { get; }
        public RelayCommand CancelEditCommand { get; }

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

        public MainPageViewModel()
        {
            AddPersonCommand = new RelayCommand((param) => AddPerson(), CanAddPerson);
            DeletePersonCommand = new RelayCommandAsync(async (param) => await DeletePersonAsync((Person)param));
            SelectFileStorageCommand = new RelayCommandAsync(async (param) => await SelectJsonFileStorageAsync());
            SelectMongoDbStorageCommand = new RelayCommandAsync(async (param) => await SelectMongoDbStorageAsync());

            EditPersonCommand = new RelayCommand((param) => EnableEditRowMode((Button)param));
            SavePersonCommand = new RelayCommandAsync(async (param) => await SaveEditRowModeAsync((Button)param));
            CancelEditCommand = new RelayCommand((param) => CancelEditRowMode((Button)param));

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
            Persons.Add(new Person(FirstName, LastName));

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

        #region Edit mode functionality

        private void EnableEditRowMode(Button button)
        {
            var currentRow = VisualHelper.FindParent<DataGridRow>(button);
            var personsDataGrid = VisualHelper.FindParent<DataGrid>(currentRow);
            
            personsDataGrid.SelectedItem = currentRow.DataContext;
            personsDataGrid.BeginEdit();
        }

        private async Task SaveEditRowModeAsync(Button button)
        {
            var currentRow = VisualHelper.FindParent<DataGridRow>(button);
            var personsDataGrid = VisualHelper.FindParent<DataGrid>(currentRow);

            var cells = VisualHelper.GetCellsInCurrentRow(personsDataGrid, currentRow);
            for (int i = 0; i < cells.Count; i++)
            {
                personsDataGrid.CurrentColumn = personsDataGrid.Columns[i];

                UpdateTextBoxBindingSourceInCell(cells[i]);
            }

            personsDataGrid.CommitEdit();

            await SavePersonsToFileAsync();
        }

        private void UpdateTextBoxBindingSourceInCell(DataGridCell cell)
        {
            if (cell.Content is TextBox textBox)
            {
                var bindingTextBox = textBox.GetBindingExpression(TextBox.TextProperty);
                if (bindingTextBox is null) throw new Exception("Binding TextBlock was not found!");

                bindingTextBox.UpdateSource();
            }
        }

        private void CancelEditRowMode(Button button)
        {
            var personsDataGrid = VisualHelper.FindParent<DataGrid>(button);
            
            personsDataGrid.CancelEdit(DataGridEditingUnit.Row);
            personsDataGrid.SelectedItem = null;
        }
        
        #endregion

        #endregion

        #region Persons list functionality

        private async Task InitializePersons()
        {
            Persons = await LoadPersonsFromFileAsync();
            Persons.CollectionChanged += OnPersons_Changed;
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
                
                if (Debugger.IsAttached)
                {
                    MessageHelper.ShowToastNotification("Data successfully saved");
                }
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