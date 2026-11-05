using IndDictionary;
using Application = Microsoft.Maui.Controls.Application;

namespace DictMobile.Pages.GamesPages.BreakWord
{
    public class MyBorder
    {
        public delegate void MethodLetterTapped(bool toWrite, string Text);
        public event MethodLetterTapped LetterTapped;
        public Border Shape;
        Label l;
        public Label Content { get => l; }
        string BorderStyleName; string TextStyleName;
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
        
        protected void Init()
        {
            //_page.onEnterText += textFound;
            Shape = new Border
            {
                Style = (Style)Application.Current.Resources[BorderStyleName],
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
                Style = (Style)Application.Current.Resources[TextStyleName],
            };
            Shape.Content = l;
        }
        public MyBorder(char Letter, string _BorderStyleName, string _TextStyleName)
        {
            BorderStyleName = _BorderStyleName;
            TextStyleName = _TextStyleName;
            Init();

            l.Text = Letter.ToString();
            
        }
        public MyBorder(string Word,string _BorderStyleName, string _TextStyleName)
        {
            BorderStyleName = _BorderStyleName;
            TextStyleName = _TextStyleName;
            Init();

            l.Text = Word;
        }

        protected void selectOn()
        {
                Shape.Style = (Style)Application.Current.Resources["var6Selected"];
                l.Style = (Style)Application.Current.Resources["var6TextSelected"];
        }
        protected void selectOff()
        {
                Shape.Style = (Style)Application.Current.Resources[BorderStyleName];//"var6"
                l.Style = (Style)Application.Current.Resources[TextStyleName];//"var6Text"
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
    public class LetterBorder: MyBorder
    {
        public LetterBorder(char Letter, int paddingSize, string _BorderStyleName, string _TextStyleName) : base(Letter, _BorderStyleName, _TextStyleName)
        {  
            Shape.Padding = new Thickness(paddingSize);
        }
        public LetterBorder(string Word, int paddingSize, string _BorderStyleName, string _TextStyleName) : base(Word, _BorderStyleName, _TextStyleName)
        {
            Shape.Padding = new Thickness(paddingSize);
        }
        
    }
}
