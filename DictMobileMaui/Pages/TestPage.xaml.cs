using DictMobile.games;
using DictMobile.models;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls.StyleSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestPage : ContentPage
	{
		Tests test = new Tests();
		bool word_translation;
		double BorderWidth;
		ListView Variants;
		public TestPage(bool _word_translation)
		{
			InitializeComponent();
			
			word_translation = _word_translation;
			Variants = new ListView
			{
				ItemsSource = test.Pool,
				//RowHeight = Convert.ToInt32( FormHeight * 0.12),
				ItemTemplate = new DataTemplate(() =>
				{
					Border num = new Border
					{
						Content = new Label() { Style = (Style)App.Current.Resources["var1Text"] },
						Style = (Style)App.Current.Resources["var1"],
						
					};
					num.SetBinding(Border.WidthRequestProperty, new Binding(path: nameof(BorderWidth), source: this));

					if (word_translation) num.Content.SetBinding(Label.TextProperty, "Translation", BindingMode.OneWay);
					else
						num.Content.SetBinding(Label.TextProperty, "Word", BindingMode.OneWay);
					return new ViewCell
					{
						View = new Grid { Children = { num } }
					};
				})
			};

			Answer.BindingContext = test;
			if (word_translation) Answer.SetBinding(Label.TextProperty, "Answer.Word");
			else
				Answer.SetBinding(Label.TextProperty, "Answer.Translation");
			MainStack.Add(Variants);
			Variants.ItemTapped += OnVariantTapped;
			this.SizeChanged += OnSizeChanged;

		}

		private void OnSizeChanged(object arg1, EventArgs args)
		{
			Variants.RowHeight = Convert.ToInt32(this.Height * 0.13);
			BorderWidth = this.Width * 0.7;
			Variants.HeightRequest = this.Height * 0.9;
		}

		private void OnVariantTapped(object sender, ItemTappedEventArgs e)
		{
			dict selected = (dict)e.Item;

			if (selected.id == test.Answer.id)
			{
				if (word_translation)
				DisplayAlert("Correct!", $"{test.Answer.Word} - {selected.Translation}", "OK"); else
				DisplayAlert("Correct!", $"{test.Answer.Translation} - {selected.Word}", "OK");
				App.Database.GetReward(selected.id, true);
				test.Generate();
			}
			else
			{
				DisplayAlert("Incorrect", $"Sorry, that is not the correct answer.", "Try Again");
				App.Database.GetReward(selected.id, false);
			}
		}
	}
}