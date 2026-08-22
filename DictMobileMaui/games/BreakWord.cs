using DictMobile.models;

namespace DictMobile.games
{
    public class BreakWord : Games
    {
        private dict theWord;
        private string[] words;
        private char[] letters;
        public string[] Words { get => words; }
        public char[] Letters { get => letters; }
        public dict TheWord
        {
            get
            {
                Random r = new Random();
                theWord = GetPool(false)[r.Next(selList.Count)];
                if (theWord.Phrase) getWords(); else getLetters();    
                return theWord;
            }
        }
        private void getWords()
        {
            Random r = new();
            string[] s = theWord.Translation.Split(' ');   
            string oneWord = "";
            List<string> ss = new();
            for (byte i = 0; i < s.Length; i++)
            {
                do
                {
                    oneWord = s[r.Next(s.Length)];
                } while (ss.Contains(oneWord));
                ss.Add(oneWord);
            }    
           words = ss.ToArray();
        }
        private void getLetters()
        {
            Random r = new();
            string s = theWord.Translation;
            char oneLetter = 'a';
            List<char> ss = new();
            for (byte i = 0; i < s.Length; i++)
            {
                do
                {
                    oneLetter = s[r.Next(s.Length)];
                } while (ss.Contains(oneLetter));
                ss.Add(oneLetter);
            }
                    
            letters = ss.ToArray();
        }
    }
}
