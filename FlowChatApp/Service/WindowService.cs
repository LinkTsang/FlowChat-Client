using FlowChatApp.Service.Interface;
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
        public string OpenFile(string caption, string filter = "All files (*.*)|*.*")
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title);
        }
    }
}
