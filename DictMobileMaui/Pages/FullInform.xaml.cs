
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
				IconImageSource = ImageSource.FromResource("DictMobileMaui.Resources.Images.ok.png")
			};
			CancelItem = new ToolbarItem()
			{
				//Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Priority = 1,
				IconImageSource = ImageSource.FromResource("DictMobileMaui.Resources.Images.cancel.png")
			};
			DeleteItem = new ToolbarItem()
			{
				//Text = "Delete",
				Order = ToolbarItemOrder.Primary,
				Priority = 2,
				IconImageSource = ImageSource.FromResource("DictMobileMaui.Resources.Images.trash_bin_small.png")
			};
			EditItem = new ToolbarItem()
			{
				//Text = "Edit",
				Order = ToolbarItemOrder.Primary,
				Priority = 3,
				IconImageSource = ImageSource.FromResource("DictMobileMaui.Resources.Images.edit1.png")
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

		protected void onConfPress(object Sender, EventArgs e)
		{
			if (TopicSpace.SelectedItem!=null)
			App.Database.saveRecD(TempDict, TopicSpace.SelectedItem.ToString()!);
			Navigation.PopAsync();
		}

		protected void onDeclPress(object Sender, EventArgs e)
		{
			Navigation.PopAsync();
		}

		protected void onDelPress(object Sender, EventArgs e)
		{
			App.Database.deleteRecD((this.BindingContext as dict).Number);
			Navigation.PopAsync();
		}

		protected override void OnAppearing()
		{
			TempTop = App.Database.showTableTopic();	
			TopicSpace.ItemsSource = TempTop.Select(p => p.Name).ToList();
			
			if (!blank)
			{
				TempDict = this.BindingContext as dict;
				var temp = from p in TempTop
						   where p.id == TempDict.Topic
						   select p.Name;
				TopicSpace.SelectedItem = temp.ToList()[0];
				TempDict.Relevation++;
				App.Database.saveRecD(TempDict);	
			}
			else
			{
				TempDict = new dict();
				this.BindingContext = TempDict;
				TopicSpace.SelectedItem = TempTop.ToList()[0].Name;
				//EditBox.IsToggled = true;
				//EditBut.Active = false;
				
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