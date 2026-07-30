using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace IndDictionary.Pages
{
	public class BaseInfo 
	{
		public string filename { get; set; }
		public string wordsCount { get; set; }
		public string lastDate { get; set; } 
	}
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FilesList : ContentPage
	{
		public List<BaseInfo> Items { get; set; }
		public FilesList()
		{
			InitializeComponent();
			//DirectoryInfo AppDir = new DirectoryInfo(App.APPFOLDER);
			//FList.ItemsSource = Directory.GetFiles(App.APPFOLDER).Select(fi=>Path.GetFileName(fi));
			//App.Current.Properties.Remove("dbPath");
			this.BindingContext = this;
			PageRefresh();
			
		}
		protected void PageRefresh()
		{
			
			Items = new List<BaseInfo>();
			IEnumerable<string> fileList = Directory.GetFiles(App.APPFOLDER)
													.Select(fi => Path.GetFileName(fi))
													.Where(fi => Path.GetExtension(fi)==".db");
			byte o=0; byte k=0; // o - index of current database; k - iterator
			foreach (string s in fileList)
			{
				BaseInfo Item = new BaseInfo();
				Item.filename = s;
				string myKey = Path.Combine(App.APPFOLDER, s);
				if (Preferences.ContainsKey(myKey))
				{
					string[] ss = Preferences.Get(myKey, "").Split('.');
					Item.wordsCount = ss[0];
					if (ss[1].Length < 3)
						Item.lastDate = $"{ss[1]}.{ss[2]}.{ss[3]}";
					else
						Item.lastDate = ss[1];
					Item.filename = s;
				}
				if (Preferences.Get("current","") == Path.Combine(App.APPFOLDER, s))
					o = k;
				k++;
				Items.Add(Item);
			}
			FList.ItemsSource = Items;
			FList.SelectedItem = Items.ElementAt(o);
		}
		protected void OnDelete(Object Sender, EventArgs e)
		{
			Button button = (Button) Sender;
			string filename = (button.CommandParameter as BaseInfo).filename;
			//App.Current.Properties.TryGetValue("current", out o);
			string o = Preferences.Get("current", "");
			if (Path.GetFileName(o) == filename)
				DisplayAlert("Unable to delete current database. Select another one.", "Error", "Ok");
			else
			{
				File.Delete(Path.Combine(App.APPFOLDER,filename));
				Preferences.Remove(Path.Combine(App.APPFOLDER, filename));
			}
			PageRefresh();
		}
		protected void OnOpen(Object Sender, EventArgs e)
		{
			Button button = (Button)Sender;
			string filename = (button.CommandParameter as BaseInfo).filename;
			Preferences.Remove("current");
			Preferences.Set("current", Path.Combine(App.APPFOLDER, filename));
			FList.SelectedItem = Items.FindIndex(x => x.filename == filename);
			App.Database.toReboot = true;
			PageRefresh();
			App.TopicsViewModel.GetTopicList();
			App.Database.ResetSelection();
			Navigation.PopAsync();
			//App.MainPage = new NavigationPage(new MainPage());
		}
	}
}
