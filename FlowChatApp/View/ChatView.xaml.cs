﻿using GalaSoft.MvvmLight.Messaging;
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
    /// ChatControl.xaml 的交互逻辑
    /// </summary>
    public partial class ChatView : UserControl
    {
        public ChatView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification != "ScrollIntoView") return;
            MessageListView.SelectedIndex = MessageListView.Items.Count - 1;
            MessageListView.ScrollIntoView(MessageListView.SelectedItem);
        }
    }
}
