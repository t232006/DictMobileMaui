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
			BindingContext = App.Database;
            
		}
        protected void SetPageContent(View content)
        {
            ContentArea.Content = content;
        }
		protected async void ShowMistake()
		{
            VisualStateManager.GoToState(ContentArea, "Error");
            VisualStateManager.GoToState(forBar, "ErrorBar");
            await Task.Delay(400);
            VisualStateManager.GoToState(forBar, "NormalBar");
            VisualStateManager.GoToState(ContentArea, "Normal");
        }
    } 
}