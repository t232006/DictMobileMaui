
//using Android.Gestures;
using DictMobile;
using DictMobile.games;
using DictMobile.models;
using IndDictionary.Converters;
using System.ComponentModel;


namespace IndDictionary
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public abstract partial class Card : ContentPage, INotifyPropertyChanged
    {
        protected double cardHeight; protected double cardWidth;
        protected bool isAnimating;
        public double CardWidth
        {
            get => cardWidth;
            set
            {
                cardWidth = value;
                OnPropertyChanged();
            }
        }
        public double CardHeight
        {
            get => cardHeight;
            set
            {
                cardHeight = value;
                OnPropertyChanged();
            }
        }
        protected CarouselView _Cards;
        protected abstract Task OnCardSwiped(SwipeDirection dir, Border cardBorder);

        protected virtual DataTemplate CreateCardTemplate()
        {
            return new DataTemplate(() =>
            {
                var supergrid = new Grid { Margin = new Thickness(0, 15, 0, 0) };
                supergrid.SetBinding(WidthRequestProperty, new Binding(nameof(CardWidth), source: this));
                supergrid.SetBinding(HeightRequestProperty, new Binding(nameof(CardHeight), source: this));

                var contentLabel = new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                var overlayGrid = new Grid();
                overlayGrid.Children.Add(contentLabel);

                // "+" сверху
                overlayGrid.Children.Add(new Label
                {
                    FontFamily = "Wingdings",
                    Text = $"{(char)0xFD}",
                    FontSize = 40,
                    TextColor = Colors.Green,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, 10, 0, 0)
                });

                // "-" снизу
                overlayGrid.Children.Add(new Label
                {
                    FontFamily = "Wingdings",
                    Text = $"{(char)0xFE}",
                    FontSize = 40,
                    TextColor = Colors.Red,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    Margin = new Thickness(0, 0, 0, 10)
                });
                var cardBorder = new Border { Content = overlayGrid };
                // === Общие жесты свайпа (наследуются всеми потомками) ===
                AddCommonSwipeGestures(cardBorder);

                // === Хук для потомка: добавить свои биндинги / жесты / контент ===
                CustomizeCard(cardBorder, contentLabel);

                supergrid.Add(cardBorder);
                return supergrid;
            });
        }

        protected abstract void CustomizeCard(Border cardBorder, Label contentLabel);

        protected virtual void AddCommonSwipeGestures(Border CardBorder)
        {
            CardBorder.GestureRecognizers.Add(new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Up,
                Command = new Command(async () => await SafeSwipe(SwipeDirection.Up, CardBorder))
            });
            CardBorder.GestureRecognizers.Add(new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Down,
                Command = new Command(async () => await SafeSwipe(SwipeDirection.Down, CardBorder))
            });
            CardBorder.GestureRecognizers.Add(new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Left,
                Command = new Command(async () => await SafeSwipe(SwipeDirection.Left, CardBorder))
            });
            CardBorder.GestureRecognizers.Add(new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Right,
                Command = new Command(async () => await SafeSwipe(SwipeDirection.Right, CardBorder))
            });
        }

        private async Task SafeSwipe(SwipeDirection dir, Border cardBorder)
        {
            if (isAnimating) return;
            isAnimating = true;
            try
            {
                await OnCardSwiped(dir, cardBorder);
            }
            finally
            {
                isAnimating = false;
            }
        }

        public Card()
        {
            Cards cards = new Cards();
            InitializeComponent();
            DeviceDisplay.Current.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
            _Cards = new CarouselView
            {
                VerticalOptions = LayoutOptions.Start,
                IsSwipeEnabled = false,
                Loop = true
            };
            _Cards.ItemTemplate = CreateCardTemplate();
            MainStack.Add(_Cards);
        }
      
        void UpdateCardSize(double widthDp, double heightDp)
        {
            if (widthDp <= 0 || heightDp <= 0) return;

            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                CardWidth = widthDp * 0.8;
                CardHeight = heightDp * 0.8;
            }
            else
            {
                if (this.Window != null)
                {
                    CardHeight = this.Window.Height * 0.75;
                    CardWidth = this.Window.Width * 0.4;
                }
                else
                {
                    CardWidth = widthDp * 0.4;
                    CardHeight = heightDp * 0.75;
                }
            }
        }

        private void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            // Получаем размеры в DIPs: pixels / density
            var info = e.DisplayInfo;
            double widthDp = info.Width / info.Density;
            double heightDp = info.Height / info.Density;
            UpdateCardSize(widthDp, heightDp);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width <= 0 || height <= 0) return;

            // width и height здесь уже в DIPs, можно передать прямо
            UpdateCardSize(width, height);
        }

        protected override void OnAppearing()
        {
            //InitializeCardTemplate();
            // При появлении используем текущие значения окна или DeviceDisplay
            double widthDp = 0, heightDp = 0;
            if (this.Window != null && this.Window.Width > 0 && this.Window.Height > 0)
            {
                widthDp = this.Window.Width;
                heightDp = this.Window.Height;
            }
            else
            {
                var info = DeviceDisplay.Current.MainDisplayInfo;
                widthDp = info.Width / info.Density;
                heightDp = info.Height / info.Density;
            }

            UpdateCardSize(widthDp, heightDp);

            base.OnAppearing();
        }

        protected async Task Swipers(SwipeDirection dir, Border CardBorder)
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
                    break;
                case SwipeDirection.Left:
                    await CardBorder.TranslateTo(-cardWidth, 0, 300, Easing.SinIn);
                    break;
                case SwipeDirection.Right:
                    await CardBorder.TranslateTo(cardWidth, 0, 300, Easing.SinIn);
                    break;
            }
        }


    }

}