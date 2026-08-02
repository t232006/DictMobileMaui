using DictMobile.models;
using IndDictionary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;


namespace DictMobile.ViewModels
{
    public class ListTopicsViewModel :INotifyPropertyChanged
    {   
        public void GetTopicList()
        {
            TopicsList = new ObservableCollection<string>(
                App.Database.showTableTopic()
                    .OrderBy(i => i.id)
                    .Select(n => n.Name));
        }
        private int currentItem;
        private dict currentDict;
        private string currentTopicName;
        public string CurrentTopicName
        {
            get
            {
                return currentTopicName;
            }
        }
        public int CurrentItem 
        { 
            get => currentItem; 
            set
            {
                if (currentItem != value)
                {
                    currentItem = value;
                    OnPropertyChanged(nameof(CurrentItem));
                    OnPropertyChanged(nameof(CurrentTopicName));
                    currentTopicName = topicsList[value];
                }
            }
        }
        public dict CurrentDict 
        { 
            set
            {
                currentDict = value;
                IEnumerable<topic> lt = App.Database.showTableTopic();
                 currentTopicName = (from n in lt
                                where n.id == currentDict.Topic
                                select n.Name).FirstOrDefault()??"---";
                OnPropertyChanged(nameof(CurrentTopicName));
                if (currentTopicName != "---")
                    CurrentItem = topicsList.IndexOf(currentTopicName);
                else
                    CurrentItem = 0;
            } 
        }
        public ListTopicsViewModel()
        {
            GetTopicList();
            if (topicsList!.Count == 0) currentItem = -1;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<string> TopicsList
        {
            get => topicsList;
            set
            {
                topicsList = value;
                OnPropertyChanged(nameof(TopicsList));
            }
        }

        private ObservableCollection<string> topicsList;
    }
}
