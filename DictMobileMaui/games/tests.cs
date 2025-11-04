using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndDictionary;

namespace DictMobileMaui.games
{
	public class Tests : INotifyPropertyChanged
	{
		ObservableCollection<dict> pool = new ObservableCollection<dict>();
		dict answ;

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

		public dict Answer 
		{ 
			get => answ; 
			private set 
			{
				answ = value;
				OnPropertyChanged("Answer");
			}
		}
		public ObservableCollection<dict> Pool { get => pool; }
		public Tests()
		{
			generateQuestion();
		}
		public void generateQuestion()
		{
			Random rand = new Random();
			pool.Clear();
			byte Rand_answ = (byte)rand.Next(0, 6);
			int k;
			ObservableCollection<dict> tempList = new ObservableCollection<dict>(App.Database.getSelected());
			for (byte i = 0; i < 6; i++)
			{
				do
				{
					k = (int)rand.Next(0, tempList.Count);
				} while (pool.Contains(tempList[k]));
				pool.Add(tempList[k]);
				if (i==Rand_answ) Answer = tempList[k];
			}	
		}
	}
}
