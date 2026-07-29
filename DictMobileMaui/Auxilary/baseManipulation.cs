using SQLite;
using IndDictionary.addition;
using DictMobile.addition;
using System.Collections.ObjectModel;
using System.Collections;
using System.Text.Json;
using DictMobile.models;
using DictMobile.httpMethods;
using System.Diagnostics;
//using Android.OS;

namespace IndDictionary
{
	public enum WhatToShow { words, phrases, alltogether }
	public enum WhatToSelect { dates, topics }
	public class baseManipulation
	{
		private uint? FBDID;
		private DateTime _LastUpdate;
		public DateTime LastUpdate { set => _LastUpdate = value; get => _LastUpdate; }
		public uint? BDID { get => FBDID; }

		SQLiteConnection database;
		public bool toReboot = false;
		List<dict> itemsD;
		List<topic> itemsT;

		public baseManipulation(string databasePath)
		{
			database = new SQLiteConnection(databasePath);
			itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
			itemsT = database.Table<topic>().Where(d => d.IsDeleted == false).ToList();
			if (itemsD.Count > 0) FBDID = itemsD.Select(d => d.DBID).First();
			else
			{
				Random r = new Random();
				FBDID = (uint)r.Next(1, 65535);
			}
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
			return database.Query<dict>(request);

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
		private int insert_update(dict item)
		{
			item.Phrase = IsItPhrase.isItPhrase(item.Word) || IsItPhrase.isItPhrase(item.Translation);
			if (item.id != 0)
			{
				database.Update(item);
				// обновляем кэш
				itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
				return item.id;
			}
			else
			{
				item.DateRec = datesCorrection.toCorrectDate(DateTime.Today.ToString());
				item.DBID = FBDID;
				item.IsDeleted = false;
				int id = database.Insert(item);
				// обновляем кэш
				itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
				return id;
			}
		}
		public int saveRecD(dict item)
		{
			if (item == null) return -1;
			return insert_update(item!);
		}
		public int saveRecD(dict item, string topic)
		{
			if (item == null) return -1;

			int TopicID = database.Table<topic>().Where(t => t.Name == topic).Select(t => t.id).FirstOrDefault();
			item.Topic = TopicID;
			return insert_update(item!);

		}
		public int saveRecT(topic item)
		{
			int result;
			item.Modification_Time = datesCorrection.toCorrectDate(DateTime.Now.ToString());
			if (item.id != 0)
			{
				database.Update(item);
				result = item.id;
			}
			else
			{
				item.DBID = FBDID;
				item.IsDeleted = false;
				result = database.Insert(item);
			}
			itemsT = database.Table<topic>().Where(d => d.IsDeleted == false).ToList();
			return result;
		}
		public void deleteRecD(int id)
		{
			//int res = database.Delete<dict>(id);
			dict? temp = findOneRecord(id);
			if (temp == null) return;
			temp.IsDeleted = true;
			temp.Modification_Time = datesCorrection.toCorrectDate(DateTime.Now.ToString());
			database.Update(temp);
			// обновляем кэш
			itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
		}
		public void deleteRecT(string Name)
		{
			//int res = database.Delete<topic>(id);
			topic? temp = itemsT.First(s => s.Name == Name);
			temp.IsDeleted = true;
			temp.Modification_Time = datesCorrection.toCorrectDate(DateTime.Now.ToString());
			database.Update(temp);
			// обновляем кэш
			itemsT = database.Table<topic>().Where(d => d.IsDeleted == false).ToList();
		}
		public async Task<ObservableCollection<dict>> findRecordsAsync(string needle, Func<dict, string> _field)
		{
			var items = await Task.Run(() =>
				itemsD.Where(f => _field(f).Contains(needle,StringComparison.CurrentCultureIgnoreCase)).Select(s => s).ToList()
			);
			return new ObservableCollection<dict>(items);
		}
		public dict? findOneRecord(int id)
		{
			if (id != 0)
				return database.Get<dict>(id);
			else return null;
		}

		public string? getInfo(byte WhatExectly) //1-date, 2-count
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
						try
						{
							result = showTableDict(true, WhatToShow.alltogether).Max(p => p.DateRec).ToString();
						}
						catch
						{
							result = "none";
						}
						break;
					}
			}

