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
			/*DateTime da;    //dates casting
			//if (wrongDate.Length>10) wrongDate = wrongDate.Remove(10);
			
			try
				{
				if (wrongDate.Contains("."))
					da = DateTime.Parse(wrongDate, new CultureInfo("ru-RU"));
				else
					da = DateTime.Parse(wrongDate);
				}
			catch { da = DateTime.Now; }
			//finally { }
			return da.ToString("yyyy-MM-dd");*/
			//if (wrongDate.IndexOf("00:00:00Z") > 0) wrongDate.Remove(10);
			DateTime da = DateTime.Parse(wrongDate);
            return $"{da:u}";
		} 
    }
}
