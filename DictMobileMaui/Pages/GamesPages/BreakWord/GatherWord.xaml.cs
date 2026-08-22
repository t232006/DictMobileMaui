using DictMobile.games;
using DictMobile.models;
using DictMobile.Pages.GamesPages.BreakWord;
namespace IndDictionary;

public partial class GatherWord : ContentPage
{
	BreakWord bw;
	//Dictionary<string, byte> WordsDict = new();
	//Dictionary<char, byte> LettersDict = new();
	private List<LetterBorder> borders;
	protected void onTextChanged (object sender, TextChangedEventArgs e)
	{
		string hailstack = e.NewTextValue.ToLower();
		foreach (LetterBorder lb in borders)
		{
			string needle = (lb.Shape.Content as Label)!.Text.ToLower();
			if (hailstack.Contains(needle))
			{
				//lb.OnLetterTapped();
				hailstack.Remove(hailstack.IndexOf(needle));
				lb.Selected = true;
			}
			else
			{
				lb.Selected = false;
			}		
		}
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
			s = s.Remove(s.LastIndexOf(Text), Text.Length);
		}
		EnterSpace.Text = s;
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
        EnterSpace.Text = s;
    }

    private void Init()
	{
		bw = new();
		borders = new();
		EnterSpace.Text = "";
		WorkingArea.Clear();
		dict theWord = bw.TheWord;
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
        InitializeComponent();
        Init();
	}
	protected void onAgainPress(object sender, EventArgs e)
	{
		Init();
	}
}