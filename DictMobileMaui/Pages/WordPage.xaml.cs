//using DictMobileMaui.Auxilary;
using Microsoft.Maui.Layouts;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WordPage : ContentPage
	{
		dict focusedItem;
		public bool transl { get; }	
		bool showall = true;
		WhatToShow wts = WhatToShow.alltogether;
		ListView ListTable;
		SearchBar searchBar;
        IEnumerable<dict> Data(bool _transl)
		{
			return _transl ? App.Database
                            .showTableDict(true, WhatToShow.alltogether)
                            .OrderBy(t => t.Translation).ToList()
                            : App.Database
                            .showTableDict(true, WhatToShow.alltogether)
                            .OrderBy(t => t.Word).ToList();
        }
		public WordPage(bool _transl)
		{
			InitializeComponent();
			transl = _transl;

			ListTable = new ListView
			{
				ItemsSource = Data(_transl),
				ItemTemplate = new DataTemplate(() =>
				{
					Label MainField = new Label
					{
						LineBreakMode = LineBreakMode.TailTruncation,
						FontSize = 14
					};

					if (transl)
						MainField.SetBinding(Label.TextProperty, "Translation");
					else
						MainField.SetBinding(Label.TextProperty, "Word");
					AbsoluteLayout.SetLayoutBounds(MainField, new Rect(10, 0, .68, AbsoluteLayout.AutoSize));
					AbsoluteLayout.SetLayoutFlags(MainField, AbsoluteLayoutFlags.WidthProportional);
					ExtSwitch extswitch = new ExtSwitch();
					extswitch.Toggled += OnToggled!;
					extswitch.SetBinding(ExtSwitch.IDProperty, "Number");
					extswitch.SetBinding(ExtSwitch.IsToggledProperty, "Usersel");
					AbsoluteLayout.SetLayoutBounds(extswitch, new Rect(.9, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
					AbsoluteLayout.SetLayoutFlags(extswitch, AbsoluteLayoutFlags.PositionProportional);
					return new ViewCell
					{
						View = new AbsoluteLayout
						{
							Children = { extswitch, MainField }
						}
					};
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
        protected void Searching(Object sender, TextChangedEventArgs e)
        {
			IEnumerable<dict> founded;
			//IEnumerable<dict> saved = (IEnumerable<dict>)ListTable.ItemsSource;
			if (transl)
				founded = App.Database
					.findRecords(searchBar.Text, f => f.Translation)
					.OrderBy(f => f.Translation).ToList();
			else
				founded = App.Database
					.findRecords(searchBar.Text, f => f.Word)
					.OrderBy(f => f.Word).ToList();
			ListTable.ItemsSource = founded;
			if (e.NewTextValue == "")
				ListTable.ItemsSource = Data(transl);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
			ListTable.ItemsSource = Data(transl);
        }
    }
}