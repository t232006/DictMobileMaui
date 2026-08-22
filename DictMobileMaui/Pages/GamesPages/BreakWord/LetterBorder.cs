using IndDictionary;
using Application = Microsoft.Maui.Controls.Application;

namespace DictMobile.Pages.GamesPages.BreakWord
{
    public class LetterBorder
    {
        public delegate void MethodLetterTapped(bool toWrite, string Text);
        public event MethodLetterTapped LetterTapped;
        public Border Shape;
        Label l;
        //private int number;
        private bool selected = false;
        public bool Selected 
        { 
            get => selected; set
            {
                if (value == true) selectOn(); else selectOff();
            }
        }
        //public Point coord { get; set; }
        
        private void Init()
        {
            //_page.onEnterText += textFound;
            Shape = new Border
            {
                Style = (Style)Application.Current.Resources["var6"],
                Padding = new Thickness(10)
            };
            Shape.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    OnLetterTapped();
                })
            });
            l = new Label
            {
                Style = (Style)Application.Current.Resources["var6Text"],
            };
            Shape.Content = l;
        }
        public LetterBorder(char Letter, int _number, GatherWord _page)
        {
            Init();
            
            l.Text = Letter.ToString();
        }
        public LetterBorder(string Word, int _number, GatherWord _page)
        {
            Init();
            
            l.Text = Word;
        }
        private void selectOn()
        {
                Shape.Style = (Style)Application.Current.Resources["var6Selected"];
                l.Style = (Style)Application.Current.Resources["var6TextSelected"];
        }
        private void selectOff()
        {
                Shape.Style = (Style)Application.Current.Resources["var6"];
                l.Style = (Style)Application.Current.Resources["var6Text"];
        }
        public void OnLetterTapped()
        {
            selected = !selected;
            if (!selected)
            {
                selectOff();
            }
            else
            {
                selectOn();
            }
            LetterTapped?.Invoke(Selected, l.Text);
        }
        
    }
}
