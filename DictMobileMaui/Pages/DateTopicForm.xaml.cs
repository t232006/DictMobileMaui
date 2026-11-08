using DictMobile.models;
using IndDictionary.addition;
using System.Collections.ObjectModel;

namespace IndDictionary
{
	//public delegate void WhatToSelect(IEnumerable<DateOrTopicClassAux> _passedList);
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DateTopicForm : ContentPage
	{
		WhatToSelect wts;
		ObservableCollection<DateOrTopicClassAux> PassedList;
		bool asc = false;
		public DateTopicForm(ObservableCollection<DateOrTopicClassAux> _passedList, WhatToSelect _wts)
		{
			PassedList = _passedList;
			wts=_wts;
            bool isEnabled = (wts != WhatToSelect.dates);
            DeleteBut.IsEnabled = isEnabled;
            AddBut.IsEnabled = isEnabled;
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
		public void onSortPress(object sender, EventArgs e)
		{
			var _PassedList =asc? PassedList.OrderBy(d => d.DaOrTo): PassedList.OrderByDescending(d => d.DaOrTo);
			asc = !asc;
            DataList.ItemsSource = _PassedList;
		}
        public void onDeletePress(object sender, EventArgs e)
        {
			//wts(PassedList);
			var s = DataList.SelectedItem as DateOrTopicClassAux;
			if (s != null)
			{
                App.Database.deleteRecT(s.DaOrTo);
				PassedList.Remove(s);
            }
            
        }
		public async void onAddPress(object sender, EventArgs e)
		{
            string result = await DisplayPromptAsync(
			"Add topic",
			"Enter topic:",
			"OK",
			"Cancel",
			placeholder: "Topic"
			);

            if (!string.IsNullOrWhiteSpace(result))
            {
				App.Database.saveRecT(new topic { Name = result });
				App.TopicsViewModel.GetTopicList();
				await DisplayAlert("Added", $"Topic {result} is added", "OK");
            }
        }
    }
}