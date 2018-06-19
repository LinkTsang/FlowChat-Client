using CommonServiceLocator;
using FlowChatApp.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.ViewModel
{
    public class AccountInfoViewModel : ViewModelBase
    {
        Account _account;

        public Account Account
        {
            get => _account;
            set => Set(ref _account, value);
        }

        public AccountInfoViewModel()
        {
            if (IsInDesignMode)
            {
                Account = new Account(123, "flow@flowchat.com", "flow", "flow",
                    "China GuangZhou", "1234567890", "Hello Flow", Gender.Unknown, string.Empty);
            }

            UploadAvatorCommand = new RelayCommand(UploadAvator);
        }

        public RelayCommand UploadAvatorCommand { get; }

        void UploadAvator()
        {

        }
    }
}
