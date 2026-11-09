
namespace DictMobileMaui.Auxilary
{
    public class RateComponent: GraphicsView
    { 
        private readonly RateDrawable Rate;
        public static readonly BindableProperty rateValueProperty = BindableProperty.Create
            ("rateValue",
            typeof(byte),
            typeof(RateComponent),
            (byte)0,
            propertyChanged: OnChanged
            );
        public byte rateValue 
        {
            get => (byte)GetValue(rateValueProperty);
            set => SetValue(rateValueProperty, value); 
        }
        public RateComponent(byte _Rate)
        {
            Rate = new RateDrawable(_Rate);
            Drawable = Rate;
        }
        private static void OnChanged(BindableObject bindable, object OldValue, object NewValue)
        {
            var view = (RateComponent)bindable;
            view.Rate.rateValue = (byte)NewValue;
            view.Invalidate();
        }

    }
    public class RateDrawable:IDrawable
    {
        private byte totalAmount;
        Color RatedColor = Colors.SandyBrown;
        Color DefaultColor = Colors.LightGray;
        private const byte padding = 5;
        public RateDrawable(byte _totalAmount)
        {
            totalAmount = _totalAmount;
        }
        public byte rateValue { set {
                _rateValue = value;
                //Draw();
            } }
        private byte _rateValue;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            
            
            if (dirtyRect.Width <= 0 || dirtyRect.Height <= 0)
                return;
            int space = 5;
            int sqWidth = (int)(dirtyRect.Width / totalAmount) - space;
            int selector = 0;
            void PrintSquere(byte i , Color color)
            {
                canvas.FillColor = color;
                canvas.FillRectangle(selector, dirtyRect.Height/2, sqWidth, dirtyRect.Height/3-padding);
                selector = selector + space + sqWidth;
            }

            for (byte i = 0; i < _rateValue; i++)
                PrintSquere(i, RatedColor);

            for (byte i = _rateValue; i < totalAmount; i++)
                PrintSquere(i, DefaultColor);              
            
        }
    }
}
