using System.Collections.ObjectModel;
using System.ComponentModel;
using IndDictionary;

namespace DictMobile.games 
{

	public class Games : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected ObservableCollection<dict> selList = new ObservableCollection<dict>(App.Database.getSelected());
		protected void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

		public ObservableCollection<dict> GetPool(int count, ObservableCollection<dict> Source)
		{
			ObservableCollection<dict> pool = new ObservableCollection<dict>();
			Random rand = new Random();
			pool.Clear();
			int k;
			for (byte i = 0; i < count; i++)
			{
				do
				{
					k = rand.Next(0, Source.Count);
				} while (pool.Contains(Source[k]));
				pool.Add(Source[k]);
			}
			return pool;
		}
	}

    public class Cards : Games 
	{
		public ObservableCollection<dict> CardSeq 
		{ 
			get => GetPool(selList.Count, selList);
		}
	}
	
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
