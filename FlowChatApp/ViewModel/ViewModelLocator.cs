/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:FlowChatApp"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using FlowChatApp.Service.Interface;
using ChatService = FlowChatApp.Service.ChatService;
using FlowChatApp.Service;
using FlowChatApp.Model;
using System;
using GalaSoft.MvvmLight;

namespace FlowChatApp.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<SignInViewModel>();
            SimpleIoc.Default.Register<SignUpViewModel>();

            SimpleIoc.Default.Register<AccountInfoViewModel>();
            SimpleIoc.Default.Register<UserInfoViewModel>();
            SimpleIoc.Default.Register<GroupInfoViewModel>();

            SimpleIoc.Default.Register<ChatViewModel>();
            SimpleIoc.Default.Register<AddContractViewModel>();
            SimpleIoc.Default.Register<AddGroupViewModel>();

            SimpleIoc.Default.Register<MessageBoxViewModel>();
            SimpleIoc.Default.Register<InvationListViewModel>();
        }

        static ViewModelLocator()
        {
            if (ViewModelBase.IsInDesignModeStatic || App.IsInDemoMode)
            {
                SimpleIoc.Default.Register<IChatService>(() => new DemoChatService());
            }
            else
            {
                SimpleIoc.Default.Register<IChatService>(() => new ChatService("http://127.0.0.1:8081/"));
            }
            SimpleIoc.Default.Register<IWindowService, WindowService>();
        }
        public LoginViewModel Login
        {
            get => ServiceLocator.Current.GetInstance<LoginViewModel>();
        }

        public SignInViewModel SignIn
        {
            get => ServiceLocator.Current.GetInstance<SignInViewModel>();
        }

        public SignUpViewModel SignUp
        {
            get => ServiceLocator.Current.GetInstance<SignUpViewModel>();
        }

        public AccountInfoViewModel AccountInfo
        {
            get => ServiceLocator.Current.GetInstance<AccountInfoViewModel>();
        }

        public UserInfoViewModel UserInfo
        {
            get => ServiceLocator.Current.GetInstance<UserInfoViewModel>();
        }

        public GroupInfoViewModel GroupInfo
        {
            get => ServiceLocator.Current.GetInstance<GroupInfoViewModel>();
        }

        public ChatViewModel Chat
        {
            get => ServiceLocator.Current.GetInstance<ChatViewModel>();
        }
        public AddGroupViewModel AddGroup
        {
            get => ServiceLocator.Current.GetInstance<AddGroupViewModel>();
        }
        public AddContractViewModel AddContract
        {
            get => ServiceLocator.Current.GetInstance<AddContractViewModel>();
        }
        public MessageBoxViewModel MessageBox
        {
            get => ServiceLocator.Current.GetInstance<MessageBoxViewModel>();
        }

        public InvationListViewModel InvationList
        {
            get => ServiceLocator.Current.GetInstance<InvationListViewModel>();
        }
        public IChatService ChatService
        {
            get => ServiceLocator.Current.GetInstance<IChatService>();
        }

        public IWindowService WindowService
        {
            get => ServiceLocator.Current.GetInstance<IWindowService>();
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}