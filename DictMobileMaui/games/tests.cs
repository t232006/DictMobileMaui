using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DictMobileMaui.games;
using IndDictionary;

namespace DictMobileMaui.games 
{
	 
	public class Games : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected ObservableCollection<dict> selList = new ObservableCollection<dict>(App.Database.getSelected());
		protected void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
	}

	public class Cards : Games 
	{
		public ObservableCollection<dict> CardSeq 
		{ get
			{
				Random rand = new Random();
				ObservableCollection<dict> cardSeq = new ObservableCollection<dict>();
				List<int> usedIndexes = new List<int>();
				int k;
				for (int i = 0; i < selList.Count; i++)
				{
					do
					{
						k = (int)rand.Next(0, selList.Count);
					} while (usedIndexes.Contains(k));
					usedIndexes.Add(k);
					cardSeq.Add(selList[k]);
				}
				return cardSeq;
			}
		}
	}
	
	public class Tests : Games
	{
		ObservableCollection<dict> pool = new ObservableCollection<dict>();
		dict answ;

		public event PropertyChangedEventHandler? PropertyChanged;
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
			//ObservableCollection<dict> selList = new ObservableCollection<dict>(App.Database.getSelected());
			for (byte i = 0; i < 6; i++)
			{
				do
				{
					k = (int)rand.Next(0, selList.Count);
				} while (pool.Contains(selList[k]));
				pool.Add(selList[k]);
				if (i==Rand_answ) Answer = selList[k];
			}	
		}
	}
}
