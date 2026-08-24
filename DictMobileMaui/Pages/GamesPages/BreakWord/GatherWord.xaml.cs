using DictMobile.games;
using DictMobile.models;
using DictMobile.Pages.GamesPages.BreakWord;
namespace IndDictionary;

public partial class GatherWord : BaseGamePage
{
    public Label WordSpace { get; private set; }
    public Editor EnterSpace { get; private set; }
    public Grid WorkingArea { get; private set; }
    public Button AgainButton { get; private set; }
    private dict theWord;
    private bool manualInput = true;
    BreakWord bw;
	//Dictionary<string, byte> WordsDict = new();
	//Dictionary<char, byte> LettersDict = new();
	private List<LetterBorder> borders;
	private void TextColor()
    {
        if (theWord == null) return;
        if (theWord.Translation == EnterSpace.Text.Trim())
        
            EnterSpace.TextColor = Colors.Green;
        else
            EnterSpace.TextColor = Colors.LightPink;
    }
  
    protected void onTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!manualInput) return;
        if (e.NewTextValue == null) return;

        if (e.NewTextValue.Contains('\r'))
        {
            var cleanText = e.NewTextValue.Replace("\r", "").Replace("\n", "");
            (sender as Editor).Text = cleanText;
            onAgainPress(this, new EventArgs());
            return;
        }

        string hailstack = e.NewTextValue.ToLower();
        foreach (LetterBorder lb in borders)
        {
            string needle = (lb.Shape.Content as Label)!.Text.ToLower();
            if (hailstack.Contains(needle))
            {
                hailstack = hailstack.Remove(hailstack.IndexOf(needle), needle.Length);
                lb.Selected = true;
            }
            else
            {
                lb.Selected = false;
            }
        }
        TextColor();
    }
	private void PrintText(bool toPrint, string Text)
	{
        
        string s = EnterSpace.Text ?? "";
		if (toPrint)
		{
			s += $" {Text}";
		}
		else
		{
			s = s.Remove(s.LastIndexOf(Text), Text.Length+1);
		}
        manualInput = false;
		EnterSpace.Text = s;
        manualInput = true;
        TextColor();
	}

    private void PrintLetter(bool toPrint, string Text)
    {
        
        string s = EnterSpace.Text ?? "";
        if (toPrint)
        {
            s += $"{Text}";
        }
        else
        {
            s = s.Remove(s.LastIndexOf(Text), Text.Length);
        }
        manualInput = false;
        EnterSpace.Text = s;
        manualInput = true;
        TextColor();
    }

    private void Init()
	{
		bw = new();
		borders = new();
		WorkingArea.Clear();
        EnterSpace.Text = "";
        EnterSpace.TextColor = Colors.LightPink;
		theWord = bw.TheWord;
		WordSpace.Text = theWord.Word;
			//WordsDict = bw.Words.Select((word, index) => new { word, index }).ToDictionary(x => x.word, x => (byte)x.index);
		for (byte i=0; i<5; i++)
		for (byte j=0; j<10; j++)
		{
			LetterBorder border;
			int index = i * 10 + j;
			if (theWord.Phrase)
			{
				if ( index > bw.Words.Length-1) return;
				border = new(bw.Words[index]);
				border.LetterTapped += PrintText;
			}
			else
			{
				if (index > bw.Letters.Length - 1) return;
				border = new(bw.Letters[index]);
					border.LetterTapped += PrintLetter;
			}
            
            borders.Add(border);
            WorkingArea.Add(border.Shape, j, i);
        }
		
	}
	public GatherWord()
	{
        //InitializeComponent();
        Title = "GatherWord";

        // Основной Grid: 3 строки, 1 колонка
        var mainGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(0.3, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(0.60, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(0.9, GridUnitType.Star) }
            }
        };

        // ===== Верхняя часть (Row 0) =====
        var topStack = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.Fill
        };

        WordSpace = new Label
        {
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center
        };

        EnterSpace = new Editor
        {
            BackgroundColor = Colors.LightYellow,
            Margin = new Thickness(50),
            AutoSize = EditorAutoSizeOption.TextChanges,
            HorizontalOptions = LayoutOptions.Fill
            
        };
        EnterSpace.TextChanged += onTextChanged;

        topStack.Children.Add(WordSpace);
        topStack.Children.Add(EnterSpace);

        mainGrid.Add(topStack, 0, 0);

        // ===== WorkingArea (Row 1) =====
        WorkingArea = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            },
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star }
            },
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
            Padding = new Thickness(5),
            ColumnSpacing = 2,
            RowSpacing = 2
        };

        mainGrid.Add(WorkingArea, 0, 1);

        // ===== Кнопка Again (Row 2) =====
        AgainButton = new Button
        {
            Text = "Next",
            Margin = new Thickness(10, 0, 10, 10)
        };
        AgainButton.Pressed += onAgainPress;
        mainGrid.Add(AgainButton, 0, 2);
        SetPageContent(mainGrid);
        //Content = mainGrid;
        Init();
    }
    
	protected void onAgainPress(object sender, EventArgs e)
	{
		if (theWord.Translation == EnterSpace.Text.Trim())
            App.Database.GetReward(theWord.id, true);  
        else
            DisplayAlert("Wrong!", $"{theWord.Word} - {theWord.Translation}", "OK");
        Init();
        
	}
}