
namespace DictMobileMaui.Auxilary
{
    public class ShapeComponent:ContentView
    {
        public ShapeComponent(bool _isWord, string _Word, string _Translation)
        {
            string curStyle = _isWord ? "CardStyle1" : "CardStyle2";
            if (Application.Current.Resources.TryGetValue(curStyle, out var style))
                Shape.Style = (Style)style;
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

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);

        }
        public Border Shape = new Border
        {
            Content = new Label(),
            //WidthRequest=70,
            HeightRequest=50
        };
        private readonly bool isWord;
        private string translation;
        private string word;
        private double startX;
        private double startY;

        public string Word
        {
            set
            {
                if (isWord)
                {
                    var label = Shape.Content as Label;
                    label.Text = value;
                    Shape.WidthRequest = label.Text.Length * 10;
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
                    Shape.WidthRequest = label.Text.Length * 10;
                }
            }
            get => translation;
        }
        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    // Запоминаем начальную позицию
                    startX = TranslationX;
                    startY = TranslationY;
                    break;

                case GestureStatus.Running:
                    // Двигаем карточку
                    TranslationX = startX + e.TotalX;
                    TranslationY = startY + e.TotalY;
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    // Можно добавить "примагничивание" к сетке, возврат на место и т.д.
                    break;
            }
        }

    }
}
