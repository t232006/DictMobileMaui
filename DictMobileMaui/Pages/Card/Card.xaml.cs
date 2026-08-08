
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
        Grid supergrid = new Grid
        {
            Margin = new Thickness(0, 15, 0, 0)
        };
        protected double cardHeight; protected double cardWidth;
        protected bool isAnimating;
        //===================
        protected Border CardBorder;
        protected Grid overlayGrid = new Grid();
        protected Label contentLabel = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        //====================

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
        protected abstract Task MySwipe(SwipeDirection dir);

        protected virtual DataTemplate CreateCardTemplate()
        {
            return new DataTemplate(() =>
            {
                SetupCardBorder();
                //SetupBindings();
                //SetupGestureRecognizers();

                supergrid.Add(CardBorder);
                return supergrid;
            });
        }

        protected virtual void SetupGestureRecognizers()
        {
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
        }

        protected virtual    void SetupBindings()
        {
            supergrid.SetBinding(WidthRequestProperty, new Binding(nameof(CardWidth), source: this));
            supergrid.SetBinding(HeightRequestProperty, new Binding(nameof(CardHeight), source: this));

            Binding param = new Binding(path: ".");
        }

        protected virtual void SetupCardBorder()
        {
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

            CardBorder = new Border
            {
                Content = overlayGrid
            };
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
        /*protected virtual void InitializeCardTemplate()
        {
            _Cards.ItemTemplate = CreateCardTemplate();
        }*/

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


    }

}