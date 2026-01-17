using IndDictionary.addition;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace IndDictionary
{
	//public delegate void WhatToSelect(IEnumerable<DateOrTopicClassAux> _passedList);
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DateTopicForm : ContentPage
	{
		WhatToSelect wts;
		IEnumerable<DateOrTopicClassAux> PassedList;
		public DateTopicForm(IEnumerable<DateOrTopicClassAux> _passedList, WhatToSelect _wts)
		{
			PassedList = _passedList;
			wts=_wts;
			InitializeComponent();
		}
		protected override void OnAppearing()
		{
			DataList.ItemsSource = PassedList;
			base.OnAppearing();
		}

		public void onSelect(object sender, ItemTappedEventArgs e)
		{
			var temp = e.Item as DateOrTopicClassAux;
			temp!.Spoted = !temp.Spoted;
		}
		public void onApplyPress(object sender, EventArgs e)
		{
			//wts(PassedList);
			App.Database.selectDatesOrTopics(PassedList, wts);
			Navigation.PopAsync();
			//OnAppearing();
		}
	}
}