using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Maui.Controls;

namespace IndDictionary.Converters
{
	class BoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool temp=(!(bool)value);
			return temp;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool temp = (!(bool)value);
			return temp;
		}
	}
	class CheckBoxToString : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string temp=(bool)value? "Редактируется":"Редактировать";
			return temp;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool temp = (string)value=="Редактируется"? true:false;
			return temp;
		}
	}
	class stringToDate : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string s = (string)value;
			if (s != null)
				return DateTime.Parse(s, new CultureInfo("ru-RU"));
			else
				return DateTime.Now;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((DateTime)value).ToString("dd.MM.yyyy");
		}
	}
	class PeakerToBool : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((byte)(value)==0) return false; else return true;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool b = (bool)value;
			if (b)
				return 1;
			else
				return 0;
		}
	}
}
