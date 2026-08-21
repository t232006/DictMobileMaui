using DictMobile.games;
using DictMobile.models;
using DictMobile.Pages.GamesPages.BreakWord;
namespace IndDictionary;

public partial class GatherWord : ContentPage
{
	BreakWord bw;
	//Dictionary<string, byte> WordsDict = new();
	//Dictionary<char, byte> LettersDict = new();
	public delegate void WrittenDel(int num);
	public event WrittenDel onEnterText;
	protected void onTextChanged (object sender, EventArgs e)
	{
		if (bw.Words.Length>0)
			for (int i=0; i<bw.Words.Length; i++)
			{
				if ((sender as Editor)!.Text.Contains(bw.Words[i])) onEnterText?.Invoke(i);
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
                    LetterBorder border = new(bw.Words[index]);
					border.LetterTapped += PrintText;
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