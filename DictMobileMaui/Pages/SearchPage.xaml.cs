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
		
	}
}