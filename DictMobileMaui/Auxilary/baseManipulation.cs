using SQLite;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using IndDictionary.addition;

namespace IndDictionary
{
	public enum WhatToShow { words, phrases, alltogether}
	public enum WhatToSelect { dates, topics }
	public class baseManipulation
	{
		SQLiteConnection database;
		public bool toReboot = false;
		IEnumerable<dict> itemsD;
		IEnumerable<topic> itemsT;
		public baseManipulation(string databasePath)
		{
			database = new SQLiteConnection(databasePath);
			itemsD = database.Table<dict>().ToList();
			itemsT = database.Table<topic>().ToList();
		}

		public IEnumerable<dict> showTableDict(bool allrec, WhatToShow wts)
		{
			return filtr(allrec, wts);
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
					return item.Number;
				}
				else
				{
					item.DateRec = datesCorrection.toCorrectDate(DateTime.Today.ToString());
					return database.Insert(item);
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
					return item.Number;
				}
				else
				{
					item.DateRec = datesCorrection.toCorrectDate(DateTime.Today.ToString());
					return database.Insert(item);
				}

			}
			return -1;
		}
		public int saveRecT(topic item)
		{
			if (item.id != 0)
			{
				database.Update(item);
				return item.id;
			}
			else
				return database.Insert(item);
		}
		public int deleteRecD(int id)
		{
			return database.Delete<dict>(id);
		}
		public int deleteRecT(int id)
		{
			return database.Delete<topic>(id);
		}
		public IEnumerable<dict> findRecords(string needle, Func<dict, string> _field)
		{
			return from s in itemsD
				   where _field(s).Contains(needle)
				   select s;
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
						result = showTableDict(true, WhatToShow.alltogether).Select(p => p.DateRec).Max().ToString();
						break;
					}
			}
			
			return result;

		}
		IEnumerable<dict> filtr(bool allrec, WhatToShow _wts)
		{
			string request = "SELECT * FROM Dict ";
			if (!allrec) request += "WHERE usersel=true ";

			switch (_wts)
			{
				case WhatToShow.words:
					request += allrec ? "WHERE phrase=false" : "AND phrase=false";
					break;
				case WhatToShow.phrases:
					request += allrec ? "WHERE phrase=true" : "AND phrase=true";
					break;
			}
			return database.Query<dict>(request);	
		}
		//------------forms list of dates or topics depending on T----------------------
		public IEnumerable<T> showTopicsDates<T>(bool showAll) where T:new()
		{
			string request;
			if (typeof(T).Equals(typeof(dict)))
			{
				request = "select distinct DateRec from Dict ";
				if (showAll==false) request += "where Usersel=true ";
				request += "order by DateRec";
			}
			else
			{
				request = "SELECT DISTINCT Name FROM Topic JOIN Dict ON Topic.ID=Dict.Topic ";
				if (showAll == false) request += "where Usersel=true";
			}
			return database.Query<T>(request);
		}
		public void ResetSelection()
		{
			database.Execute("Update Dict set Usersel=false");
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
		}

    }
}
