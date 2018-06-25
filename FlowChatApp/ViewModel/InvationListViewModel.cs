using CommonServiceLocator;
using FlowChatApp.Model;
using FlowChatApp.Service.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.ViewModel
{
    public class InvationListViewModel : ViewModelBase
    {
        IChatService ChatService => ServiceLocator.Current.GetInstance<IChatService>();

        ObservableCollection<ContractInvation> _invations = new ObservableCollection<ContractInvation>();
        public ObservableCollection<ContractInvation> Invations
        {
            get => _invations;
            set => Set(ref _invations, value);
        }

        public InvationListViewModel()
        {
            if (IsInDesignMode)
            {
                Invations.Add(new ContractInvation(1, "test0", "Hello"));
            }

            AcceptCommand = new RelayCommand<ContractInvation>(Accept);
            RejectCommand = new RelayCommand<ContractInvation>(Reject);
        }

        public RelayCommand<ContractInvation> AcceptCommand { get; }
        async void Accept(ContractInvation invation)
        {
            await ChatService.ConfirmContractInvation(invation.RecordId, "", true);
            invation.Tag = 2;
        }

        public RelayCommand<ContractInvation> RejectCommand { get; }
        async void Reject(ContractInvation invation)
        {
            await ChatService.ConfirmContractInvation(invation.RecordId, "", false);
            invation.Tag = 3;
        }
    }
}
