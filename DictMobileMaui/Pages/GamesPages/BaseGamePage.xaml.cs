using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndDictionary
{
	public partial class BaseGamePage : ContentPage
	{
		public BaseGamePage()
		{
			InitializeComponent();
		}
        protected void SetPageContent(View content)
        {
            ContentArea.Content = content;
        }
    } 
}