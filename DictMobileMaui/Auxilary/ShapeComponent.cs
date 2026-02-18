
namespace DictMobileMaui.Auxilary
{
    public class ShapeComponent:ContentView
    {
        public ShapeComponent(bool _isWord, string _Word, string _Translation)
        {
            string curStyle = _isWord ? "var41" : "var3Round";
            string curTextStyle = _isWord ? "var4Text" : "var3Text";
            if (Application.Current.Resources.TryGetValue(curStyle, out var style))
                Shape.Style = (Style)style;
            if (Application.Current.Resources.TryGetValue(curTextStyle, out var textstyle))
                (Shape.Content as Label).Style = (Style)textstyle;
            isWord = _isWord;
            Translation = _Translation;
            Word = _Word;
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
                    if ((val as ShapeComponent).isWord != this.isWord) drop.AllowDrop = true;
                }
            };

            drop.Drop += (s, e) =>
            {
                if (e.Data.Properties.TryGetValue("object", out object val))
                { 
                    var label = Shape.Content as Label;
                    label.Text = label.Text + (val as ShapeComponent).translation;
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
