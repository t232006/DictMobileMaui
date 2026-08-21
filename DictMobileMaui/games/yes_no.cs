using DictMobile.models;
using IndDictionary;
using System.Collections.ObjectModel;

namespace DictMobile.games
{
    public class yesnoModel
    {
        public delegate void Punish();
        public event Punish onPunish;
        void GetRewordLocal(bool forward)
        {
            if ((word.Word == translation.Word) || (word.Translation == translation.Translation))
            {
                App.Database.GetReward(word.id, forward);
                if (word.id != translation.id) //if there are not the same, but words are equal
                    App.Database.GetReward(translation.id, forward);
                if (forward == false) onPunish?.Invoke();
            }
            else
            {
                App.Database.GetReward(word.id, !forward);
                App.Database.GetReward(translation.id, !forward);
                if (forward == true) onPunish?.Invoke();
            }
        }
        //string quantor;
        private dict? word;
        private dict? translation;
        public string Quantor
        {
            get
            {
                return $"{word.Word} \n = \n {translation.Translation} ";
            }
        }
        internal dict? Word 
        {
            set => word = value;
        }
        internal dict? Translation
        {
            set => translation = value;
        }
        public bool UsersAnswer
        {
            set
            {
                if (value)
                    GetRewordLocal(true);
                else
                    GetRewordLocal(false);
            }
        }
    }
    public class yes_no:Games
    {
        public delegate void Punish();
        public event Punish toPunish;
        ObservableCollection<yesnoModel> output;
        public ObservableCollection<yesnoModel> Output
        {
            get => output;
        }
        public yes_no()
        {
            yes_no_seq = GetPool(false);
            output = new();
            Random r = new Random();
            foreach (dict d in yes_no_seq)
            {
                int bit = r.Next(2);
                yesnoModel m = new();
                m.Word = d;
                m.onPunish += ()=> toPunish?.Invoke();
                if (bit == 1)
                {
                    int randVar = r.Next(yes_no_seq.Count);
                    m.Translation = yes_no_seq[randVar];
                }
                else
                    m.Translation = d;
                output.Add(m);
            }
        }
        ObservableCollection<dict> yes_no_seq;
        public ObservableCollection<dict> Yes_no_seq
        {
            get => yes_no_seq;
        }
    }
}
