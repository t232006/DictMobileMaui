using IndDictionary;

namespace DictMobile.Pages;

public partial class AfterYes_NoPage : ContentPage
{
	public AfterYes_NoPage(short result, short yourrecord)
	{
		InitializeComponent();
		ResultLabel.Text = result.ToString();
		RecordLabel.Text = yourrecord.ToString();
	}
	protected void onRetryPress(object sender, EventArgs e)
	{
        var flyout = Application.Current.MainPage as Flyout_Page;
        if (flyout != null)
            flyout.Detail = new NavigationPage(new Yes_No_Page());
		Navigation.PopAsync();
    }
	protected void onBackPress(object sender, EventArgs e)
	{
		var flyout = Application.Current.MainPage as Flyout_Page;
		if (flyout != null)
		{
			flyout.Detail = new NavigationPage(new WordPage(flyout.Xmode));
			flyout.Sections.SelectedItem = 0;
		}
			
		Navigation.PopAsync();
	}
	
        
    
}