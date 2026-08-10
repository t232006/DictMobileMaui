using System.Collections.ObjectModel;
using DictMobile.models;

namespace DictMobile.games 
{
    public class Cards : Games 
	{
		public ObservableCollection<dict> CardSeq 
		{ 
			get => GetPool(selList.Count, selList);
		}
	}
}
