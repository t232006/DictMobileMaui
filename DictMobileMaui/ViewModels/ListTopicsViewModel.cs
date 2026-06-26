using IndDictionary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DictMobile.ViewModels
{
    internal class ListTopicsViewModel
    {   
        public void GetTopicList()
        {
            topicsList = new ObservableCollection<string>(
                App.Database.showTableTopic()
                    .OrderBy(i => i.id)
                    .Select(n => n.Name));
        }
        private int currentItem=0;
        public int CurrentItem { get => currentItem;
            set => currentItem = value; 
        }
        public ListTopicsViewModel()
        {
            GetTopicList();
            if (topicsList!.Count == 0) currentItem = -1;
            
        }
        
        public ObservableCollection<string> TopicsList { get =>topicsList; set => TopicsList = value; }
        private ObservableCollection<string> topicsList;
    }
}
