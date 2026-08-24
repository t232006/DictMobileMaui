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
            List<string> conteinter = s.ToList();
            List<string> ss = new();
            while (conteinter.Any())
            {
                oneWord = conteinter[r.Next(conteinter.Count)];
                ss.Add(oneWord);
                conteinter.Remove(oneWord);
            }  
           words = ss.ToArray();
        }
        private void getLetters()
        {
            Random r = new();
            char oneLetter = 'a';
            List<char> conteinter = theWord.Translation.ToList();
            List<char> ss = new();
            while (conteinter.Any())
            {
                oneLetter = conteinter[r.Next(conteinter.Count)];
                ss.Add(oneLetter);
                conteinter.Remove(oneLetter);
            }        
            letters = ss.ToArray();
        }
    }
}
