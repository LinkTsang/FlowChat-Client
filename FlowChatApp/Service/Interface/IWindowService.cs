using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.Service.Interface
{
    public interface IWindowService
    {
        void ShowMessage(string title, string message);

        string OpenFile(string caption, string filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*");

        void ShowDialog(object content, Action<object> closeingAction = null);

        void ShowAddContractWindow();
        void CloseAddContractWindow();
    }
}
