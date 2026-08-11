using DictMobile.games;
using DictMobile.models;
using DictMobile.Pages;

namespace IndDictionary
{
    public partial class Yes_No_Page : Card	
    {
        yes_no Cards;
        IDispatcherTimer timer;
        short ta = 0;
        short rec;
        public Yes_No_Page()
        {
            Yes_No_Progress.IsVisible = true;
            CurrentRecord.Text = Preferences.Get("CurrentRecord", 0).ToString();
            rec = Int16.Parse(CurrentRecord.Text);
            Cards = new yes_no();
            _Cards.ItemsSource = Cards.Output;
            timer = Dispatcher.CreateTimer();
            timer.Tick += onTimerTick;
            TimerBar.Progress = 1;
            timer.Interval = TimeSpan.FromSeconds(1);
            Cards.toPunish += MakeMistake;
        }
        private void onTimerTick(object sender, EventArgs e)
        {
            double progr = TimerBar.Progress;
            progr -= 1.0 / 60.0;
            TimerBar.ProgressTo(progr, 1000, Easing.Linear);
            if (TimerBar.Progress <= 0)
            {
                timer.Stop();
                if (ta > rec)
                {
                    rec = ta;
                    Preferences.Set("CurrentRecord", rec);
                }
                Navigation.PushAsync(new AfterYes_NoPage(ta, rec));
            }
                
        }
        public async void MakeMistake()
        {
            ta = Int16.Parse(TrueAnswers.Text);
            if (ta>=0)
            {
                ta--;
                TrueAnswers.Text = ta.ToString();
            }
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
            ta = Int16.Parse(TrueAnswers.Text);
            timer.Start();
            switch (dir)
            {
                case SwipeDirection.Up:
                    await CardBorder.TranslateTo(0, -cardHeight, 300, Easing.SinIn);
                    (_Cards.CurrentItem as yesnoModel)!.UsersAnswer = true;
                    ta++;
                    break;
                case SwipeDirection.Down:
                    await CardBorder.TranslateTo(0, cardHeight, 300, Easing.SinIn);
                    (_Cards.CurrentItem as yesnoModel)!.UsersAnswer = false;
                    ta++;
                    break;
                case SwipeDirection.Left:
                    await CardBorder.TranslateTo(-cardWidth, 0, 300, Easing.SinIn);
                    break;
                case SwipeDirection.Right:
                    await CardBorder.TranslateTo(cardWidth, 0, 300, Easing.SinIn);
                    break;
            }
            TrueAnswers.Text = ta.ToString();
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


