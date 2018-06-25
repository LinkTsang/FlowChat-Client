using CommonServiceLocator;
using FlowChatApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlowChatApp.View
{
    /// <summary>
    /// ErrorMessageView.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBoxDialogView : UserControl
    {

        public MessageBoxDialogView()
        {
            InitializeComponent();
        }

        public MessageBoxDialogView(string title, string message)
        {
            MessageBoxViewModel messageBoxViewModel = ServiceLocator.Current.GetInstance<MessageBoxViewModel>();
            messageBoxViewModel.Title = title;
            messageBoxViewModel.Message = message;
            InitializeComponent();
        }

        public MessageBoxDialogView(string message) : this("", message)
        {
        }
    }
}
