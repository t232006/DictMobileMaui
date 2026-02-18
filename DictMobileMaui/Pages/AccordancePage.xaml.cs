using DictMobileMaui;
using DictMobileMaui.games;
using IndDictionary.Converters;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using System.ComponentModel;
using DictMobileMaui.Auxilary;
using Microsoft.Maui.Layouts;



namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	
	public partial class AccordancePage : ContentPage, INotifyPropertyChanged
	{
        

        public AccordancePage()
		{
			InitializeComponent();
            FlexLayout fl1 = new FlexLayout { Direction = FlexDirection.Column };
            FlexLayout fl2 = new FlexLayout { Direction = FlexDirection.Column };
            Accordance accordance = new Accordance();
            MainStack.Direction = (DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
                 ? FlexDirection.Column
                 : FlexDirection.Row;
            MainStack.JustifyContent = FlexJustify.SpaceEvenly;
            MainStack.AlignItems = FlexAlignItems.Center;
            FlexLayout.SetGrow(fl1, 1); FlexLayout.SetGrow(fl2, 1);
            MainStack.Children.Add(fl1); MainStack.Children.Add(fl2);
            ShapeComponent sc11 = new ShapeComponent(false, accordance.Pool[0].Word, accordance.Pool[0].Translation);
            ShapeComponent sc12 = new ShapeComponent(false, accordance.Pool[1].Word, accordance.Pool[1].Translation);
            ShapeComponent sc13 = new ShapeComponent(false, accordance.Pool[2].Word, accordance.Pool[2].Translation);
            ShapeComponent sc14 = new ShapeComponent(false, accordance.Pool[3].Word, accordance.Pool[3].Translation);
            ShapeComponent sc15 = new ShapeComponent(false, accordance.Pool[4].Word, accordance.Pool[4].Translation);
            ShapeComponent sc16 = new ShapeComponent(false, accordance.Pool[5].Word, accordance.Pool[5].Translation);
            ShapeComponent sc21 = new ShapeComponent(true, accordance.Pool[0].Word, accordance.Pool[0].Translation);
            ShapeComponent sc22 = new ShapeComponent(true, accordance.Pool[1].Word, accordance.Pool[1].Translation);
            ShapeComponent sc23 = new ShapeComponent(true, accordance.Pool[2].Word, accordance.Pool[2].Translation);
            ShapeComponent sc24 = new ShapeComponent(true, accordance.Pool[3].Word, accordance.Pool[3].Translation);
            ShapeComponent sc25 = new ShapeComponent(true, accordance.Pool[4].Word, accordance.Pool[4].Translation);
            ShapeComponent sc26 = new ShapeComponent(true, accordance.Pool[5].Word, accordance.Pool[5].Translation);
            fl1.Children.Add(sc11);
			fl1.Children.Add(sc12);
            fl1.Children.Add(sc13);
            fl1.Children.Add(sc14); 
            fl1.Children.Add(sc15);
            fl1.Children.Add(sc16); 
            fl2.Children.Add(sc21);
            fl2.Children.Add(sc22); 
            fl2.Children.Add(sc23);
            fl2.Children.Add(sc24);
            fl2.Children.Add(sc25);
            fl2.Children.Add(sc26);
            
        }

	}

}
