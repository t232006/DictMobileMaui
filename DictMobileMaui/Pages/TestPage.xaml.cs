using DictMobileMaui.games;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls.StyleSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestPage : ContentPage
	{
		Tests test = new Tests();
		bool word_translation;
		public TestPage(bool _word_translation)
		{
			InitializeComponent();
			word_translation = _word_translation;
			ListView Variants = new ListView
			{
				ItemsSource = test.Pool,
				ItemTemplate = new DataTemplate(() =>
				{
					Border num = new Border
					{
						Content = new Label() { Style = (Style)MyStyle["var1Text"] },
						Style = (Style)MyStyle["var1"]
					};

					if (word_translation) num.Content.SetBinding(Label.TextProperty, "Translation", BindingMode.OneWay);
					else
						num.Content.SetBinding(Label.TextProperty, "Word", BindingMode.OneWay);
					return new ViewCell
					{
						View = new AbsoluteLayout { Children = { num } }
					};
				})
			};

			//this.Resources.Add(StyleSheet.FromResource("styles/testStyles.css", IntrospectionExtensions.GetTypeInfo(typeof(TestPage)).Assembly));
			Answer.BindingContext = test;
			if (word_translation) Answer.SetBinding(Label.TextProperty, "Answer.Word");
			else
				Answer.SetBinding(Label.TextProperty, "Answer.Translation");
			MainStack.Add(Variants);
			Variants.ItemTapped += OnVariantTapped;

		}
		private void OnVariantTapped(object sender, ItemTappedEventArgs e)
		{
			dict selected = (dict)e.Item;

			if (selected.Number == test.Answer.Number)
			{
				if (word_translation)
				DisplayAlert("Correct!", $"{test.Answer.Word} - {selected.Translation}", "OK"); else
				DisplayAlert("Correct!", $"{test.Answer.Translation} - {selected.Word}", "OK");
				App.Database.GetReward(selected.Number, true);
				test.generateQuestion();
			}
			else
			{
				DisplayAlert("Incorrect", $"Sorry, that is not the correct answer.", "Try Again");
				App.Database.GetReward(selected.Number, false);
			}
		}
	}
}