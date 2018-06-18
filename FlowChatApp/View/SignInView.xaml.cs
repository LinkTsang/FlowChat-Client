using FlowChatApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
using FlowChatApp.View.Interface;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace FlowChatApp.View
{
    /// <summary>
    /// SignWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SignInView : UserControl, ISignInView
    {
        public SignInView()
        {
            InitializeComponent();
            SimpleIoc.Default.Unregister<ISignInView>();
            SimpleIoc.Default.Register<ISignInView>(() => this);
        }

        public string GetPassword()
        {
            return PasswordBox.Password;
        }

    }
}
