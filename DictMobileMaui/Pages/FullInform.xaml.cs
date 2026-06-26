
using DictMobile.ViewModels;
using IndDictionary.addition;
using System.Collections.ObjectModel;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FullInform : ContentPage
	{
		dict TempDict;
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
			if (EditBox.IsToggled)
			{
				//(sender as ToolbarItem).;
				ToolbarItems.Add(DeleteItem); ToolbarItems.Add(ConfirmItem); ToolbarItems.Add(CancelItem);
			} else
			{
				//(sender as Button).BackgroundColor = Color.Gainsboro;
				ToolbarItems.Remove(DeleteItem); ToolbarItems.Remove(ConfirmItem); ToolbarItems.Remove(CancelItem);
			}
		}

		async protected void onConfPress(object Sender, EventArgs e)
		{
			if (TopicSpace.SelectedItem != null)
			{
				TempDict.Modification_Time = datesCorrection.toCorrectDate(DateTime.Now.ToString());
                TempDict.DateRec = datesCorrection.toCorrectDate(DateTime.Today.ToString());
				ObservableCollection<dict> found = await App.Database.findRecordsAsync(TempDict.Word, f=>f.Word);
				foreach (dict d in found)
				{
					if ((d.Word.IndexOf(TempDict.Word) >= 0) || (d.Translation.IndexOf(TempDict.Translation) >= 0))
					{
						bool result=await DisplayAlert($"Word {TempDict.Word} already presents in dictionary", "Add anyway?", "Yes", "Cancel");
						if (!result) return;
					}			
				}
				App.Database.saveRecD(TempDict, TopicSpace.SelectedItem.ToString()!);
			}
			
			Navigation.PopAsync();
		}

		protected void onDeclPress(object Sender, EventArgs e)
		{
			Navigation.PopAsync();
		}

		protected void onDelPress(object Sender, EventArgs e)
		{
			App.Database.deleteRecD((this.BindingContext as dict).id);
			Navigation.PopAsync();
		}

		protected override void OnAppearing()
		{
			TempTop = App.Database.showTableTopic();	
			//TopicSpace.ItemsSource = TempTop.Select(p => p.Name).ToList();
			
			if (!blank)
			{
				TempDict = this.BindingContext as dict;
				var temp = from p in TempTop
						   where p.id == TempDict.Topic
						   select p.Name;
				
					TempDict.Relevation++;
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