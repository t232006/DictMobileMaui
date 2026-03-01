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
        public Accordance()
        {
            Init();
        }
        ObservableCollection<dict> poolword = new ObservableCollection<dict>();
        ObservableCollection<dict> pooltrans = new ObservableCollection<dict>();
        public ObservableCollection<dict> PoolWord { get => poolword; }
        public ObservableCollection<dict> PoolTrans { get => pooltrans; }
        public void Init()
        {
            poolword.Clear(); pooltrans.Clear();   
            poolword = GetPool(6, selList);
            pooltrans = GetPool(6, poolword);
        }
    }
}
