using DictMobileMaui;
using DictMobileMaui.games;

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
            var newFl1 = new FlexLayout { Direction = FlexDirection.Column };
            var newFl2 = new FlexLayout { Direction = FlexDirection.Column };
            FlexLayout.SetGrow(newFl1, 1);
            FlexLayout.SetGrow(newFl2, 1);
            for (byte i = 0; i < 6; i++)
            {
                newFl1.Children.Add(new ShapeComponent(false, accordance.PoolWord[i].Number));
                newFl2.Children.Add(new ShapeComponent(true, accordance.PoolTrans[i].Number));
            }
            MainStack.Children.Clear();
            MainStack.Children.Add(newFl1);
            MainStack.Children.Add(newFl2);
            fl1 = newFl1; fl2 = newFl2;
            fl1.InvalidateMeasure();
            fl2.InvalidateMeasure();
            MainStack.InvalidateMeasure();

        }
        public AccordancePage()
		{
			InitializeComponent();
            
            
            MainStack.Direction = (DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
                 ? FlexDirection.Row
                 : FlexDirection.Column;
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
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width <= 0 || height <= 0) return;

            MainStack.Direction = (DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
                 ? FlexDirection.Row
                 : FlexDirection.Column;
        }

    }

}
