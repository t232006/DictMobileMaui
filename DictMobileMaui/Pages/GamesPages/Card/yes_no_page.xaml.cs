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
            // === Нижний Grid (Yes_No_Progress) ===
            Yes_No_Progress = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = 10 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 10 }
                }
            };

            // TrueAnswers
            TrueAnswers = new Label
            {
                Text = "0",
                Style = (Style)Application.Current.Resources["var5Text"]   // или из Resources страницы, если определено там
            };
            Grid.SetColumn(TrueAnswers, 0);
            Yes_No_Progress.Children.Add(TrueAnswers);

            // "/"
            var slashLabel = new Label
            {
                Text = "/",
                Style = (Style)Application.Current.Resources["var5Text"]
            };
            Grid.SetColumn(slashLabel, 1);
            Yes_No_Progress.Children.Add(slashLabel);

            // CurrentRecord
            CurrentRecord = new Label
            {
                Text = "0",
                Style = (Style)Application.Current.Resources["var5Text"]
            };
            Grid.SetColumn(CurrentRecord, 2);
            Yes_No_Progress.Children.Add(CurrentRecord);

            // TimerBar
            TimerBar = new ProgressBar
            {
                //Progress = 0.9,
                HeightRequest = 10
            };
            Grid.SetColumn(TimerBar, 4);
            Yes_No_Progress.Children.Add(TimerBar);

            // Добавляем нижний Grid во вторую строку
            Grid.SetRow(Yes_No_Progress, 1);
            MainFraim.Children.Add(Yes_No_Progress);

            Yes_No_Progress.IsVisible = true;
            CurrentRecord.Text = Preferences.Get("CurrentRecord", 0).ToString();
            rec = Int16.Parse(CurrentRecord.Text);
            Cards = new yes_no();
            _Cards.ItemsSource = Cards.Output;
            //_Cards.Position = 0;
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
            ShowMistake();
            
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
            CardBorder.TranslationY = 0;
            CardBorder.TranslationX = 0;
            CardBorder.Opacity = 0;
            if (dir != SwipeDirection.Right)
                _Cards.Position = (_Cards.Position + 1) % Cards.Output.Count;
            else
                _Cards.Position = (_Cards.Position - 1 + Cards.Output.Count) % Cards.Output.Count;
            
            CardBorder.Opacity = 1;
            isAnimating = false;
        }
    }

}


