using IndDictionary.addition;
using IndDictionary.Pages;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ToolsPage : ContentPage
	{
		bool showAll = true;
		WhatToShow wts = WhatToShow.alltogether;

		WordPage detail;

		public ToolsPage(WordPage Detail)
		{
			InitializeComponent();
			//if (Detail.transl) Title = "Translation"; else Title = "Word";
			detail = Detail;
			
			NavigationButtons navButtons = new NavigationButtons(Detail);
			forNavButtons.Children.Add(navButtons);
		}
		protected override void OnDisappearing()
		{
			detail.PassParams(showAll, wts);
			base.OnDisappearing();
		}

		protected override void OnAppearing()
		{
			DateLabel.Text = "Last record: " + App.Database.getInfo(2);
			CountLabel.Text = "Records count: " + App.Database.getInfo(1);
			base.OnAppearing();
		}
		protected void OnAll(object sender, EventArgs e)
		{
			wts = WhatToShow.alltogether;
			//detail.Refresh(showAll, wts);
		}
		protected void OnPhrases(object sender, EventArgs e)
		{
			wts = WhatToShow.phrases;
			//detail.Refresh(showAll, wts);
		}
		protected void OnWords(object sender, EventArgs e)
		{
			wts = WhatToShow.words;
			//detail.Refresh(showAll, wts);
		}
		protected void onReset(object sender, EventArgs e)
		{
			App.Database.ResetSelection();
			//detail.Refresh(showAll, wts);
		}
		protected void OnChecking(object sender, EventArgs e)
		{
			showAll = !(sender as CheckBox)!.IsChecked;
			//detail.Refresh(showAll, wts);
		}

		async Task<FileResult> PickAndShow(PickOptions options)
		{
			var result = await FilePicker.PickAsync(options);
			if (result != null)
			{
				if (result.FileName.EndsWith("db", StringComparison.OrdinalIgnoreCase))
				{
					try
					{
						using (var readStream = await result.OpenReadAsync())
						{
							string st = Path.Combine(App.APPFOLDER, result.FileName);
							if (File.Exists(st))
							{
								byte adder = 1;
								do
								{
									string? fpath = Path.GetDirectoryName(st);
									string fname = $"{Path.GetFileNameWithoutExtension(st)}_{adder++}.db";
									st = Path.Combine(fpath!, fname);
								} while (File.Exists(st));
							}
								
							using (var fileStream = new FileStream(st, FileMode.Create, FileAccess.Write))
							{
								await readStream.CopyToAsync(fileStream);
							}
							Preferences.Remove("current");
							//App.Current.Properties.Add("current", st);
							Preferences.Set("current", st);
							App.databasename = result.FileName;
							App.Database.toReboot = true;
							App.Database.ResetSelection();	
						}
					}
					catch { }
					;
				}
			}
			return result!;

		}
		protected async void SaveToCloud(object sender, EventArgs e)
		{
			string currentDB = Preferences.Get("current","");
			string DBName = Path.GetFileName(currentDB);
			string? DBPath = Path.GetDirectoryName(currentDB);
			App.Database.dispose();
			await SyncCloud.SaveToCloud("client_secret.json", DBPath!, DBName);
		}
		
		protected async void OnSynchr(object sender, EventArgs e)
		{
			var options = new PickOptions
			{
				FileTypes = new FilePickerFileType
				(new Dictionary<DevicePlatform, IEnumerable<string>>
				{
					{ DevicePlatform.Android, new[] { "*/*"} },
					{ DevicePlatform.WinUI, new[] { ".db" } }
				}),
				PickerTitle = "Please, select database file"
			};
			await PickAndShow(options);

		}
		protected async void OpenLibrary(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new FilesList());
		}
		protected async void onDates(object sender, EventArgs e)
		{
			List<DateOrTopicClassAux> conteiner = new List<DateOrTopicClassAux>();
			IEnumerable<dict> tempcont = App.Database.showTopicsDates<dict>(!ShowSelected.IsChecked);
			foreach (dict t in tempcont)
			{
				conteiner.Add(new DateOrTopicClassAux { DaOrTo = t.DateRec, Spoted = false });
			}
			DateTopicForm dateForm = new DateTopicForm(conteiner, WhatToSelect.dates);
			await Navigation.PushAsync(dateForm);
		}

		protected async void onTopics(object sender, EventArgs e)
		{
			List<DateOrTopicClassAux> conteiner = new List<DateOrTopicClassAux>();
			IEnumerable<topic> tempcont = App.Database.showTopicsDates<topic>(!ShowSelected.IsChecked);
			foreach (topic t in tempcont)
			{
				conteiner.Add(new DateOrTopicClassAux { DaOrTo = t.Name, Spoted = false });
			}
			DateTopicForm topicForm = new DateTopicForm(conteiner, WhatToSelect.topics);
			await Navigation.PushAsync(topicForm);
		}
		
	}
}
