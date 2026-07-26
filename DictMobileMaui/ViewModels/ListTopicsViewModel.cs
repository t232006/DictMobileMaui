using IndDictionary;
using System.Collections.ObjectModel;
using System.ComponentModel;


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
        private int currentItem=0;
        public int CurrentItem { get => currentItem; set => currentItem = value; }
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
                OnPropertyChanged(nameof(TopicsList)); // если используешь INotifyPropertyChanged
            }
        }
        private ObservableCollection<string> topicsList;
    }
}
