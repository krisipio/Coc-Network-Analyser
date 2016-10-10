using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ClashRoyale_NetworkAnalyser.Messages;
using ClashRoyale_NetworkAnalyser.Utils;

namespace ClashRoyale_NetworkAnalyser
{
    public class NetworkAnalyserViewModel
    {
        private MTObservableCollection<Message> _messageQueue = new MTObservableCollection<Message>();
        private MTObservableCollection<Message> _filteredMessageQueueView = new MTObservableCollection<Message>();
        private Message _selectedMessage;
        private string _messageIdFilter = String.Empty;

        public Message SelectedMessage
        {
            get { return _selectedMessage; }
            set { _selectedMessage = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public MTObservableCollection<Message> MessageQueue
        {
            get { return _messageQueue; }
            set { _messageQueue = value; }
        }

        public string MessageIdFilter
        {
            get { return _messageIdFilter; }
            set
            {
                _messageIdFilter = value;
                _filteredMessageQueueView.Clear();
                UpdateView();
                _filteredMessageQueueView.Update();
            }
        }

        public void AddMessageToQueueLog(Message msg)
        {
            _messageQueue.Add(msg);
            try
            {
                int messageIdToFilter = int.Parse(MessageIdFilter);
                foreach (var message in MessageQueue)
                {
                    if (message.ID == messageIdToFilter && !_filteredMessageQueueView.Contains(message))
                        FilteredMessageQueueView.Add(message);
                }
            }
            catch (Exception)
            {
                FilteredMessageQueueView.Add(msg);
            }
        }

        public void UpdateView()
        {
            if (MessageIdFilter == String.Empty)
            {
                foreach (var message in MessageQueue)
                {
                    if (!_filteredMessageQueueView.Contains(message))
                        _filteredMessageQueueView.Add(message);
                }
            }
            else
            {
                try
                {
                    int messageIdToFilter = int.Parse(MessageIdFilter);
                    foreach (var message in MessageQueue)
                    {
                        if (message.ID == messageIdToFilter && !_filteredMessageQueueView.Contains(message))
                            _filteredMessageQueueView.Add(message);
                    }
                }
                catch (Exception)
                {
                    foreach (var message in MessageQueue)
                    {
                        if (!_filteredMessageQueueView.Contains(message))
                            _filteredMessageQueueView.Add(message);
                    }
                }
            }
        }

        public MTObservableCollection<Message> FilteredMessageQueueView
        {
            get
            {
                return _filteredMessageQueueView;
            }
            set { _filteredMessageQueueView = value; }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