			return result;

		}

		public void doSelectedToTopic(string Topic)
		{
			string request = $"update dict set topic = (select distinct id from topic where name='{Topic}') where usersel=true and isDeleted=false";
			database.Query<dict>(request);
		}

		public IEnumerable<dict> getSelected()
		{
			int count = database.Table<dict>().Where(d => d.Usersel == true).Count();
			if (count < 6) database.Execute("Update Dict set Usersel=true where isDeleted=false");
			return database.Table<dict>().Where(d => d.Usersel == true).ToList();
		}
		//------------forms list of dates or topics depending on T----------------------
		public IEnumerable<T> showTopicsDates<T>(bool showAll) where T : new()
		{
			string request;
			if (typeof(T).Equals(typeof(dict)))
			{
				request = "select distinct DateRec from Dict where isDeleted=false";
				if (showAll == false) request += " and Usersel=true ";
				return (IEnumerable<T>)database.Query<dict>(request).OrderBy(t => DateTime.Parse(t.DateRec));
			}
			else
			{
				request = "SELECT DISTINCT Name FROM Topic left JOIN Dict ON Topic.ID=Dict.Topic where topic.isDeleted=false";
				if (showAll == false) request += " and Usersel=true";
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
			collect = "'" + collect + "'";
			string requestString = wtsel == WhatToSelect.dates ?
				"Update Dict set Usersel=true where isDeleted=false and daterec in (" + collect + ")" :
				"UPDATE Dict SET Usersel=true WHERE isDeleted=false and Topic in (SELECT id FROM Topic WHERE Name in (" + collect + "))";
			ResetSelection();
			database.Execute(requestString);
			database.Commit();
			itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();
		}

		public IEnumerable<DictVM> GetListDict()
		{
			string trueDate = datesCorrection.toCorrectDate(_LastUpdate.ToString());
			string query = $"SELECT d.DBID, Word, Translation, Name as TopicName, DateRec, Score, Usersel, Phrase, Relevation, d.IsDeleted, d.Modification_Time " +
				$"from dict d join topic t on d.Topic=t.Id " +
				$"where d.Modification_time>?";

			return database.Query<DictVM>(query, trueDate);
		}

		public IEnumerable<topic> GetListTopic()
		{
			string trueDate = datesCorrection.toCorrectDate(_LastUpdate.ToString());
			string query = $"SELECT DBID, Name, IsDeleted, Modification_Time from topic where Modification_time>?";
			return database.Query<topic>(query, trueDate);

		}
		public void WriteTopicFromServer(List<topic> topic)
		{
			foreach (topic t in topic)
			{
				try
				{
					saveRecT(t);
                    Debug.WriteLine("Topic has written");
                }
				finally { }
				;
			}
			itemsT = database.Table<topic>().Where(d => d.IsDeleted == false).ToList();
		}
		public void WriteDictFromServer(List<dict> dict)
		{
			foreach (dict d in dict)
			{
				try
				{
					if (d.IsDeleted == false)  //new record
					{
						saveRecD(d);
						Debug.WriteLine("Record has written");
					}
					else
					{
						dict dd = itemsD.First(r => r.Word == d.Word && r.Translation == d.Translation);
						dd.IsDeleted = true;
						database.Update(dd);
                        Debug.WriteLine("Record has deleted");
                    }
					
				}
				catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE"))
				{
                    dict dd = itemsD.First(r => r.Word == d.Word && r.Translation == d.Translation); //when record is recovered
					dd.IsDeleted = d.IsDeleted;
					database.Update(dd);
                    Debug.WriteLine("UNIQUE on WriteDictServer"); 
				}
				;
			}
			itemsD = database.Table<dict>().Where(d => d.IsDeleted == false).ToList();

		}
	}
}
