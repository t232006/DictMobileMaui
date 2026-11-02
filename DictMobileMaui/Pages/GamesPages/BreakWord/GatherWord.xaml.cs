using DictMobile.games;
using DictMobile.models;
using DictMobile.Pages.GamesPages.BreakWord;
using TextAlignment = Microsoft.Maui.TextAlignment;
using Button = Microsoft.Maui.Controls.Button;
using DictMobile.addition;
using System.Text.RegularExpressions;




#if ANDROID
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.Widget;
#endif
namespace IndDictionary;

public partial class GatherWord : BaseGamePage
{
    public Label WordSpace { get; private set; }
    public Editor EnterSpace { get; private set; }
    public Grid WorkingArea { get; private set; }
    public Microsoft.Maui.Controls.Button AgainButton { get; private set; }
    private dict theWord;
    private bool manualInput = true;
    BreakWord bw;
	//Dictionary<string, byte> WordsDict = new();
	//Dictionary<char, byte> LettersDict = new();
	private List<LetterBorder> borders;
    private int columns; int rows;
	private void TextColor()
    {
        if (theWord == null) return;
        string prelim = theWord.Translation;
        prelim = Regex.Replace(prelim, @"\[.*?\]", ""); //remove text inside '[ ]'
        if (prelim.ManyWordsComparier(EnterSpace.Text.Trim()))

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
    private void ConfigureWorkingArea()
    {
        WorkingArea.Clear();
        WorkingArea.RowDefinitions.Clear();
        WorkingArea.ColumnDefinitions.Clear();
        
        bool isPortrait = DeviceDisplay.MainDisplayInfo.Orientation
                  == DisplayOrientation.Portrait;

        if (isPortrait)
        {
            if (theWord.Phrase)
            {
                columns = 4; rows = 9;
            }
            else
            {
                columns = 9; rows = 5;
            }
        }
        else
        {
            if (theWord.Phrase)
            {
                columns = 7; rows = 5;
            }
            else
            {
                columns = 15; rows = 4;
            }
        }

        for (int i = 0; i < columns; i++)
            WorkingArea.ColumnDefinitions.Add(
                new ColumnDefinition { Width = GridLength.Star });

        for (int i = 0; i < rows; i++)
            WorkingArea.RowDefinitions.Add(
                new RowDefinition { Height = GridLength.Star });

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
            {
                int index = i * columns + j;
                LetterBorder border;
                if (theWord.Phrase)
                {
                    if (index >= bw.Words.Length) break;

                    border = new(bw.Words[index]);
                    border.LetterTapped += PrintText;
                }
                else
                {
                    if (index >= bw.Letters.Length) break;
                    border = new(bw.Letters[index]);
                    border.LetterTapped += PrintLetter;
                }

                borders.Add(border);
                WorkingArea.Add(border.Shape, j, i);
            }
    }

    private void Init()
	{
		bw = new();
		borders = new();
		//WorkingArea.Clear();
        EnterSpace.Text = "";
        EnterSpace.TextColor = Colors.LightPink;
		theWord = bw.TheWord;
		WordSpace.Text = theWord.Word;
        //WordsDict = bw.Words.Select((word, index) => new { word, index }).ToDictionary(x => x.word, x => (byte)x.index);
        ConfigureWorkingArea();
	}
	public GatherWord()
	{
        //InitializeComponent();
        Title = "GatherWord";

        // Основной Grid: 3 строки, 1 колонка
        var mainGrid = new Grid
        {
            BackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark
                    ? (Color)Application.Current.Resources["BackgroundDark"]
                    : (Color)Application.Current.Resources["BackgroundLight"],
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
            
            HorizontalOptions = LayoutOptions.Fill,
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
            Margin = new Thickness(25),
            AutoSize = EditorAutoSizeOption.TextChanges,
            HorizontalOptions = LayoutOptions.Fill
            
        };
        EnterSpace.TextChanged += onTextChanged;
#if ANDROID
        EnterSpace.HandlerChanged += (s, e) =>
            AttachAndroidEditor();
#endif

        topStack.Children.Add(WordSpace);
        topStack.Children.Add(EnterSpace);

        WorkingArea = new Grid();

        mainGrid.Add(topStack, 0, 0);

        // ===== WorkingArea (Row 1) =====

        mainGrid.Add(WorkingArea, 0, 1);

        // ===== Кнопка Again (Row 2) =====
        AgainButton = new Button
        {
            Text = "\u21BB Next",
            Margin = new Thickness(10, 0, 10, 10)
        };
        AgainButton.Pressed += onAgainPress;
        mainGrid.Add(AgainButton, 0, 2);
        SetPageContent(mainGrid);
        //Content = mainGrid;
        Init(); 
        DeviceDisplay.MainDisplayInfoChanged += OnDisplayInfoChanged;
    }
    private void OnDisplayInfoChanged(
    object sender,
    DisplayInfoChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ConfigureWorkingArea();
        });
    }

    protected void onAgainPress(object sender, EventArgs e)
	{
        //if (theWord.Translation == EnterSpace.Text.Trim())
        string prelim = theWord.Translation;
        prelim = Regex.Replace(prelim, @"\[.*?\]", ""); //remove text inside '[ ]'
        if (prelim.ManyWordsComparier(EnterSpace.Text.Trim()))
            App.Database.GetReward(theWord.id, true);  
        else
            DisplayAlert("Wrong!", $"{theWord.Word} - {theWord.Translation}", "OK");
        Init();
        
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        DeviceDisplay.MainDisplayInfoChanged -= OnDisplayInfoChanged;
#if ANDROID
        DetachAndroidEditor();
#endif

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID
        EnterSpace.HandlerChanged += (s, e) =>
            AttachAndroidEditor();
#endif

        DeviceDisplay.MainDisplayInfoChanged -= OnDisplayInfoChanged;
        DeviceDisplay.MainDisplayInfoChanged += OnDisplayInfoChanged;
    }

