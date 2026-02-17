
namespace DictMobileMaui.Auxilary
{
    public class ShapeComponent:ContentView
    {
        public ShapeComponent(bool _isWord, string _Word, string _Translation)
        {
            if (Application.Current.Resources.TryGetValue("CardStyle1", out var style))
                Shape.Style = (Style)style;
            isWord = _isWord;
            translation = _Translation;
            word = _Word;
            Content = Shape;
            if (Shape.Content is Label label)
            {
                label.Text = isWord ? word : translation;
                label.HorizontalOptions = LayoutOptions.Center;
                label.VerticalOptions = LayoutOptions.Center;
            }

            Shape.HorizontalOptions = LayoutOptions.Start;
            Shape.VerticalOptions = LayoutOptions.Start;

        }
        public Border Shape = new Border
        {
            Content = new Label(),
            WidthRequest=50,
            HeightRequest=70
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
                }
            }
            get => translation;
        }
    }
}
