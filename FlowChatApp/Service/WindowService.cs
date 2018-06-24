using FlowChatApp.Service.Interface;
using FlowChatApp.View;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FlowChatApp.Service
{
    public class WindowService : IWindowService
    {
        public string OpenFile(string caption, string filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*")
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = filter,
                Title = caption
            };
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title);
        }

        public void ShowDialog(object content, Action<object> closeingAction)
        {
            DialogHost.Show(content, (sender, args) =>
            {
                closeingAction?.Invoke(args.Parameter);
            });
        }



        AddContractMessageView _addContractMessageView;
        public void ShowAddContractWindow()
        {
            if (_addContractMessageView == null)
            {
                _addContractMessageView = new AddContractMessageView();
            }
            _addContractMessageView.Show();
        }

        public void CloseAddContractWindow()
        {
            if (_addContractMessageView != null)
            {
                _addContractMessageView.Hide();
            }
        }
    }
}
