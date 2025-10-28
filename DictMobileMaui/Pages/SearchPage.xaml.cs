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
	public partial class SearchPage : ContentPage
	{
		private string _result;
		public string Result { get => _result; }
		public SearchPage()
		{
			InitializeComponent();
		}
		protected override void OnDisappearing()
		{
			_result = SearchEntry.Text;
			base.OnDisappearing();
		}
		protected void Searching(Object sender, TextChangedEventArgs e)
		{
			
			/* IEnumerable<dict> founded = App.Database.findRecords(SearchEntry.Text, f => f.Word);
			if (founded != null)
				_ListTable.ItemsSource = founded;
			if (e.NewTextValue == "")
				_ListTable.ItemsSource = App.Database.showTableDict(showAll, wts);*/
		}
	}
}