using FlowChatApp.Model;
using GalaSoft.MvvmLight;
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
        }
    }
}
