using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace IndDictionary.addition
{
	[ContentProperty("Source")]
	public class ImageResourceExt : IMarkupExtension
	{
		public string Source { get; set; }

		public object? ProvideValue(IServiceProvider serviceProvider)
		{
			if (Source == null)
			{
				return null;
			}
			var imageSource = ImageSource.FromResource(Source);

			return imageSource;
		}
	}
}
