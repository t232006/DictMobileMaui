using System.Collections.ObjectModel;
using System.ComponentModel;
using DictMobile.models;
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

		public ObservableCollection<dict> GetPool()
		{
			return GetPool(selList.Count, selList);
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
}
