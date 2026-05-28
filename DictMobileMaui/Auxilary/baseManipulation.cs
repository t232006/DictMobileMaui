using SQLite;
using IndDictionary.addition;
using DictMobile.addition;
using System.Collections.ObjectModel;
using System.Collections;

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
			itemsD = database.Table<dict>().ToList();
			itemsT = database.Table<topic>().ToList();
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
                    // обновляем кэш
                    itemsD = database.Table<dict>().ToList();
                    return item.Number;
                }
                else
                {
                    item.DateRec = datesCorrection.toCorrectDate(DateTime.Today.ToString());
                    int id = database.Insert(item);
                    // обновляем кэш
                    itemsD = database.Table<dict>().ToList();
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
                    itemsD = database.Table<dict>().ToList();
                    return item.Number;
				}
				else
				{
					item.DateRec = datesCorrection.toCorrectDate(DateTime.Today.ToString());
					if (IsItPhrase.isItPhrase(item.Word)) item.Phrase = true; else item.Phrase = false;
                    int id = database.Insert(item);
					itemsD = database.Table<dict>().ToList();
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
            itemsT = database.Table<topic>().ToList();
			return result;


        }
		public int deleteRecD(int id)
		{
            int res = database.Delete<dict>(id);
            // обновляем кэш
            itemsD = database.Table<dict>().ToList();
            return res;
        }
		public int deleteRecT(string Name)
		{
            int id = itemsT.First(s => s.Name == Name).id;
            int res = database.Delete<topic>(id);
            // обновляем кэш
            itemsT = database.Table<topic>().ToList();
            return res;
        }
		public ObservableCollection<dict> findRecords(string needle, Func<dict, string> _field)
		{
			IEnumerable<dict> items = from s in itemsD
				   where _field(s).Contains(needle)
				   select s;
			ObservableCollection<dict> result = new ObservableCollection<dict>();
			foreach (var r in items)
				result.Add(r);
			return result;
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
				request = "select distinct DateRec from Dict ";
				if (showAll==false) request += "where Usersel=true ";
				return (IEnumerable<T>)database.Query<dict>(request).OrderBy(t => DateTime.Parse(t.DateRec));
            }
			else
			{
				request = "SELECT DISTINCT Name FROM Topic JOIN Dict ON Topic.ID=Dict.Topic ";
				if (showAll == false) request += "where Usersel=true";
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
            itemsD = database.Table<dict>().ToList();
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
            itemsD = database.Table<dict>().ToList();
        }

    }
}
