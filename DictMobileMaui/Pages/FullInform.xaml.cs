
using DictMobile.models;
using DictMobile.ViewModels;
using IndDictionary.addition;
using System.Collections.ObjectModel;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FullInform : ContentPage
	{
		dict TempDict, OldDict;
		bool blank;
		//bool editable = false;
		IEnumerable<topic> TempTop;
		ToolbarItem ConfirmItem;
		ToolbarItem CancelItem;
		ToolbarItem DeleteItem;
		ToolbarItem EditItem;

		public FullInform(bool _blank)
		{
			InitializeComponent();
			blank = _blank;
			EditBox.IsToggled=blank;
			ConfirmItem = new ToolbarItem()
			{
				//Text = "Confirm",
				Order = ToolbarItemOrder.Primary,
				Priority = 0,
				IconImageSource = ImageSource.FromResource("DictMobile.Resources.Images.ok.png")
			};
			CancelItem = new ToolbarItem()
			{
				//Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Priority = 1,
				IconImageSource = ImageSource.FromResource("DictMobile.Resources.Images.cancel.png")
			};
			DeleteItem = new ToolbarItem()
			{
				//Text = "Delete",
				Order = ToolbarItemOrder.Primary,
				Priority = 2,
				IconImageSource = ImageSource.FromResource("DictMobile.Resources.Images.trash_bin_small.png")
			};
			EditItem = new ToolbarItem()
			{
				//Text = "Edit",
				Order = ToolbarItemOrder.Primary,
				Priority = 3,
				IconImageSource = ImageSource.FromResource("DictMobile.Resources.Images.edit1.png")
			};
			
			ConfirmItem.Clicked += onConfPress!;
			CancelItem.Clicked += onDeclPress!;
			DeleteItem.Clicked += onDelPress!;
			EditItem.Clicked += onEditBut!;
			if (_blank)
			{
				ToolbarItems.Add(ConfirmItem);
				ToolbarItems.Add(CancelItem);
				ToolbarItems.Add(DeleteItem);
			}
			else ToolbarItems.Add(EditItem);
		}
		protected void onEditBut(object sender, EventArgs e)
		{
			EditBox.IsToggled = !EditBox.IsToggled;
			
			OldDict = (dict)TempDict.Clone();
		}
		protected void onEditBoxToggle(object sender, EventArgs e)
		{
            if (EditBox.IsToggled)
            {
                //(sender as ToolbarItem).;
				if (!blank)
				{
					ToolbarItems.Add(DeleteItem); ToolbarItems.Add(ConfirmItem); ToolbarItems.Add(CancelItem);
				}
                
            }
            else
            {
                //(sender as Button).BackgroundColor = Color.Gainsboro;
                ToolbarItems.Remove(DeleteItem); ToolbarItems.Remove(ConfirmItem); ToolbarItems.Remove(CancelItem);
            }
        }

		async protected void onConfPress(object Sender, EventArgs e)
		{
			EditBox.IsToggled = false;
			if (TopicSpace.SelectedItem != null)
			{
				TempDict.Modification_Time = datesCorrection.toCorrectDate(DateTime.Now.ToString());
				DateTime dt = DateTime.Now.AddMilliseconds(1000);
				OldDict.Modification_Time = datesCorrection.toCorrectDate(dt.ToString());
                //TempDict.DateRec = datesCorrection.toCorrectDate(DateTime.Today.ToString());
                ObservableCollection<dict> found = await App.Database.findRecordsAsync(TempDict.Word, f=>f.Word);
				if (blank)
				foreach (dict d in found)
				{
					if (d.Word.IndexOf(TempDict.Word) >= 0) 
					{
						bool result=await DisplayAlert($"Phrase '{TempDict.Word}' already presents in dictionary", "Add anyway?", "Yes", "Cancel");
						if (!result) return;
					}	
					if (d.Translation.IndexOf(TempDict.Translation) >= 0)
					{
                        bool result = await DisplayAlert($"Phrase '{TempDict.Translation}' already presents in dictionary", "Add anyway?", "Yes", "Cancel");
                        if (!result) return;
                    }
				}
				App.Database.saveRecD(TempDict, OldDict, TopicSpace.SelectedItem.ToString()!);
			}
			
			Navigation.PopAsync();
		}

		protected void onDeclPress(object Sender, EventArgs e)
		{
            EditBox.IsToggled = false;
            Navigation.PopAsync();
		}

		protected void onDelPress(object Sender, EventArgs e)
		{
            EditBox.IsToggled = false;
            App.Database.deleteRecD((this.BindingContext as dict).id);
			Navigation.PopAsync();
		}

		protected override void OnAppearing()
		{
				
			//TopicSpace.ItemsSource = TempTop.Select(p => p.Name).ToList();
			
			if (!blank)
			{
                TempTop = App.Database.showTableTopic();
                TempDict = this.BindingContext as dict;

				App.TopicsViewModel.CurrentDict = TempDict!;
				TempDict!.Relevation++;
				App.Database.saveRecD(TempDict);	
			}
			else
			{
				TempDict = new dict();
				this.BindingContext = TempDict;
				
			}
			//ConfirmB.IsVisible = blank;			
			base.OnAppearing();
		}
		protected override void OnDisappearing()
		{
			
			base.OnDisappearing();
		}
	}
}