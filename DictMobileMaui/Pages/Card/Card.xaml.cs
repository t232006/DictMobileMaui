
//using Android.Gestures;
using DictMobile;
using DictMobile.games;
using DictMobile.models;
using IndDictionary.Converters;
using System.ComponentModel;


namespace IndDictionary
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Card : ContentPage, INotifyPropertyChanged
    {
        double cardHeight; double cardWidth;
        bool isAnimating;
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
        bool _isWord;
        CarouselView _Cards;

        public bool isWord
        {
            get => _isWord;
            set
            {
                _isWord = value;
                OnPropertyChanged(nameof(isWord));
            }
        }
        public Card(bool _word)
        {
            _isWord = _word;
            Cards cards = new Cards();
            InitializeComponent();
            DeviceDisplay.Current.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
            _Cards = new CarouselView
            {
                VerticalOptions = LayoutOptions.Start,
                IsSwipeEnabled = false,
                Loop = true
            };
            _Cards.ItemsSource = cards.CardSeq;
            _Cards.ItemTemplate = new DataTemplate(() =>
            {
                Grid grid = new Grid
                {
                    Margin = new Thickness(0, 15, 0, 0)
                };

                // bind WidthRequest/HeightRequest к свойствам страницы, чтобы шаблон реагировал на изменения размеров
                grid.SetBinding(WidthRequestProperty, new Binding(nameof(CardWidth), source: this));
                grid.SetBinding(HeightRequestProperty, new Binding(nameof(CardHeight), source: this));

                var contentLabel = new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                var overlayGrid = new Grid();

                // основной текст карточки
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

                Border CardBorder = new Border
                {
                    Content = overlayGrid
                };
                async Task MySwipe(SwipeDirection dir)
                {
                    
                    if (isAnimating) return;
                    isAnimating = true;
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
                    };
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
                CardBorder.GestureRecognizers.Add(new SwipeGestureRecognizer
                {
                    Direction = SwipeDirection.Up,
                    Command = new Command(async () => await MySwipe(SwipeDirection.Up))
                });
                CardBorder.GestureRecognizers.Add(new SwipeGestureRecognizer
                {
                    Direction = SwipeDirection.Down,
                    Command = new Command(async () => await MySwipe(SwipeDirection.Down))
                });
                CardBorder.GestureRecognizers.Add(new SwipeGestureRecognizer
                {
                    Direction = SwipeDirection.Left,
                    Command = new Command(async () => await MySwipe(SwipeDirection.Left))
                });
                CardBorder.GestureRecognizers.Add(new SwipeGestureRecognizer
                {
                    Direction = SwipeDirection.Right,
                    Command = new Command(async () => await MySwipe(SwipeDirection.Right))
                });
                grid.Add(CardBorder);
                return grid;
            });
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


    }

}