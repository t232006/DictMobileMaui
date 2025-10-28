using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IndDictionary.addition
{
	public class DateOrTopicClassAux :INotifyPropertyChanged
    {
		string daorto;
		bool spoted;

		public string DaOrTo
		{
			get { return daorto; }
			set
			{
				daorto = value;
				OnPropertyChanged("DaOrTo");
			}
		}
		public bool Spoted
		{
			get { return spoted; }
			set
			{
				spoted = value;
				OnPropertyChanged("Spoted");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged (string prop)
		{ 
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}	
}
