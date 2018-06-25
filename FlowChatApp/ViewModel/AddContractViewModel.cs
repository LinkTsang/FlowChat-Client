using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommonServiceLocator;
using FlowChatApp.Model;
using FlowChatApp.Service;
using FlowChatApp.Service.Interface;
using FlowChatApp.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;

namespace FlowChatApp.ViewModel
{
    public class AddContractViewModel : ViewModelBase
    {
        IChatService ChatService => ServiceLocator.Current.GetInstance<IChatService>();

        public AddContractViewModel()
        {
            SearchCommand = new RelayCommand(SearchUser);
            AddCommand = new RelayCommand<User>(AddUser);
            DialogAcceptCommand = new RelayCommand(DialogAccept);
            DialogCancelCommand = new RelayCommand(DialogCancel);
            if (IsInDesignMode)
            {
                SetUpDesignData();
            }
        }

        object _dialogContent;
        public object DialogContent
        {
            get => _dialogContent;
            set => Set(ref _dialogContent, value);
        }
        bool _isDialogOpen;
        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set => Set(ref _isDialogOpen, value);
        }
        public RelayCommand DialogAcceptCommand { get; }
        async void DialogAccept()
        {
            IsDialogOpen = false;
            var result = await ChatService.AddContact(UserToAdd.Username, "", Message);
            DialogContent = new MessageBoxDialogView(result.Ok ? "Invation sent successfully" : result.Message);
            IsDialogOpen = true;
        }
        public RelayCommand DialogCancelCommand { get; }
        async void DialogCancel()
        {
            IsDialogOpen = false;
        }

        async void SetUpDesignData()
        {
            (await ServiceLocator.Current.GetInstance<IChatService>().GetContracts()).Data.ForEach(c => Users.Add(c.User));
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
        IWindowService WindowService => ServiceLocator.Current.GetInstance<IWindowService>();



        public RelayCommand SearchCommand { get; }
        async void SearchUser()
        {
            var results = (await ChatService.SearchUser(SearchType.ByUserName, SearchText)).Data;
            Users.Clear();
            results?.ForEach(g => Users.Add(g));
        }


        User _userToAdd;
        public User UserToAdd
        {
            get => _userToAdd;
            set => Set(ref _userToAdd, value);
        }
        public string _message;
        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }
        public RelayCommand<User> AddCommand { get; }
        async void AddUser(User user)
        {
            UserToAdd = user;
            Message = string.Empty;
            DialogContent = new AddContractMessageDialog();
            IsDialogOpen = true;
        }
    }
}
