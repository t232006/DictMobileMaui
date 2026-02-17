using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace IndDictionary
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Flyout_Page : FlyoutPage
    {
        bool Xmode=false;
        byte actPage = 0;
		//public bool Transl { set => transl = value; }
		public Flyout_Page()
        {
            InitializeComponent();
            //FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
            Detail = new NavigationPage(new WordPage(false));
        }
        protected void onSelected(object sender, SelectedItemChangedEventArgs e)
        {
            switch (e.SelectedItemIndex)
            {
                case 0:
                    {
                        Detail = new NavigationPage(new WordPage(Xmode)); break;
                    }
                case 1:
                    {
                        Detail = new NavigationPage(new TestPage(Xmode)); break;
                    }
                case 2:
                    {
                        Detail = new NavigationPage(new Card(Xmode)); break;
                    }
                case 3:
                    {
                        Detail = new NavigationPage(new Accordance(Xmode)); break;
                    }
            }
            actPage = (byte)e.SelectedItemIndex;
			//Detail = new NavigationPage(new WordPage(transl));
			//IsPresented = false;
		}
        protected void onModeChanged(object sender, EventArgs e)
        {
            Xmode = ModePicker.SelectedIndex==0? false: true;
            onSelected(this, new SelectedItemChangedEventArgs(Sections,actPage));
		}

	}
}