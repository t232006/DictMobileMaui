using DictMobile.games;
using DictMobile.models;

namespace IndDictionary
{
    public partial class Yes_No_Page : Card	
    {
        yes_no Cards;
        public Yes_No_Page()
        {
            Cards = new yes_no();
            _Cards.ItemsSource = Cards.Output;
        }
        protected override void CustomizeCard(Border cardBorder, Label contentLabel)
        {
            contentLabel.BindingContext = _Cards;
            contentLabel.SetBinding(Label.TextProperty, "Quantor");
        }

        protected override async Task OnCardSwiped(SwipeDirection dir, Border CardBorder)
        {
            await Swipers(dir, CardBorder);
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


