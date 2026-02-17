using DictMobileMaui;
using DictMobileMaui.games;
using IndDictionary.Converters;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using System.ComponentModel;
using DictMobileMaui.Auxilary;


namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	
	public partial class Accordance : ContentPage, INotifyPropertyChanged
	{
        private readonly bool word;

        public Accordance(bool _word)
		{
			InitializeComponent();
			word = _word;
            ShapeComponent sc = new ShapeComponent(word, "Clovo", "Perevod");
            MainStack.Add(sc);
        }
		
       

	}

}
