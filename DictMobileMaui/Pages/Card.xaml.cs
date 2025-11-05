using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DictMobileMaui.games;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Layouts;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Card : ContentPage
	{
		public Card()
		{
			InitializeComponent();
			CarouselView Cards = new CarouselView
			{
				VerticalOptions = LayoutOptions.Start,
			};
			Cards.ItemsSource = App.Database.getSelected();
			Cards.ItemTemplate = new DataTemplate(() =>
			{
				Border CardBorder = new Border
				{
					Style = (Style)MyStyle["cardStyle"],
					Content = new Label() { Style = (Style)MyStyle["var2Text"] }
				};
				Label lbl = (Label)CardBorder.Content;
				lbl.SetBinding(Label.TextProperty, "Word");
				CardBorder.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = new Command(async () =>
					{
						await CardBorder.ScaleXTo(0.001, 250);
						await CardBorder.ScaleXTo(1, 250);
					})
				});
				CardBorder.Content.SetBinding(Label.TextProperty, "Word", BindingMode.OneWay);
				return new ViewCell
				{
					View = new AbsoluteLayout { Children = { CardBorder } }
				};
			});

		}
		
		
	}

}
