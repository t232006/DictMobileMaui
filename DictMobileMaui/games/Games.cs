using System.Collections.ObjectModel;
using System.ComponentModel;
using DictMobile.models;
using IndDictionary;

namespace DictMobile.games 
{
    public class Games : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected ObservableCollection<dict> selList
		{
			get
			{
				return new ObservableCollection<dict>(App.Database.getSelected());
            }
		}
		protected void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

		public ObservableCollection<dict> GetPool(ObservableCollection<dict> source)
		{
			ObservableCollection<dict> pool = new ObservableCollection<dict>();
			Random rand = new Random();
			pool.Clear();
			int k;
			for (byte i = 0; i < 6; i++)
			{
				do
				{
					k = rand.Next(0, source.Count);
				} while (pool.Contains(source[k]));
				pool.Add(source[k]);
			}
			return pool;
		}
	}
}
