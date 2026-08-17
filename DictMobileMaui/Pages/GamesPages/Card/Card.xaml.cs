
//using Android.Gestures;
using DictMobile;
using DictMobile.games;
using DictMobile.models;
using IndDictionary.Converters;
using System.ComponentModel;


namespace IndDictionary
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public abstract partial class Card : BaseGamePage, INotifyPropertyChanged
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
        protected Grid MainFraim;
        protected StackLayout MainStack;
        protected Grid Yes_No_Progress;
        protected Label TrueAnswers;
        protected Label CurrentRecord;
        protected ProgressBar TimerBar;

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
            Resources = new ResourceDictionary();
            Resources.Add("boolToBorderStyle", new BoolToBorderStyle());

            // === Основной Grid ===
            MainFraim = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };

            // === StackLayout (MainStack) ===
            MainStack = new StackLayout();

            // VisualStateManager для MainStack
            var normalGroup = new VisualStateGroup { Name = "NormalGroup" };

            var errorState = new VisualState { Name = "Error" };
            errorState.Setters.Add(new Setter
            {
                Property = BackgroundColorProperty,
                Value = Colors.DarkSalmon
            });

            var normalState = new VisualState { Name = "Normal" };
            normalState.Setters.Add(new Setter
            {
                Property = BackgroundColorProperty,
                Value = Colors.White
            });

            normalGroup.States.Add(errorState);
            normalGroup.States.Add(normalState);

            VisualStateManager.GetVisualStateGroups(MainStack).Add(normalGroup);

            // Добавляем MainStack в первую строку
            Grid.SetRow(MainStack, 0);
            MainFraim.Children.Add(MainStack);

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
                Progress = 0.9,
                HeightRequest = 10
            };
            Grid.SetColumn(TimerBar, 4);
            Yes_No_Progress.Children.Add(TimerBar);

            // Добавляем нижний Grid во вторую строку
            Grid.SetRow(Yes_No_Progress, 1);
            MainFraim.Children.Add(Yes_No_Progress);

            // Устанавливаем Content страницы
            //Content = MainFraim;

            Cards cards = new Cards();
            



            DeviceDisplay.Current.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
            _Cards = new CarouselView
            {
                VerticalOptions = LayoutOptions.Start,
                IsSwipeEnabled = false,
                Loop = true
            };
            _Cards.ItemTemplate = CreateCardTemplate();
            MainStack.Add(_Cards);
            //InitializeComponent();
            SetPageContent(MainFraim);
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
                    CardHeight = this.Window.Height * 0.72;
                    CardWidth = this.Window.Width * 0.4;
                }
                else
                {
                    CardWidth = widthDp * 0.4;
                    CardHeight = heightDp * 0.72;
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