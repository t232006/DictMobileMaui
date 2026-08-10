using DictMobile.games;
using DictMobile.models;

namespace IndDictionary
{
    public partial class Yes_No_Page : Card	
    {
        yes_no Cards;
        public Yes_No_Page()
        {
            Yes_No_Progress.IsVisible = true;
            CurrentRecord.Text = Preferences.Get("CurrentRecord", 0).ToString();
            Cards = new yes_no();
            _Cards.ItemsSource = Cards.Output;
            Cards.toPunish += MakeMistake;
        }
        public async void MakeMistake()
        {
            VisualStateManager.GoToState(MainStack, "Error");
            await Task.Delay(400);
            VisualStateManager.GoToState(MainStack, "Normal");
        }
        protected override void CustomizeCard(Border cardBorder, Label contentLabel)
        {
            //contentLabel.BindingContext = _Cards.CurrentItem;
            contentLabel.SetBinding(Label.TextProperty, "Quantor");
            cardBorder.Style = (Style)App.Current.Resources["var5"];
            contentLabel.Style = (Style)App.Current.Resources["var5Text"];
            MainStack.BackgroundColor = Colors.AliceBlue;
        }

        protected override async Task OnCardSwiped(SwipeDirection dir, Border CardBorder)
        {
            switch (dir)
            {
                case SwipeDirection.Up:
                    await CardBorder.TranslateTo(0, -cardHeight, 300, Easing.SinIn);
                    (_Cards.CurrentItem as yesnoModel)!.UsersAnswer = true;
                    break;
                case SwipeDirection.Down:
                    await CardBorder.TranslateTo(0, cardHeight, 300, Easing.SinIn);
                    (_Cards.CurrentItem as yesnoModel)!.UsersAnswer = false;
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
                _Cards.Position = (_Cards.Position + 1) % Cards.Output.Count;
            else
                _Cards.Position = (_Cards.Position - 1 + Cards.Output.Count) % Cards.Output.Count;
            CardBorder.TranslationY = 0;
            CardBorder.TranslationX = 0;
            CardBorder.Opacity = 1;
            isAnimating = false;
        }
    }

}


