using DictMobileMaui.games;
using Microsoft.Maui.Controls.StyleSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Maui.Controls.Shapes;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestPage : ContentPage
	{
		public TestPage(bool word_translation)
		{
			Tests test = new Tests();
			ListView Variants = new ListView
			{
				ItemsSource = test.Pool,
				ItemTemplate = new DataTemplate(() =>
				{
					Border num = new Border
					{
						StrokeShape = new RoundRectangle
						{
							CornerRadius = 20
						},
						StrokeThickness = 10,
						Margin = new Thickness(5),
						Padding = new Thickness(5),
						BackgroundColor = Color.FromArgb("8fd4c1")

					};
					Label MainField = new Label { FontSize = 16};
					if (word_translation) MainField.SetBinding(Label.TextProperty, "Translation");
					else
						MainField.SetBinding(Label.TextProperty, "Word");
					return new ViewCell
					{
						View = new AbsoluteLayout { Children = { num, MainField } }
					};
					
				})
			};
			Variants.SeparatorVisibility = SeparatorVisibility.Default;
			Variants.SeparatorColor = Colors.Gray;
			//Variants.HeightRequest = 17;
			

			InitializeComponent();
			//this.Resources.Add(StyleSheet.FromResource("styles/testStyles.css", IntrospectionExtensions.GetTypeInfo(typeof(TestPage)).Assembly));
			answer.BindingContext = test.Answer;
			if (word_translation) answer.SetBinding(Label.TextProperty, "Word"); else 
				answer.SetBinding(Label.TextProperty, "Translation");
			MainStack.Add(Variants);
		}
	}
}