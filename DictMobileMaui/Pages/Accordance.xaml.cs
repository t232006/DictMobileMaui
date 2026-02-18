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
            ShapeComponent sc1 = new ShapeComponent(word, "Clovo", "Perevod");
            ShapeComponent sc2 = new ShapeComponent(true, "Perevod", "Slovo");
            MainStack.Add(sc1);
			MainStack.Add(sc2);
        }
		
       

	}

}