#if ANDROID

    private AppCompatEditText? nativeEditor;
    private EnterKeyListener? enterKeyListener;

    private class EnterKeyListener : Java.Lang.Object,
        Android.Views.View.IOnKeyListener
    {
        private readonly Action onEnter;

        public EnterKeyListener(Action action)
        {
            onEnter = action;
        }

        public bool OnKey(
            Android.Views.View? v,
            Keycode keyCode,
            KeyEvent? e)
        {
            if (keyCode != Keycode.Enter)
                return false;

            if (e?.Action == KeyEventActions.Up)
                onEnter();

            // Не позволяем Enter добавлять новую строку
            return true;
        }
    }
    private void AttachAndroidEditor()
    {
        DetachAndroidEditor();

        nativeEditor = EnterSpace.Handler?.PlatformView
                       as AppCompatEditText;

        if (nativeEditor == null)
            return;

        // Делаем ввод однострочным
        nativeEditor.SetSingleLine(true);

        // Меняем Enter на Done на экранной клавиатуре
        nativeEditor.ImeOptions = ImeAction.Done;

        // Обрабатываем нажатие Done
        nativeEditor.EditorAction += NativeEditor_EditorAction;

        // Обрабатываем физическую клавишу Enter
        enterKeyListener = new EnterKeyListener(() =>
        {
            onAgainPress(this, EventArgs.Empty);
        });

        nativeEditor.SetOnKeyListener(enterKeyListener);
    }

    private void DetachAndroidEditor()
    {
        if (nativeEditor != null)
        {
            nativeEditor.EditorAction -= NativeEditor_EditorAction;
            nativeEditor.SetOnKeyListener(null);
        }

        enterKeyListener?.Dispose();
        enterKeyListener = null;
        nativeEditor = null;
    }

    private void NativeEditor_EditorAction(
        object? sender,
        TextView.EditorActionEventArgs e)
    {
        if (e.ActionId == ImeAction.Done)
        {
            e.Handled = true;
            onAgainPress(this, EventArgs.Empty);
        }
    }

#endif
}
