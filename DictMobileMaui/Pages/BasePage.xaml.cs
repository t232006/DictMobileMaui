using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BasePage : ContentPage
	{
		bool wordState;
		static VerticalStackLayout frameContent;
		public bool WordState { get => wordState; }
		public BasePage()
		{
			InitializeComponent();
			wordState = SMode.IsToggled;
			frameContent = FrameContent;
		}
	
		public static ControlTemplate GetSelector()
		{
			return (ControlTemplate)Application.Current!.Resources["BasePageTemplate"];
		}
	} 
}