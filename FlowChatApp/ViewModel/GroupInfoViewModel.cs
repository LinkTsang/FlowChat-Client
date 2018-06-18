using FlowChatApp.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChatApp.ViewModel
{
    public class GroupInfoViewModel : ViewModelBase
    {
        Group _group;

        public Group Group {
            get => _group;
            set => Set(ref _group, value);
        }
        
        public GroupInfoViewModel()
        {

        }
    }
}
