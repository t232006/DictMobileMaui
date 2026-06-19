using SQLite;
using IndDictionary.addition;
using DictMobile.addition;
using System.Collections.ObjectModel;
using System.Collections;
//using Android.OS;

namespace IndDictionary
{
	public enum WhatToShow { words, phrases, alltogether}
	public enum WhatToSelect { dates, topics }
	public class baseManipulation
	{
		SQLiteConnection database;
		public bool toReboot = false;
		List<dict> itemsD;
		List<topic> itemsT;
		public baseManipulation(string databasePath)
		{
			database = new SQLiteConnection(databasePath);
			itemsD = database.Table<dict>().Where(d=>d.IsDeleted==false).ToList();
			itemsT = database.Table<topic>().Where(d=>d.IsDeleted==false).ToList();
		}
		public void GetReward(int id, bool increase)
		{
			dict? item = findOneRecord(id);
			if (item != null)
			{
				if (increase)
				{
					if (item.Grade < 6) item.Grade += 1;
				}
				else
				{
					if (item.Grade > 0) item.Grade -= 1;
				}
				saveRecD(item);
			}
		}
		public void dispose()
		{
			database.Dispose();
		}

        public IEnumerable<dict> showTableDict(bool allrec, WhatToShow wts)
        {
            string request = "SELECT * FROM Dict WHERE isDeleted=false ";
            if (!allrec) request += "AND usersel=true ";

            switch (wts)
            {
                case WhatToShow.words:
                    request += "AND phrase=false";
                    break;
                case WhatToShow.phrases:
                    request += "AND phrase=true";
                    break;
            }
            return  database.Query<dict>(request);

        }

        public async Task<IEnumerable<dict>> showTableDictAsync(bool allrec, WhatToShow wts)
		{
            string request = "SELECT * FROM Dict WHERE isDeleted=false ";
            if (!allrec) request += "AND usersel=true ";

            switch (wts)
            {
                case WhatToShow.words:
                    request += "AND phrase=false";
                    break;
                case WhatToShow.phrases:
                    request += "AND phrase=true";
                    break;
            }
            return await Task.Run(() => database.Query<dict>(request));
            
        }

		public IEnumerable<topic> showTableTopic()
		{
			return itemsT;
		}
		public int saveRecD(dict item)
		{
			if (item != null)
			{
				if (item.Number != 0)
                {
                    item.DateRec = datesCorrection.toCorrectDate(item.DateRec);
                    database.Update(item);
                    // обновляем кэш
                    itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
                    return item.Number;
                }
                else
                {
                    item.DateRec = datesCorrection.toCorrectDate(DateTime.Today.ToString());
                    int id = database.Insert(item);
                    // обновляем кэш
                    itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
                    return id;
                }

            }
			return -1;
		}
		public int saveRecD(dict item, string topic)
		{
			if (item != null)
			{
				int TopicID = database.Table<topic>().Where(t => t.Name == topic).Select(t => t.id).FirstOrDefault();
				item.Topic = TopicID;
				
				if (item.Number != 0)
				{
					item.DateRec = datesCorrection.toCorrectDate(item.DateRec);
					database.Update(item);
					if (IsItPhrase.isItPhrase(item.Word)) item.Phrase = true; else item.Phrase = false;
                    itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
                    return item.Number;
				}
				else
				{
					item.DateRec = datesCorrection.toCorrectDate(DateTime.Today.ToString());
					if (IsItPhrase.isItPhrase(item.Word)) item.Phrase = true; else item.Phrase = false;
                    int id = database.Insert(item);
					itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
                    return id;
				}

			}
			return -1;
		}
		public int saveRecT(topic item)
		{
			int result;
			if (item.id != 0)
			{
				database.Update(item);
				result = item.id;
			}
			else
                result = database.Insert(item);
            itemsT = database.Table<topic>().Where(d => d.IsDeleted == false).ToList();
            return result;


        }
		public void deleteRecD(int id)
		{
			//int res = database.Delete<dict>(id);
			dict? temp = findOneRecord(id);
			temp.IsDeleted = true;
			temp.DateRec = DateTime.Now.ToString();
            // обновляем кэш
            itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
        }
		public void deleteRecT(string Name)
		{
			//int res = database.Delete<topic>(id);
			topic? temp = itemsT.First(s => s.Name == Name);
			temp.IsDeleted = true;
			temp.DateRec = DateTime.Now.ToString();

            // обновляем кэш
            itemsT = database.Table<topic>().Where(d => d.IsDeleted == false).ToList();
        }
		public async Task<ObservableCollection<dict>> findRecordsAsync(string needle, Func<dict, string> _field)
		{
			var items = await Task.Run(() =>
				itemsD.Where(f => _field(f).Contains(needle)).Select(s => s).ToList()
			);
			return new ObservableCollection<dict>(items);
		}
		public dict? findOneRecord(int id)
		{
			if (id != 0)
				return database.Get<dict>(id);
			else return null;
		}

