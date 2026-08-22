
//using Android.Gestures;
using DictMobile;
using DictMobile.games;
using DictMobile.models;
using IndDictionary.Converters;
using System.ComponentModel;


namespace IndDictionary
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Know_Dont : Card
    { 
        bool _isWord;
        Cards cards;

        public bool isWord
        {
            get => _isWord;
            set
            {
                _isWord = value;
                OnPropertyChanged(nameof(isWord));
            }
        }
        protected override async Task OnCardSwiped(SwipeDirection dir, Border CardBorder)
        {
            switch (dir)
            {
                case SwipeDirection.Up:
                    await CardBorder.TranslateTo(0, -cardHeight, 300, Easing.SinIn);
                    App.Database.GetReward((_Cards.CurrentItem as dict)!.id, true);
                    break;
                case SwipeDirection.Down:
                    await CardBorder.TranslateTo(0, cardHeight, 300, Easing.SinIn);
                    App.Database.GetReward((_Cards.CurrentItem as dict)!.id, false);
                    ShowMistake();
                    break;
                case SwipeDirection.Left:
                    await CardBorder.TranslateTo(-cardWidth, 0, 300, Easing.SinIn);
                    break;
                case SwipeDirection.Right:
                    await CardBorder.TranslateTo(cardWidth, 0, 300, Easing.SinIn);
                    break;
            }
            CardBorder.Opacity = 0;
            if (dir != SwipeDirection.Right)
                _Cards.Position = (_Cards.Position + 1) % cards.CardSeq.Count;
            else
                _Cards.Position = (_Cards.Position - 1 + cards.CardSeq.Count) % cards.CardSeq.Count;
            CardBorder.TranslationY = 0;
            CardBorder.TranslationX = 0;
            CardBorder.Opacity = 1;
            isAnimating = false;
        }
        public Know_Dont(bool _word) : base()
        {
            _isWord = _word;
            cards = new Cards();       
            _Cards.ItemsSource = cards.CardSeq;
            
        }
        protected override void CustomizeCard(Border CardBorder, Label contentLabel)
        {
            Binding bindBorderStyle = new Binding(path: nameof(isWord), source: this, converter: new BoolToBorderStyle());
            Binding bindTextStyle = new Binding(path: nameof(isWord), source: this, converter: new BoolToBorderLabelStyle());
            MultiBinding MultiBind = new MultiBinding
            {
                Converter = new BoolToBorderText(),
            };
            Binding bind = new Binding(
                path: nameof(isWord),
                source: this
                );
            Binding param = new Binding(path: ".");
            MultiBind.Bindings.Add(bind); MultiBind.Bindings.Add(param);
            CardBorder.SetBinding(Border.StyleProperty, bindBorderStyle);
            contentLabel.SetBinding(Label.StyleProperty, bindTextStyle);
            contentLabel.SetBinding(Label.TextProperty, MultiBind);
            CardBorder.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await CardBorder.ScaleXTo(0.001, 250, Easing.SinInOut);
                    isWord = !isWord;
                    //rotate(isWord);
                    await CardBorder.ScaleXTo(1, 250, Easing.SinInOut);
                })
            });
        }
    }
}