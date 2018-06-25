using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using FlowChatApp.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;

namespace FlowChatApp.ViewModel
{
    public class AddGroupViewModel : ViewModelBase
    {
        IChatService ChatService => ServiceLocator.Current.GetInstance<IChatService>();
        public AddGroupViewModel()
        {
            AddCommand = new RelayCommand<Group>(JoinGroup);
            SearchCommand = new RelayCommand(SearchGroup);
            NewGroupCommand = new RelayCommand(NewGroup);
            DialogAcceptCommand = new RelayCommand(DialogAccept);
            DialogCancelCommand = new RelayCommand(DialogCancel);
            if (IsInDesignMode)
            {
                SetUpDesignData();
            }

        }

        async void SetUpDesignData()
        {
            (await ChatService.GetGroups()).Data.ForEach(g => Groups.Add(g));
        }
        string _searchText;

        public string SearchText
        {
            get => _searchText;
            set => Set(ref _searchText, value);
        }

        ObservableCollection<Group> _groups = new ObservableCollection<Group>();
        public ObservableCollection<Group> Groups
        {
            get => _groups;
            set => Set(ref _groups, value);
        }

        Group _currentGroup;
        public Group CurrentGroup
        {
            get => _currentGroup;
            set => Set(ref _currentGroup, value);
        }

        public RelayCommand<Group> AddCommand { get; }
        async void JoinGroup(Group group)
        {
            var result = await ChatService.JoinGroup(group.Id);
            DialogContent = new MessageBoxDialogView(result.Message);
            IsDialogOpen = true;
        }

        public RelayCommand SearchCommand { get; }
        async void SearchGroup()
        {
            var results = (await ChatService.SearchGroups(SearchText)).Data;
            Groups.Clear();
            results?.ForEach(g => Groups.Add(g));
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
        string _groupNameToCreate;
        public string GroupNameToCreate
        {
            get => _groupNameToCreate;
            set => Set(ref _groupNameToCreate, value);
        }
        public RelayCommand NewGroupCommand { get; }
        async void NewGroup()
        {
            GroupNameToCreate = string.Empty;
            DialogContent = new AddNewGroupDialog();
            IsDialogOpen = true;
        }
        public RelayCommand DialogAcceptCommand { get; }
        async void DialogAccept()
        {
            IsDialogOpen = false;
            var result = await ChatService.CreateGroup(GroupNameToCreate);
            DialogContent = new MessageBoxDialogView(result.Message);
            IsDialogOpen = true;
        }
        public RelayCommand DialogCancelCommand { get; }
        async void DialogCancel()
        {
            IsDialogOpen = false;
        }
    }
}
