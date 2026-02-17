using IndDictionary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictMobileMaui.games
{
    public class Accordance:Games
    {
        ObservableCollection<dict> pool=new ObservableCollection<dict>();
        public ObservableCollection<dict> Pool { get => pool; }
        public void Init()
        {
            Random rand = new Random();
            pool.Clear();
            for (byte i=0; i<6; i++)
            {
                pool.Add(selList[rand.Next(0,selList.Count)]);
            }
        }
    }
}