		public string getInfo(byte WhatExectly) //1-date, 2-count
		{
			string result = "";
			switch (WhatExectly)
			{
				case 1:
					{
						result = showTableDict(true, WhatToShow.alltogether).Count().ToString();
						break;
					}
				case 2:
					{
						result = showTableDict(true, WhatToShow.alltogether).Max(p => p.DateRec)!.ToString();
						break;
					}
			}
			
			return result;

		}

		public void doSelectedToTopic(string Topic)
		{
			string request = $"update dict set topic = (select distinct id from topic where name='{Topic}') where usersel=true ";
			database.Query<dict>(request);
		}
        
		public IEnumerable<dict> getSelected()
		{
			int count = database.Table<dict>().Where(d => d.Usersel == true).Count();
			if (count<6) database.Execute("Update Dict set Usersel=true");
			return database.Table<dict>().Where(d => d.Usersel == true).ToList();
		}
		//------------forms list of dates or topics depending on T----------------------
		public IEnumerable<T> showTopicsDates<T>(bool showAll) where T:new()
		{
			string request;
			if (typeof(T).Equals(typeof(dict)))
			{
				request = "select distinct DateRec from Dict where isDeleted=false";
				if (showAll==false) request += "and Usersel=true ";
				return (IEnumerable<T>)database.Query<dict>(request).OrderBy(t => DateTime.Parse(t.DateRec));
            }
			else
			{
				request = "SELECT DISTINCT Name FROM Topic JOIN Dict ON Topic.ID=Dict.Topic where isDeleted=false";
				if (showAll == false) request += "and Usersel=true";
                return (IEnumerable<T>)database.Query<topic>(request).OrderBy(t => t.id);
            }
		}
		public bool AnySelection(WhatToShow wts)
		{
			return showTableDict(false, wts).Any();
		}
		public void ResetSelection()
		{
			database.Execute("Update Dict set Usersel=false");
			database.Commit();
            itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
        }
        public void ResetRating()
        {
            database.Execute("Update Dict set Score=0");
            database.Commit();
        }

        //------------------selects records from dict which are selected by user on form 
        public void selectDatesOrTopics(IEnumerable<DateOrTopicClassAux> datesList, WhatToSelect wtsel)
		{
			IEnumerable<string> l = from sl in datesList
									where sl.Spoted == true
									select sl.DaOrTo;
			string collect = string.Join("','", l);
			collect = "'" + collect +"'";
			string requestString = wtsel == WhatToSelect.dates ?
				"Update Dict set Usersel=true where daterec in (" + collect + ")" :
				"UPDATE Dict SET Usersel=true WHERE Topic in (SELECT id FROM Topic WHERE Name in (" + collect + "))";
			ResetSelection();
			database.Execute(requestString);
			database.Commit();
            itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
        }

    }
}
