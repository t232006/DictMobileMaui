using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NavigationButtons : ContentView
	{
		bool translationShow = false;
		public bool TranslationShow { set { translationShow = value; } }
		WordPage WordPage { get; set; }
		public NavigationButtons(WordPage _WordPage)
		{
			WordPage = _WordPage;
			InitializeComponent();
		}
		protected void DictOpen(object sender, EventArgs e)
		{
			//WordPage = new WordPage(translationShow);
			
			Navigation.PopModalAsync();
		}
		protected void ToolsOpen(object sender, EventArgs e)
		{
			//ToolsPage = new NavigationPage(new ToolsPage(WordPage));
			Navigation.PushModalAsync(new NavigationPage(new ToolsPage(WordPage)));
		}
	}
}