using DictMobile.httpMethods;
using DictMobile.models;
using DictMobile.Pages;
using DictMobile.ViewModels;
using IndDictionary.addition;
using System.Diagnostics;
using System.Reflection;
using Path = System.IO.Path;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace IndDictionary
{
    public struct httpResponce
    {
        public bool success;
        public int? topicCount;
        public int? dictCount;
        public string? message;
        public httpResponce()
        {
            success = false; topicCount = null; dictCount = null; message = "";
        }

    }
    public partial class App : Application
    {
        public static string databasename;//!!reset after development
        public const string DEFAULTDATABASENAME = "dictionary_empty.db";    //only for default!
        public static string APPFOLDER = FileSystem.AppDataDirectory;
        public static readonly string SECRETFILE = DeviceInfo.Platform == DevicePlatform.Android ?
                                                        "client_secret_mobile.json" :
                                                        "client_secret.json";

        static baseManipulation database;
        private static ListTopicsViewModel topicsViewModel;
        private static WordsViewModel wordsViewModel;

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
                    if (database != null) SetDatabasename();
                    string dbPath = databasename;
                    CopyFilesFromResource(dbPath, DEFAULTDATABASENAME);
                    LoadDBFirstTime(dbPath);

                }
                return database!;
            }
        }
        public static async Task<httpResponce> PostAsync()
        {
            HttpMethods httpMet = new HttpMethods();
            httpResponce result = new(); httpResponce res = new();
            try
            {
                res = await httpMet.PostTopicAsync(database.ListTopicToPost());
                Debug.WriteLine($"====> Topics {result.dictCount} Time: {database.LastPostUpdate}");
                if (!res.success) throw new Exception(res.message);
                result.topicCount = res.topicCount;


                res = await httpMet.PostDictAsync(database.ListDictToPost());
                Debug.WriteLine($"====> Records {result.dictCount} Time: {database.LastPostUpdate}");
                if (!res.success) throw new Exception(res.message);
                result.dictCount = res.dictCount;

                result.success = true;

                database.LastPostUpdate = DateTime.Now;
                Preferences.Set("LastPostUpdateTime", DateTime.Now);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("error ====>", ex.Message);
                return result;
            }
        }
        public static async Task<httpResponce> GetAsync()
        {
            HttpMethods httpMet = new HttpMethods();
            httpResponce result = new();
            try
            {
                uint? num = database.showTableTopic().First().DBID;
                List<topic>? t = await httpMet.GetListAsync<topic>(num, datesCorrection.toCorrectDate(database.LastGetUpdate.ToString()));
                if (t != null)
                {
                    database.WriteTopicFromServer(t);
                    result.topicCount = t.Count;
                    Debug.WriteLine($"---->Received topics {t.Count} Time: {database.LastGetUpdate}");
                }

                else return result;


                List<dict>? d = await httpMet.GetDictAsync(num, datesCorrection.toCorrectDate(database.LastGetUpdate.ToString()));
                if (d != null)
                {
                    database.WriteDictFromServer(d);
                    result.dictCount = d.Count;
                    Debug.WriteLine($"------> Received words {t.Count} Time: {database.LastGetUpdate}");
                }

                else return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error------> {ex}");
                if (ex.Message.Contains("UNIQUE"))
                {
                    Debug.WriteLine("......> UNIQUE Exception");
                    result.success = true;
                    return result; //it is not error
                }
                return result;
            }
            database.LastGetUpdate = DateTime.Now;
            Preferences.Set("LastGetUpdateTime", DateTime.Now);
            result.success = true;
            return result;
        }
        private async Task StartResume()
        {
            await PostAsync();                //send data if was unable do it last time (didn't have internet connection)
            await GetAsync();

        }
        public App()
        {
            InitializeComponent();
            SetDatabasename();

            Database.LastGetUpdate = Preferences.Get("LastGetUpdateTime", DateTime.Now);
            Database.LastPostUpdate = Preferences.Get("LastPostUpdateTime", DateTime.Now);
            // Database.LastPostUpdate = DateTime.Parse("2026-08-04 15:10:48");
            //httpMet = new HttpMethods();




            //MainPage = new NavigationPage(new WordPage(false));
#pragma warning disable CS0618 // Тип или член устарел
            MainPage = new LoadingPage();
#pragma warning restore CS0618 // Тип или член устарел

        }

        protected override async void OnStart()
        {
            try
            {
                await StartResume();          // ждём завершения синхронизации
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"StartResume error: {ex}");
            }
            finally
            {
                // Только после синхронизации показываем основную страницу
                MainPage = new Flyout_Page();
            }
        }
        protected override void OnSleep()
        {
            Task.Run(async () =>
            {
                await GetAsync();
                await PostAsync();

            });
        }
        protected override void OnResume()
        {
            //Task.Run(async () => await StartResume());
        }
    }
}
