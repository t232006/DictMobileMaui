using DictMobile;
using DictMobile.games;
using DictMobile.Pages.GamesPages.Accordance;
//using DictMobile.Auxilary;
using Microsoft.Maui.Layouts;
using static DictMobile.Pages.GamesPages.Accordance.ShapeComponent;
using static DictMobile.Pages.GamesPages.BreakWord.MyBorder;



namespace IndDictionary
{	
	public partial class AccordancePage : BaseGamePage
	{
        Accordance accordance;
        FlexLayout MainStack = new()
        {
            BackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark
                    ? (Color)Application.Current.Resources["BackgroundDark"]
                    : (Color)Application.Current.Resources["BackgroundLight"]
        };
        FlexLayout fl1 = new FlexLayout { Direction = FlexDirection.Column };
        FlexLayout fl2 = new FlexLayout { Direction = FlexDirection.Column };
        void DrawNewPage()
        {
            accordance = new Accordance();
            var newFl1 = new FlexLayout { Direction = FlexDirection.Column };
            var newFl2 = new FlexLayout { Direction = FlexDirection.Column };
            FlexLayout.SetGrow(newFl1, 1);
            FlexLayout.SetGrow(newFl2, 1);
            for (byte i = 0; i < 6; i++)
            {
                var newShape1 = new ShapeComponent(false, accordance.PoolWord[i].Word, accordance.PoolWord[i].Translation);
                var newShape2 = new ShapeComponent(true, accordance.PoolTrans[i].Word, accordance.PoolTrans[i].Translation);
                newShape1.onAnswer += onAnswerGave;
                newShape2.onAnswer += onAnswerGave;
                newFl1.Children.Add(newShape1);
                newFl2.Children.Add(newShape2);
                newShape1.Shape.LetterTapped += (s,e) => onSelected(newShape1, e);
                newShape2.Shape.LetterTapped += (s,e) => onSelected(newShape2, e);

            }
            MainStack.Children.Clear();
            MainStack.Children.Add(newFl1);
            MainStack.Children.Add(newFl2);
            fl1 = newFl1; fl2 = newFl2;
            fl1.InvalidateMeasure();
            fl2.InvalidateMeasure();
            MainStack.InvalidateMeasure();

        }
        protected void onSelected(object Sender, LetterTappedEventArgs e)
        {
            var fl = (FlexLayout)(Sender as ShapeComponent)!.Parent;
            foreach (var temp in fl.Children.OfType<ShapeComponent>())
            {
                if (temp == Sender) continue;
                if (temp.IsEnabled)
                    temp.Shape.Selected = false;
            }
            var another_fl = fl == fl1 ? fl2 : fl1;
            foreach (var temp in another_fl.Children.OfType<ShapeComponent>())
            {
                if (temp.Shape.Selected)
                {
                    (Sender as ShapeComponent).Shape.Selected = false;
                    temp.Shape.Selected = false;
                    onAnswerGave(Sender, new AnswerEventArgs(temp));
                    break;
                }
            }
        }
        protected void onAnswerGave(object Sender, AnswerEventArgs e)
        {
            
            var target = (ShapeComponent)Sender;
            //if (target.Translaiton==e.Source.Translaiton) target.an
            target.CanDrag = false;
            e.Source.CanDrag = false;
            e.Source.IsEnabled = false;
            target.IsEnabled = false;
            if (target.Translation == e.Source.Translation)
            { 
                target.Answer(1);
                var label = target.Shape.Content;
                label.Text = e.Source.IsWord
                    ? label.Text + "=" + target.Word
                    : label.Text + "=" + target.Translation;
                RemoveSource(e.Source); 
                accordance.GetReward(true, target.Translation);
            }

            else
            {
                target.Answer(0);
                e.Source.Answer(0);
                accordance.GetReward(false, target.Translation);
                SearchRightAnswer(fl1, target.Translation, target, e.Source);
                SearchRightAnswer(fl2, target.Translation, target, e.Source);
                SearchRightAnswer(fl1, e.Source.Translation, target, e.Source);
                SearchRightAnswer(fl2, e.Source.Translation, target, e.Source);
            }

            void RemoveSource(ShapeComponent source)
            {
                if (source.Parent is FlexLayout parent)
                {
                    parent.Children.Remove(source);
                    //parent.InvalidateMeasure();
                    //MainStack.InvalidateMeasure();
                }
            }

            void SearchRightAnswer(FlexLayout fl, string translation, ShapeComponent selectedTarget, ShapeComponent selectedSource)
            {
                foreach (var temp in fl.Children.OfType<ShapeComponent>())
                {
                    if (temp == selectedTarget || temp == selectedSource) continue;
                    if (temp.Translation == translation)
                    {
                        temp.Answer(2); //underlining it
                        temp.CanDrag = false;
                        temp.IsEnabled = false;
                    }
                }
            }
        }
        
        public AccordancePage()
		{
            InitializeComponent();
            SetPageContent(MainStack);

            MainStack.Direction = (DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
                 ? FlexDirection.Row
                 : FlexDirection.Column;
            MainStack.JustifyContent = FlexJustify.SpaceEvenly;
            MainStack.AlignItems = FlexAlignItems.Center;
            FlexLayout.SetGrow(fl1, 1); FlexLayout.SetGrow(fl2, 1);
            DrawNewPage();
            
            fl1.ChildRemoved += (s, e) =>
            {
                if (fl1.Children.Count + fl2.Children.Count == 6) NextBut.IsEnabled = true;
            };
            fl2.ChildRemoved += (s, e) =>
            {
                if (fl1.Children.Count + fl2.Children.Count == 6) NextBut.IsEnabled = true;
            };
        }
        protected void NextBlock(Object sender, EventArgs e) 
        {
            DrawNewPage();
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width <= 0 || height <= 0) return;

            MainStack.Direction = (DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
                 ? FlexDirection.Row
                 : FlexDirection.Column;
        }

    }

}
