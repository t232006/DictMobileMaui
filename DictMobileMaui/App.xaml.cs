using System.Diagnostics;
using System.Reflection;
using DictMobile.httpMethods;
using DictMobile.models;
using DictMobile.ViewModels;
using IndDictionary.addition;
using Path = System.IO.Path;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace IndDictionary
{
	public partial class App : Application
	{
		public static string databasename;//!!reset after development
		public const string DEFAULTDATABASENAME = "dictionary_empty.db";	//only for default!
		public static string APPFOLDER = FileSystem.AppDataDirectory;
		public static readonly string SECRETFILE = DeviceInfo.Platform == DevicePlatform.Android ?
														"client_secret_mobile.json" :
														"client_secret.json";

        static baseManipulation database;
        private static ListTopicsViewModel topicsViewModel;
        HttpMethods httpMet;
        public static ListTopicsViewModel TopicsViewModel 
        { 
            get 
            { 
                topicsViewModel ??= new ListTopicsViewModel();
                return topicsViewModel; 
            } 
        }
        public static double screenWidth => DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
		public static double screenHeight => DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
		private static void SetDatabasename()
		{
			//Preferences.Clear();
            if (Preferences.ContainsKey("current"))
					databasename = Preferences.Get("current", "");
				else
				{
					databasename = Path.Combine(APPFOLDER, DEFAULTDATABASENAME); 
					Preferences.Set("current", databasename);
				}
		}

		public static void CopyFilesFromResource(string filenameFull, string resFile)
		{
            if (!File.Exists(filenameFull))
            {
				//string filename = Path.GetFileName(filenameFull);
				using (Stream? s = Assembly.GetExecutingAssembly().GetManifestResourceStream($"DictMobile.Resources.Raw.{resFile}"))
                {
                    using (FileStream dest = new FileStream(filenameFull, FileMode.OpenOrCreate))
                    {
                        s?.CopyTo(dest);
                        dest.Flush();
                    }
                }
            }
        }
		
		public static void LoadDBFirstTime(string dbPath)
		{
			if (database != null) database.dispose();
			database = new baseManipulation(dbPath);
			
			string baseInfo = $"{database.getInfo(1)}.{database.getInfo(2)}";
			Preferences.Set(databasename, baseInfo);

		}
		public static baseManipulation Database
		{
			get
			{
				if ((database == null) || (database.toReboot))
				{
					if (database!=null) SetDatabasename();
					string dbPath = databasename;
					CopyFilesFromResource(dbPath, DEFAULTDATABASENAME);
					LoadDBFirstTime(dbPath);
				}
				return database!;
			}
		}

		public App()
		{
			InitializeComponent();
			SetDatabasename();
            


            //MainPage = new NavigationPage(new WordPage(false));
#pragma warning disable CS0618 // Тип или член устарел
            MainPage = new Flyout_Page();
#pragma warning restore CS0618 // Тип или член устарел

        }

		protected override void OnStart()
		{
            database.LastUpdate = Preferences.Get("LastUpdateTime", DateTime.Now);
            //database.LastUpdate = DateTime.Parse("2026-07-23 15:10:00");
            httpMet = new HttpMethods();
			Task.Run(async () => 
			{
				try
				{
					List<topic>? t = await httpMet.GetTopicAsync(database.showTableTopic().First().DBID, datesCorrection.toCorrectDate(database.LastUpdate.ToString()));
					if (t!=null)
					database.WriteTopicFromServer(t);
				}
				catch (Exception ex)
				{
                    Debug.WriteLine($"Error {ex}");
                }
				
				database.LastUpdate = DateTime.Now;
				
			});
            
        }

		protected override void OnSleep()
		{
			
			//database.LastUpdate = DateTime.Parse("2026-06-20 17:55:00");

			Task.Run(async () =>
			{
				try
				{
					Debug.WriteLine(httpMet.PostTopicAsync(database.GetListTopic()));
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				};
			});
            Task.Run(async () =>
            {
                try
                {
                    Debug.WriteLine(httpMet.PostDictAsync(database.GetListDict()));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                ;
            });


            //httpMet.PostDictAsync(database.GetListDict()); 
			database.LastUpdate = DateTime.Now;
			Preferences.Set("LastUpdateTime", DateTime.Now);
        }

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
