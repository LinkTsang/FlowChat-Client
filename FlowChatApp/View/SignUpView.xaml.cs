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
using FlowChatApp.Service2;
using FlowChatApp.Service2.Interfaces;
using FlowChatApp.View.Interface;
using GalaSoft.MvvmLight.Ioc;

namespace FlowChatApp.View
{
    /// <summary>
    /// SignInView.xaml 的交互逻辑
    /// </summary>
    public partial class SignUpView : UserControl, ISignUpView
    {
        public SignUpView()
        {
            InitializeComponent();
            SimpleIoc.Default.Unregister<ISignUpView>();
            SimpleIoc.Default.Register<ISignUpView>(() => this);
        }

        public string GetPassword()
        {
            return PasswordBox.Password;
        }
    }
}
