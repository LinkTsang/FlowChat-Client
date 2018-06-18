using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace FlowChatApp.ViewModel
{
    public class SearchViewModel : ViewModelBase
    {
        public SearchViewModel()
        {

        }

        string _searchText;

        public string SearchText
        {
            get => _searchText;
            set => Set(ref _searchText, value);
        }
    }
}
