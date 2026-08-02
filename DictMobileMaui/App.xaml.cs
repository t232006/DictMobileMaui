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
		private static WordsViewModel wordsViewModel;
        HttpMethods httpMet;
        public static ListTopicsViewModel TopicsViewModel 
        { 
            get 
            { 
                topicsViewModel ??= new ListTopicsViewModel();
                return topicsViewModel; 
            } 
        }
		public static WordsViewModel WordsViewModel
		{
			get
			{
				wordsViewModel ??= new WordsViewModel(true, WhatToShow.alltogether, true);
				return wordsViewModel;
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
        private async Task<bool> PostAsync()
        {
            try
            {
                var result = await httpMet.PostTopicAsync(database.GetListTopic());
                Debug.WriteLine($"====> Topics {result} Time: {database.LastUpdate}");
                result = await httpMet.PostDictAsync(database.GetListDict());
                Debug.WriteLine($"====> Records {result} Time: {database.LastUpdate}");
                return result.Contains("applied");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("error ====>", ex);
                return false;
            }
        }
        private async Task<bool> GetAsync()
        {
            try
            {
                uint? num = database.showTableTopic().First().DBID;
                List<topic>? t = await httpMet.GetListAsync<topic>(num, datesCorrection.toCorrectDate(database.LastUpdate.ToString()));
                if (t != null)
                {
                    database.WriteTopicFromServer(t);
                    Debug.WriteLine($"---->Received topics {t.Count} Time: {database.LastUpdate}" ); 
                }
                    
                else return false;

                
                List<dict>? d = await httpMet.GetDictAsync(num, datesCorrection.toCorrectDate(database.LastUpdate.ToString()));
                if (d != null)
                {
                    database.WriteDictFromServer(d);
                    Debug.WriteLine($"------> Received words {t.Count} Time: {database.LastUpdate}" );
                }
                      
                else return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error------> {ex}");
                if (ex.Message.Contains("UNIQUE")) 
                {
                    Debug.WriteLine("......> UNIQUE Exception");
                    return true; //it is not error
                }
                return false;
            }
            return true;
        }
        private async Task StartResume()
        {
           // await PostAsync(); //send data if was unable do it last time (didn't have internet connection)
            if (await GetAsync())
                database.LastUpdate = DateTime.Now;
        }
        public App()
		{
			InitializeComponent();
			SetDatabasename();

            Database.LastUpdate = Preferences.Get("LastUpdateTime", DateTime.Now);
            Database.LastUpdate = DateTime.Parse("2026-08-02 19:34:48");
            httpMet = new HttpMethods();
            Task.Run(async() => await StartResume());
            


            //MainPage = new NavigationPage(new WordPage(false));
#pragma warning disable CS0618 // Тип или член устарел
            MainPage = new Flyout_Page();
#pragma warning restore CS0618 // Тип или член устарел

        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            // Вызывается, когда приложение полностью скрылось с экрана (свернуто или закрывается)
            window.Destroying += (s, e) =>
            {
				OnSleep();
            };
            return window;
        }


        protected override void OnStart()
		{
           
        }
		protected override void OnSleep()
		{
            Task.Run(async () =>
            {
                await GetAsync();
                if (await PostAsync())
                {
                    database.LastUpdate = DateTime.Now;
                    Preferences.Set("LastUpdateTime", DateTime.Now);
                }
            }); 
        }
		protected override void OnResume()
		{
            Task.Run(async () => await StartResume());
		}
	}
}
