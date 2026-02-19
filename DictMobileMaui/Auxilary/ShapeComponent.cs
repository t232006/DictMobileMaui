
using IndDictionary;

namespace DictMobileMaui.Auxilary
{
    public class ShapeComponent:ContentView
    {
        void Answer(bool _true)
        {
           
            string curStyle = _true ? "var2" : "var21";
            string curTextStyle = _true ? "var2Text" : "var1TextStrike";
            if (Application.Current.Resources.TryGetValue(curStyle, out var style))
                Shape.Style = (Style)style;
            if (Application.Current.Resources.TryGetValue(curTextStyle, out var textstyle))
                (Shape.Content as Label).Style = (Style)textstyle;
            App.Database.GetReward(id, _true);
        }
        public ShapeComponent(bool _isWord, int _id)
        {
            string curStyle = _isWord ? "var41" : "var3Round";
            string curTextStyle = _isWord ? "var4Text" : "var3Text";
            if (Application.Current.Resources.TryGetValue(curStyle, out var style))
                Shape.Style = (Style)style;
            if (Application.Current.Resources.TryGetValue(curTextStyle, out var textstyle))
                (Shape.Content as Label).Style = (Style)textstyle;
            isWord = _isWord;
            id = _id;
            Translation = App.Database.findOneRecord(_id)!.Translation;
            Word = App.Database.findOneRecord(_id)!.Word;
            Content = Shape;
            if (Shape.Content is Label label)
            {
                //label.Text = isWord ? word : translation;
                label.HorizontalOptions = LayoutOptions.Center;
                label.VerticalOptions = LayoutOptions.Center;
            }
            var drag = new DragGestureRecognizer
            {
                CanDrag = true,
            };
            drag.DragStarting +=(s, e) =>
            {
                e.Data.Properties.Add("object", this);
            };
            GestureRecognizers.Add(drag);
            var drop = new DropGestureRecognizer();

            drop.DragOver += (s, e) =>
            {
                if (e.Data.Properties.TryGetValue("object", out object val))//не бросать в свои
                {
                    if ((val as ShapeComponent).isWord != this.isWord) e.AcceptedOperation = DataPackageOperation.Copy;
                    else
                        e.AcceptedOperation = DataPackageOperation.None;
                }
            };

            drop.Drop += (s, e) =>
            {
                if (e.Data.Properties.TryGetValue("object", out object val) && val is ShapeComponent source)
                { 
                    var label = Shape.Content as Label;
                    if (source.Parent is FlexLayout parent)
                    {
                        parent.Children.Remove(source);
                    }
                    label.Text = source.isWord
                        ? label.Text + (val as ShapeComponent).word
                        : label.Text + (val as ShapeComponent).translation;
                    if (source.translation == this.translation) Answer(true); else Answer(false);
                }
            };
            GestureRecognizers.Add(drop);

        }
        public Border Shape = new Border
        {
            Content = new Label(),
            //WidthRequest=70,
            HeightRequest=40
        };
        private readonly bool isWord;
        private string translation;
        private string word;
        private int id;
       

        public string Word
        {
            set
            {
                if (isWord)
                {
                    var label = Shape.Content as Label;
                    label.Text = value;
                    //Shape.WidthRequest = label.Text.Length * 10;
                    Shape.HorizontalOptions = LayoutOptions.Fill;
                }
            }
            get => word;
        }
        public string Translation {
            set
            {
                if (!isWord)
                {
                    var label = Shape.Content as Label;
                    label.Text = value;
                    //Shape.WidthRequest = label.Text.Length * 10;
                    Shape.HorizontalOptions = LayoutOptions.Fill;
                }
                translation = value;
            }
            get => translation;
        }
        

    }
}
