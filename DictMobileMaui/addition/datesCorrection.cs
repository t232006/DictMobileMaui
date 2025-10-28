using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IndDictionary.addition
{
    public static class datesCorrection
    {
		public static string toCorrectDate(string wrongDate)
		{
			DateTime da;    //dates casting
			if (wrongDate.Length>10) wrongDate = wrongDate.Remove(10);
			
			try
				{
				if (wrongDate.Contains("."))
					da = DateTime.Parse(wrongDate, new CultureInfo("ru-RU"));
				else
					da = DateTime.Parse(wrongDate);
				}
			catch { da = DateTime.Now; }
			//finally { }
			return da.ToString("dd.MM.yyyy");
		} 
    }
}
