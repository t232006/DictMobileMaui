using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DictMobileMaui;
using DictMobileMaui.games;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Layouts;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Card : ContentPage
	{
		double CardHeight; double CardWidth;
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
				Grid grid = new Grid
				{
					WidthRequest = CardWidth,
					HeightRequest = CardHeight,
					Margin = new Thickness(0, 15, 0, 0)

				};

				Border CardBorder = new Border
				{
					Style = (Style)App.Current.Resources["CardStyle"],
					Content = new Label() { Style = (Style)App.Current.Resources["CardTextStyle"] }
				};
				
				CardBorder.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = new Command(async () =>
					{
						await CardBorder.ScaleXTo(0.001, 250, Easing.SinInOut);
						await CardBorder.ScaleXTo(1, 250, Easing.SinInOut);
					})
				});
				CardBorder.Content.SetBinding(Label.TextProperty, "Word", BindingMode.OneWay);
				grid.Add(CardBorder);
				return grid;
			});
			MainStack.Add(Cards);

		}
		protected override void OnAppearing()
		{
			if (DeviceInfo.Platform==DevicePlatform.Android && DeviceInfo.Platform == DevicePlatform.iOS)
			{
				CardHeight = App.screenHeight * 0.9;
				CardWidth = App.screenWidth * 0.8;
			} else
			{	
				CardHeight=this.Window.Height * 0.75;
				CardWidth=this.Window.Width * 0.4;
			}

			base.OnAppearing();
		}


	}

}
