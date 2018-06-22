using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FlowChatApp.Utility
{
    public static class DispatcherFetcher
    {
        public static Dispatcher Dispatcher
        {
            get {
                if (Application.Current == null)
                {
                    var app = new Application();
                }
                return Application.Current.Dispatcher;
            }
        }
    }
}
