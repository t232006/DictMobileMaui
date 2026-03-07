
//using Android.Gestures;
using DictMobileMaui;
using DictMobileMaui.games;
using IndDictionary.Converters;
using System.ComponentModel;


namespace IndDictionary
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    enum Direction { Up, Down, Left, Right}
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

                Border CardBorder = new Border
                {
                    Content = new Label() { },
                };
                async Task MySwipe(Direction dir)
                {
                    
                    if (isAnimating) return;
                    isAnimating = true;
                    switch (dir)
                    {
                        case Direction.Up:
                            await CardBorder.TranslateTo(0, -cardHeight, 300, Easing.SinIn);
                            App.Database.GetReward((_Cards.CurrentItem as dict)!.Number, true);
                            break;
                        case Direction.Down:
                            await CardBorder.TranslateTo(0, cardHeight, 300, Easing.SinIn);
                            App.Database.GetReward((_Cards.CurrentItem as dict)!.Number, false);
                            break;
                        case Direction.Left:
                            await CardBorder.TranslateTo(-cardWidth, 0, 300, Easing.SinIn);
                            break;
                        case Direction.Right:
                            await CardBorder.TranslateTo(cardWidth, 0, 300, Easing.SinIn);
                            break;
                    };
                    CardBorder.Opacity = 0;
                    if (dir != Direction.Right)
                    {
                        if (_Cards.Position == cards.CardSeq.Count - 1) _Cards.Position = 0;
                        else
                            _Cards.Position++;
                    } else 
                    {
                        if (_Cards.Position == 0) _Cards.Position = cards.CardSeq.Count - 1;
                        else
                            _Cards.Position--;
                    }
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
                CardBorder.Content.SetBinding(Label.StyleProperty, bindTextStyle);

                CardBorder.Content.SetBinding(Label.TextProperty, MultiBind);

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
                    Command = new Command(async () =>
                    {
                        await CardBorder.TranslateTo(0, -CardHeight, 300, Easing.SinIn);
                        //await CardBorder.FadeTo(0,300);
                        CardBorder.Opacity = 0;
                        if (_Cards.Position == cards.CardSeq.Count - 1) _Cards.Position = 0; 
                        else
                            _Cards.Position++;
                        TranslationY = 0;
                        CardBorder.Opacity = 1;
                        App.Database.GetReward((_Cards.CurrentItem as dict)!.Number, true);
                    })
                });
                CardBorder.GestureRecognizers.Add(new SwipeGestureRecognizer
                {
                    Direction = SwipeDirection.Down,
                    Command = new Command(async () =>
                    {
                        await CardBorder.TranslateTo(0, CardHeight, 300, Easing.SinIn);
                        CardBorder.Opacity = 0;
                        if (_Cards.Position == cards.CardSeq.Count - 1) _Cards.Position = 0;
                        else
                            _Cards.Position++;
                        TranslationY = 0;
                        CardBorder.Opacity = 1;
                        App.Database.GetReward((_Cards.CurrentItem as dict)!.Number, false);
                    })
                });
                CardBorder.GestureRecognizers.Add(new SwipeGestureRecognizer
                {
                    Direction = SwipeDirection.Left,
                    Command = new Command(async () =>
                    {
                        if (isAnimating) return;
                        isAnimating = true;
                        await CardBorder.TranslateTo(-CardWidth, 0, 300, Easing.SinIn);
                        CardBorder.Opacity = 0;
                        if (_Cards.Position == cards.CardSeq.Count - 1) _Cards.Position = 0;
                        else
                            _Cards.Position++;
                        CardBorder.TranslationY = 0;
                        CardBorder.TranslationX = 0;
                        CardBorder.Opacity = 1;
                        isAnimating = false;
                    })
                });
                CardBorder.GestureRecognizers.Add(new SwipeGestureRecognizer
                {
                    Direction = SwipeDirection.Right,
                    Command = new Command(async () =>
                    {
                        if (isAnimating) return;
                        isAnimating = true;
                        await CardBorder.TranslateTo(CardWidth, 0, 300, Easing.SinIn);
                        CardBorder.Opacity = 0;
                        if (_Cards.Position == 0) _Cards.Position = cards.CardSeq.Count-1;
                        else
                            _Cards.Position--;
                        //CardBorder.TranslationY = 0;
                        //CardBorder.TranslationX = 0;
                        CardBorder.Opacity = 1;
                        App.Database.GetReward((_Cards.CurrentItem as dict)!.Number, false);
                    })
                });
                grid.Add(CardBorder);
                return grid;
            });
            MainStack.Add(_Cards);
        }

        // Обновляет размеры карточки на основе переданных DIPs (device-independent pixels)
        void UpdateCardSize(double widthDp, double heightDp)
        {
            if (widthDp <= 0 || heightDp <= 0) return;

            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // мобильные: используем 80% экрана
                CardWidth = widthDp * 0.8;
                CardHeight = heightDp * 0.8;
            }
            else
            {
                // десктоп/другие: используем окно если доступно, иначе переданные размеры
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