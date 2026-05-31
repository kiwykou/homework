using Dapper;
using DbDemo.Commands;
using DbDemo.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Tmds.DBus.Protocol;

namespace DbDemo.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string ConnectionString = "Data Source=app.db";
        public event PropertyChangedEventHandler?  PropertyChanged;
        public ObservableCollection<Users> Users { get; } = new();

        public ICommand LoadUsersCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand SearchUsersCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand ClearInputCommand { get; }



        public MainViewModel()
        {
            LoadUsersCommand = new RelayCommand(_ => LoadUsers());
            AddUserCommand = new RelayCommand(_ => AddUser());
            DeleteUserCommand = new RelayCommand(_ => DeleteSelectedUser());
            SearchUsersCommand = new RelayCommand(_ => SearchUsers());
            ClearSearchCommand = new RelayCommand(_ => ClearSearch());
            ClearInputCommand = new RelayCommand(_ => ClearInput());
            DeleteUserCommand = new RelayCommand(_ => DeleteSelectedUser());

            LoadUsers();
        }

        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        private string _email = "";
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }
        private string _search = "";
        public string searchText
        {
            get => _search;
            set
            {
                _search = value;
                OnPropertyChanged();
            }
        }
        private string _info = "";
        public string Info
        {
            get => _info;
            set
            {
                _info = value;
                OnPropertyChanged();
            }
        }

        private Users? _selectedUser;
        public Users? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }
        private Users? _allUsers;
        public Users? AllUsers
        {
            get => _allUsers;
            set
            {
                _allUsers = value;
                OnPropertyChanged();
            }
        }
        private string _id = "";
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        public void LoadUsers()
        {
            Users.Clear();

            using var connection = new SqliteConnection(ConnectionString);
            {

                connection.Open();
                var usersFromDb = connection.Query<Users>("SELECT * FROM Users");
                foreach (var user in usersFromDb)
                {
                    Users.Add(user);
                }
            }
            
            Info = $"Загружено пользователей: {Users.Count}";

        }
        public void AddUser()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email))
            {
                Info = "Введите имя и email";
                return;
            }

            using var connection = new SqliteConnection(ConnectionString);
            {
                connection.Open();
                string sql = "INSERT INTO Users (Name, Email) VALUES (@Name, @Email)";
                connection.Execute(sql, new { Name, Email });
            }
            Info = "Пользователь добавлен";
            ClearInput();
            LoadUsers();
        }

        public void DeleteSelectedUser()
        {
            if (SelectedUser == null)
            {
                Info = "Выберите пользователя для удаления";
                return;
            }

            using var connection = new SqliteConnection(ConnectionString);
            {
                connection.Open();
                string sql = "DELETE FROM Users WHERE Id = @Id";
                connection.Execute(sql, new { Id = SelectedUser.Id });
            }

            Info = "Пользователь удалён";
            LoadUsers();
        }
        public void SearchUsers()
        {
            Users.Clear();

            using var connection = new SqliteConnection(ConnectionString);
            {
                connection.Open();
                string sql = "SELECT * FROM Users WHERE Name LIKE @Search OR Email LIKE @Search";
                string searchPattern = $"%{SearchText.ToLower()}%";

                var result = connection.Query<Users>(sql, new { Search = searchPattern });
                foreach (var user in result)
                {
                    Users.Add(user);
                }
            }

            Info = $"Найдено совпадений: {Users.Count}";

        }

        private void ClearSearch()
        {
            SearchText = "";
            LoadUsers();
        }

        public void DeleteUser(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            {

                connection.Open();
                string sql = "DELETE FROM Users WHERE Id = @Id";
                connection.Execute(sql, new { Id = Id });

            }
            LoadUsers();
        }
        public void ClearInput()
        {
            Name = "";
            Email = "";
            Info = "Очищено";
            LoadUsers();
        }
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
