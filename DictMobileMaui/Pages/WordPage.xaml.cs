//using DictMobile.Auxilary;
using DictMobile.Auxilary;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;

namespace IndDictionary
{
	//public enum bool {word, translation, none };
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WordPage : ContentPage
	{
		dict focusedItem;
		bool side;
		//bool earlyopen = false; //shows whether page has already opened
		bool showall = true;
		bool _showSecondField = false;
		public bool showSecondField { get => _showSecondField;
            set
            {
                if (_showSecondField != value)
                {
                    _showSecondField = value;
                    OnPropertyChanged(nameof(showSecondField));
                }
            }
        }
		WhatToShow wts = WhatToShow.alltogether;
		ListView ListTable;
		SearchBar searchBar;
        CancellationTokenSource _cts;
        ObservableCollection<dict> items;
		async Task LoadDataAsync(bool _side)
		{
			var data = await Task.Run(() =>
			{
                var raw = App.Database.showTableDict(showall, wts);
                return _side ? raw.OrderBy(t => t.Translation).ToList()
								: raw.OrderBy(t => t.Word).ToList();
			}
			);
			items = new ObservableCollection<dict>(data);
			ListTable.ItemsSource = items;
        }
		
		protected void OnShowTranslation(object? Sender, EventArgs e)
		{
			showSecondField = !showSecondField;
			showSecond.Text=showSecondField?"W-T":"W";
			
		}
		public WordPage(bool _side)
		{
			InitializeComponent();
			side = _side;

			ListTable = new ListView
			{
				//ItemsSource = Data(_side),
				ItemTemplate = new DataTemplate(() =>
				{
					Label MainField = new Label
					{
						LineBreakMode = LineBreakMode.TailTruncation,
						FontSize = 16,
						Padding = 10
					};
					Binding bindVisible = new Binding(path: nameof(showSecondField), source: this);
					//Binding bindLabelSource = new Binding(path: ".");
					//MultiBinding mult = new mu
					Label SecondField = new Label
					{
						FontSize = 16,
						Padding = 10,
					};

					if (_side)
					{
                        MainField.SetBinding(Label.TextProperty, "Translation");
						SecondField.SetBinding(Label.TextProperty, "Word");
                    }

					else
					{
                        MainField.SetBinding(Label.TextProperty, "Word");
                        SecondField.SetBinding(Label.TextProperty, "Translation");
                    }
                    SecondField.SetBinding(Label.IsVisibleProperty, bindVisible);
					
					ExtSwitch extswitch = new ExtSwitch();
					extswitch.Toggled += OnToggled!;
					extswitch.SetBinding(ExtSwitch.IDProperty, "id");
					extswitch.SetBinding(ExtSwitch.IsToggledProperty, "Usersel");
					
					SectorComponent diagram = new SectorComponent();
                    diagram.SetBinding(SectorComponent.AlphaProperty, "Grade");

                    
                     MainField.SizeChanged += (s, e) =>
                     {
                         if (MainField.Height > 0)
                         {
                             // немного отступа, подстраивайте коэффициент под ваш дизайн
                             double target = MainField.Height * 0.9;
                             diagram.WidthRequest = target;
                             diagram.HeightRequest = target;

                             // обновляем layout bounds: X,Y пропорциональные, W/H — абсолютные
                             AbsoluteLayout.SetLayoutBounds(diagram, new Rect(.95, 0.5, diagram.WidthRequest, diagram.HeightRequest));

                             // заставляем перерисовать компонент (если у вашего SectorComponent есть Invalidate/InvalidateMeasure)
                             diagram.Invalidate();
                         }
                     };
                    var cellLayout = new Grid
                    {
                        ColumnDefinitions =
						{
							new ColumnDefinition { Width = GridLength.Star },   // Main
							new ColumnDefinition { Width = GridLength.Auto },   // Second
							new ColumnDefinition { Width = GridLength.Auto },   // Switch
							new ColumnDefinition { Width = GridLength.Auto },    // Diagram

                            new ColumnDefinition { Width = 50 }    // Diagram
						}
                    };
                    cellLayout.Add(MainField, 0, 0);
                    cellLayout.Add(SecondField, 1, 0);
                    cellLayout.Add(extswitch, 2, 0);
                    cellLayout.Add(diagram, 3, 0);
                   
                    
                    return new ViewCell { View = cellLayout };
                }
				)
			};
            NavigationButtons navButtons = new NavigationButtons(this);
			searchBar = new SearchBar();
			searchBar.TextChanged += Searching;
			ListTable.ItemTapped += OnPress;
			MainStack.Add(searchBar, 0, 0);
			MainStack.Add(ListTable, 0, 1);
			MainStack.Add(navButtons, 0, 2);  
        }
		public void PassParams(bool _showAll, WhatToShow _wts)
		{
			showall=_showAll;
			wts=_wts;
			//ListTable.ItemsSource = App.Database.showTableDict(_showAll, _wts);
		}
		protected async void OnPress(object? sender, ItemTappedEventArgs e)
		{
			focusedItem = (dict)e.Item;
			FullInform fullinform = new FullInform(false);
			fullinform.BindingContext = focusedItem;
			await Navigation.PushAsync(fullinform);
		}
		protected void OnToggled(object sender, ToggledEventArgs e)
		{
			focusedItem = App.Database.findOneRecord((sender as ExtSwitch)!.ID)!;
			if (focusedItem != null)
			{
				focusedItem.Usersel = e.Value;
				App.Database.saveRecD(focusedItem);
			}
		}
		protected async void OnAddPressed(object sender, EventArgs e)
		{
			FullInform fullinform = new FullInform(true);
			await Navigation.PushAsync(fullinform);
		}
        private async Task<ObservableCollection<dict>> Search(string needle)
		{
            if (string.IsNullOrEmpty(needle))
			{
				var result=await App.Database.showTableDictAsync(showall, wts);
				return new ObservableCollection<dict>(result); 
			} 
            return side
                ? await App.Database.findRecordsAsync(needle, f => f.Translation)
                : await App.Database.findRecordsAsync(needle, f => f.Word);
        }
		protected async void Searching(Object sender, TextChangedEventArgs e)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            try
            {
                await Task.Delay(300, token); // debounce
                var text = e.NewTextValue;
                var result = await Search(e.NewTextValue);

                var sorted = side
                    ? result.OrderBy(f => f.Translation).ToList()
                    : result.OrderBy(f => f.Word).ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    items = new ObservableCollection<dict>(sorted);
                    ListTable.ItemsSource = items;
                });
            }
            catch (TaskCanceledException) { }
        }

        protected async override void OnAppearing()
        { 
			base.OnAppearing();
			if (searchBar.Text != "")
				{ var result = await Search(searchBar.Text); }
			await LoadDataAsync(side); //do one time only
			//earlyopen = true;
        }
    }
}