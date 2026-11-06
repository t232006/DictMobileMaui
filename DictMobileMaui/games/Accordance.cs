using DictMobile.models;
using Google.Apis.Drive.v3.Data;
using System.Collections.ObjectModel;
using IndDictionary;

namespace DictMobile.games
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
        public void GetReward(bool increase, string Translation)
        {
            int id = poolword.Where(t => t.Translation == Translation).Select(i => i.id).First();
            IndDictionary.App.Database.GetReward(id, increase);
        }

        public void Init()
        {
            poolword.Clear(); pooltrans.Clear();   
            poolword = GetPool(selList);
            pooltrans = GetPool(poolword);
        }
        
    }
}
