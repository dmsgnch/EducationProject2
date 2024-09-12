using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EducationProject2.Models.Abstract;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace EducationProject2.Models
{
    [Serializable]
    public class Person : INotifyPropertyChanged, IMongoDbObject, IDataGridCustomMultiEditingObject
    {
        [JsonIgnore] public ObjectId Id { get; set; } = ObjectId.Empty;

        private string firstName;
        private string lastName;

        public string FirstName
        {
            get => firstName;
            set
            {
                if (!value.Equals(firstName))
                {
                    firstName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastName
        {
            get => lastName;
            set
            {
                if (!value.Equals(lastName))
                {
                    lastName = value;
                    OnPropertyChanged();
                }
            }
        }

        private EditVisibility editVisibility;

        [JsonIgnore]
        [BsonIgnore]
        public EditVisibility EditVisibility
        {
            get => editVisibility;
            set
            {
                if (value != editVisibility)
                {
                    editVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public Person()
        {
        }

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