using System.Collections.ObjectModel;
using System.ComponentModel;
using DictMobile.models;
using IndDictionary;

namespace DictMobile.games 
{
	
	public class Tests : Games
	{
		ObservableCollection<dict> pool = new ObservableCollection<dict>();
		dict answ;
		public dict Answer 
		{ 
			get => answ; 
			private set 
			{
				answ = value;
				OnPropertyChanged("Answer");
			}
		}
		public ObservableCollection<dict> Pool 
		{ 
			get => pool;
        }
		public Tests()
		{
			Generate();
		}
		public void Generate()
		{
            var newPool = GetPool(6, selList);
            Random rand = new Random();
            Answer = newPool[rand.Next(0, 6)];
			pool.Clear();
			foreach (dict p in newPool) pool.Add(p);
        }
	}
}
