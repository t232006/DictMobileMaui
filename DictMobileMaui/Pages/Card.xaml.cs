using DictMobileMaui;
using DictMobileMaui.games;
using IndDictionary.Converters;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	
	public partial class Card : ContentPage, INotifyPropertyChanged
	{
		double CardHeight; double CardWidth;
		bool _isWord;
		public bool isWord 
		{ 
			get => _isWord;
			set
			{ 
				_isWord = value;
				OnPropertyChanged(nameof(isWord));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
		public Card(bool _word)
		{
			_isWord = _word;
			Cards cards = new Cards();
			InitializeComponent();
			CarouselView Cards = new CarouselView
			{
				VerticalOptions = LayoutOptions.Start,
			};
			Cards.ItemsSource = cards.CardSeq;
			Cards.ItemTemplate = new DataTemplate(() =>
			{
				Grid grid = new Grid
				{
					WidthRequest = CardWidth,
					HeightRequest = CardHeight,
					Margin = new Thickness(0, 15, 0, 0)

				};
				//var binding = new Binding(path: "isWord", source: this, converter: new BoolToBorderStyle());
				Border CardBorder = new Border
				{
					Content = new Label() {	},
				};
				Binding bindBorderStyle = new Binding(path: nameof(isWord), source: this, converter: new BoolToBorderStyle());
				Binding bindTextStyle  = new Binding(path: nameof(isWord), source: this, converter: new BoolToBorderLabelStyle());
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
				grid.Add(CardBorder);
				return grid;
			});
			MainStack.Add(Cards);
		}
		protected override void OnAppearing()
		{
			if (DeviceInfo.Platform==DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
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
