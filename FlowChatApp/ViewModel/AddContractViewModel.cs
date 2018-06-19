using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommonServiceLocator;
using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FlowChatApp.ViewModel
{
    public class AddContractViewModel : ViewModelBase
    {
        IChatService ChatService => ServiceLocator.Current.GetInstance<IChatService>();
        public AddContractViewModel()
        {
            AddCommand = new RelayCommand<User>(AddUser);
            SearchCommand = new RelayCommand(SearchUser);
            if (IsInDesignMode)
            {
                SetUpDesignData();
            }

        }

        async void SetUpDesignData()
        {
            (await ServiceLocator.Current.GetInstance<IChatService>().GetContacts()).Data.ForEach(c => Users.Add(c.User));
        }
        string _searchText;

        public string SearchText
        {
            get => _searchText;
            set => Set(ref _searchText, value);
        }

        ObservableCollection<User> _users = new ObservableCollection<User>();
        public ObservableCollection<User> Users
        {
            get => _users;
            set => Set(ref _users, value);
        }

        User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set => Set(ref _currentUser, value);
        }

        public RelayCommand<User> AddCommand { get; }
        async void AddUser(User User)
        {
            await ChatService.AddContact(User.UserName, "", "Hello");
        }

        public RelayCommand SearchCommand { get; }
        async void SearchUser()
        {
            var results = (await ChatService.SearchUser(SearchType.ByUserName, SearchText)).Data;
            Users.Clear();
            results.ForEach(g => Users.Add(g));
        }
    }
}
