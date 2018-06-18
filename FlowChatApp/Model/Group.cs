﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.Model
{
    public class Group : ObservableObject
    {
        public static readonly string DefaultAvatar = "pack://application:,,,/FlowChatApp;component/Images/group.png";
        public Group()
        {

        }


        string _id = string.Empty;
        public string Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        string _avatar;
        public string Avatar
        {
            get => !string.IsNullOrEmpty(_avatar) ? _avatar : DefaultAvatar;
            set => Set(ref _avatar, value);
        }

        string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        User _owner;
        public User Owner
        {
            get => _owner;
            set => Set(ref _owner, value);
        }

        public ObservableCollection<User> Members { get; } = new ObservableCollection<User>();
    }
}
