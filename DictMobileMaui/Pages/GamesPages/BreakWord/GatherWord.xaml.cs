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
		if (bw.Words.Length > 0)
		{
			string hailstack = e.NewTextValue;
			foreach (LetterBorder lb in borders)
			{
				string needle = (lb.Shape.Content as Label)!.Text;
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
	private void Init()
	{
		bw = new();
		borders = new();
		EnterSpace.Text = "";
		WorkingArea.Clear();
		dict theWord = bw.TheWord;
		WordSpace.Text = theWord.Word;

		if (theWord.Phrase)
		{
			//WordsDict = bw.Words.Select((word, index) => new { word, index }).ToDictionary(x => x.word, x => (byte)x.index);
			for (byte i=0; i<5; i++)
			for (byte j=0; j<10; j++)
				{
					int index = i * 10 + j;
					if ( index > bw.Words.Length-1) return;
                    LetterBorder border = new(bw.Words[index], index, this);
					border.LetterTapped += PrintText;
					borders.Add(border);
					WorkingArea.Add(border.Shape,j,i);
				}
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