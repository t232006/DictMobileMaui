using System;
using Microsoft.Maui.Controls;
using System.IO;
using Microsoft.Maui.Controls.Xaml;
using System.Reflection;
using IndDictionary.addition;
using Microsoft.Maui.Controls.Shapes;
using Path = System.IO.Path;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace IndDictionary
{
	public partial class App : Application
	{
		public static string databasename;//!!reset after development
		public const string DEFAULTDATABASENAME = "dictionaryCut.db";	//only for default!
		public static string APPFOLDER = FileSystem.AppDataDirectory;
		static baseManipulation database;
		private static void SetDatabasename()
		{
				if (Preferences.ContainsKey("current"))
					databasename = Preferences.Get("current", "");
				else
				{
					databasename = Path.Combine(APPFOLDER, DEFAULTDATABASENAME); 
					Preferences.Set("current", databasename);
				}
		}
		
		private static void LoadDBFirstTime(string dbPath)
		{
			if (database != null) database.dispose();
			database = new baseManipulation(dbPath);
			//for first open to write information about database
			if (Preferences.ContainsKey(databasename))
			{
				string baseInfo = $"{database.getInfo(1)}.{database.getInfo(2)}";
				Preferences.Set(databasename, baseInfo);
			}


			foreach (dict d in database.showTableDict(true, WhatToShow.alltogether))
			{
				d.DateRec = datesCorrection.toCorrectDate(d.DateRec);
				App.Database.saveRecD(d);
			}
		}
		public static baseManipulation Database
		{
			get
			{
				if ((database == null) || (database.toReboot))
				{
					if (database!=null) SetDatabasename();
					string dbPath = databasename;
					if (!File.Exists(dbPath))
					{
						using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream($"DictMobileMaui.Resources.Raw.{DEFAULTDATABASENAME}"))
						{
							using (FileStream dest = new FileStream(dbPath, FileMode.OpenOrCreate))
							{
								s.CopyTo(dest);
								dest.Flush();
							}
						}
					}
					LoadDBFirstTime(dbPath);
					//database = new baseManipulation(dbPath);
					//RefreshAllForms();
				}
				return database;
			}
		}

		public App()
		{
			InitializeComponent();
			SetDatabasename();
			//MainPage = new NavigationPage(new WordPage(false));
			MainPage = new Flyout_Page();

		}

		protected override void OnStart()
		{
			
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
