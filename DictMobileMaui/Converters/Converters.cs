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
	class BoolToBorderStyle: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string temp = (bool)value ? "CardStyle1" : "CardStyle2";
			return App.Current.Resources[temp] as Style;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	class BoolToBorderLabelStyle : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string temp = (bool)value ? "CardTextStyle1" : "CardTextStyle2";
			return App.Current.Resources[temp] as Style;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	class BoolToBorderText : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if ((values.Length != 2) || !(values[0] is bool isWord) || !(values[1] is dict item)) return null; else
				return isWord ? item.Word: item.Translation;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
