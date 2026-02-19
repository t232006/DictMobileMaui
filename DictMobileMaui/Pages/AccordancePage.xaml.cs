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
        FlexLayout fl1 = new FlexLayout { Direction = FlexDirection.Column };
        FlexLayout fl2 = new FlexLayout { Direction = FlexDirection.Column };
        void DrawNewPage()
        {
            Accordance accordance = new Accordance();
            fl1.Clear();fl2.Clear();MainStack.Clear();
            for (byte i = 0; i < 6; i++)
            {
                fl1.Children.Add(new ShapeComponent(false, accordance.Pool[i].Number));
                fl2.Children.Add(new ShapeComponent(true, accordance.Pool[i].Number));
                
            }
            MainStack.Children.Add(fl1); MainStack.Children.Add(fl2);
        }
        public AccordancePage()
		{
			InitializeComponent();
            
            
            MainStack.Direction = (DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
                 ? FlexDirection.Column
                 : FlexDirection.Row;
            MainStack.JustifyContent = FlexJustify.SpaceEvenly;
            MainStack.AlignItems = FlexAlignItems.Center;
            FlexLayout.SetGrow(fl1, 1); FlexLayout.SetGrow(fl2, 1);
            DrawNewPage();
            
            fl1.ChildRemoved += (s, e) =>
            {
                if (fl1.Children.Count + fl2.Children.Count == 6) NextBut.IsEnabled = true;
            };
            fl2.ChildRemoved += (s, e) =>
            {
                if (fl1.Children.Count + fl2.Children.Count == 6) NextBut.IsEnabled = true;
            };
            
            
        }
        protected void NextBlock(Object sender, EventArgs e) 
        {
            DrawNewPage();
        } 
        
	}

}
