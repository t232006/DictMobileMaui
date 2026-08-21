using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.Maui.Controls.Application;

namespace DictMobile.Pages.GamesPages.BreakWord
{
    public class LetterBorder
    {
        public delegate void MethodLetterTapped(bool toWrite, string Text);
        public event MethodLetterTapped LetterTapped;
        public Border Shape;
        Label l;
        private int number;
        private bool selected = false;
        public bool Selected { get => selected; }
        public Point coord { get; set; }
        private void textFound(int num)
        {
            if (num==number)
            {
                selected = true;
                OnLetterTapped();
            }
        }
        private void Init()
        {
            Application.MainPage
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
        public LetterBorder(char Letter, int _number)
        {
            Init();
            number = _number;
            l.Text = Letter.ToString();
        }
        public LetterBorder(string Word, int _number)
        {
            Init();
            number = _number;
            l.Text = Word;
        }
        private void OnLetterTapped()
        {
            selected = !selected;
            if (!selected)
            {
                Shape.Style = (Style)Application.Current.Resources["var6"];
                l.Style = (Style)Application.Current.Resources["var6Text"];
            }
            else
            {
                Shape.Style = (Style)Application.Current.Resources["var6Selected"];
                l.Style = (Style)Application.Current.Resources["var6TextSelected"];
            }
            LetterTapped?.Invoke(Selected, l.Text);
        }
        
    }
}
