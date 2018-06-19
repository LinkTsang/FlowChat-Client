using CommonServiceLocator;
using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.ViewModel
{
    public class AccountInfoViewModel : ViewModelBase
    {
        Account _account;
        IChatService ChatService => ServiceLocator.Current.GetInstance<IChatService>();
        IWindowService WindowService => ServiceLocator.Current.GetInstance<IWindowService>();

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

        async void UploadAvator()
        {
            var path = WindowService.OpenFile("FlowChat");
            if(path != null)
            {
                byte[] bytes = File.ReadAllBytes(path); 
                if(bytes == null)
                {
                    return;
                }
                var uploadResult = await ChatService.UploadAvator(Path.GetFileName(path), bytes);
                if(uploadResult.Ok)
                {
                    Account.Avatar = path;
                }
            }
        }
    }
}
