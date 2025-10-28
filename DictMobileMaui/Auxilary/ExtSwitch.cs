using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Controls;

namespace IndDictionary
{
    public class ExtSwitch : Switch
    {
		public static readonly BindableProperty IDProperty =
			BindableProperty.Create("ID",
				typeof(int),
				typeof(ExtSwitch),
				0,
				BindingMode.OneWay);
		public int ID
		{
			get => (int)GetValue(IDProperty);
			set { SetValue(IDProperty, value); }
		}
		/*
		public static readonly BindableProperty IndexProperty =
			BindableProperty.Create("Index",
				typeof(int),
				typeof(ExtSwitch),
				0,
				BindingMode.OneWay);
		public int Index
		{
			get => (int)GetValue(IndexProperty); 
			set { SetValue(IndexProperty, value); }
		}*/
    }
}
