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
        //bool transl=false;
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
                        Detail = new NavigationPage(new WordPage(false)); break;
                    }
                case 1:
                    {
                        Detail = new NavigationPage(new WordPage(true)); break;
                    }
                case 2:
                    {
                        Detail = new NavigationPage(new TestPage(false)); break;
                    }
                case 3:
                    {
                        Detail = new NavigationPage(new TestPage(true)); break;
                    }
                case 4:
                    {
                        Detail = new NavigationPage(new Card()); break;
                    }
            }
			//Detail = new NavigationPage(new WordPage(transl));
            //IsPresented = false;
		}

    }
}