using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

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
            await ChatService.JoinGroup(group.Id);
        }

        public RelayCommand SearchCommand { get; }
        async void SearchGroup()
        {
            var results = (await ChatService.SearchGroup(SearchText)).Data;
            Groups.Clear();
            results.ForEach(g => Groups.Add(g));
        }

        public RelayCommand NewGroupCommand { get; }
        async void NewGroup()
        {

        }

    }
}
