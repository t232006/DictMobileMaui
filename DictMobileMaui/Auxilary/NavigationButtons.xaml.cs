

namespace IndDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NavigationButtons : ContentView
	{
		bool translationShow = false;
		public bool TranslationShow { set { translationShow = value; } }
		WordPage WordPage { get; set; }
		public NavigationButtons(WordPage _WordPage)
		{
			WordPage = _WordPage;
			InitializeComponent();
		}
		protected void DictOpen(object sender, EventArgs e)
		{
			//WordPage = new WordPage(translationShow);
			
			Navigation.PopModalAsync();
		}
		protected void ToolsOpen(object sender, EventArgs e)
		{
            // Получаем навигацию основного окна; если недоступна, используем навигацию этого ContentView
            var nav = Application.Current?.MainPage?.Navigation ?? this.Navigation;
            if (nav == null)
                return;

            // Берём верхнюю модальную страницу, если есть, иначе — сам MainPage
            Page? top = null;
            var modalStack = nav.ModalStack;
            if (modalStack != null && modalStack.Count > 0)
                top = modalStack[modalStack.Count - 1];
            else
                top = Application.Current?.MainPage!;

            // Если уже открыт NavigationPage, внутри которого текущая страница — ToolsPage, ничего не делаем
            if (top is NavigationPage topNav && topNav.CurrentPage is ToolsPage)
                return;

            // Если верхняя страница сама по себе — ToolsPage, тоже ничего не делаем
            if (top is ToolsPage)
                return;
            Navigation.PushModalAsync(new NavigationPage(new ToolsPage(WordPage)));
		}
	}
}