using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndDictionary;

namespace DictMobileMaui.games
{
	public class Tests
	{
		List<dict> pool = new List<dict>();
		dict answ;
		public dict Answer { get => answ; }
		public List<dict> Pool { get => pool; }
		public Tests()
		{
			generateQuestion();
		}
		void generateQuestion()
		{
			Random rand = new Random();
			byte Rand_answ = (byte)rand.Next(1, 6);
			int k;
			List<dict> tempList = App.Database.getSelected().ToList<dict>();
			for (byte i = 0; i < 6; i++)
			{
				do
				{
					k = (int)rand.Next(0, tempList.Count-1);
				} while (pool.Contains(tempList[k]));
				pool.Add(tempList[k]);
				if (i==Rand_answ) answ = tempList[k];
			}	
		}
	}
}
