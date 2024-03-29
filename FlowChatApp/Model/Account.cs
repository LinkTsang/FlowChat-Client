﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowChatApp.Frameworks;
using Newtonsoft.Json;

namespace FlowChatApp.Model
{
    public class Account : User
    {
        public Account()
        {

        }
        public Account(User user)
            : this(user.Id, user.Email, user.Username, user.Nickname, user.Region, user.Phone, user.Status, user.Gender, user.HeadUrl)
        {
        }
        public Account(long id, string email, string username, string nickname,
            string region, string phone, string status, Gender gender, string headUrl)
            : base(id, email, username, nickname, region, phone, status, gender, headUrl)
        {
        }

        string _password = string.Empty;

        [JsonIgnore]
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        internal void MergeFrom(Account account)
        {
            Password = account.Password;
            base.MergeFrom(account);
        }
    }
}
