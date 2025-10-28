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
        bool transl=false;
        public bool Transl { set => transl = value; }
        public Flyout_Page()
        {
            InitializeComponent();
            //FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
            Detail = new NavigationPage(new WordPage(transl));
        }
        protected void onSelected(object sender, SelectedItemChangedEventArgs e)
        {
            transl = (e.SelectedItemIndex == 0) ? false : true;
			Detail = new NavigationPage(new WordPage(transl));
            //IsPresented = false;
		}

    }
}