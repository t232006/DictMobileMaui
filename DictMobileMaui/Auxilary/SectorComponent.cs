using static DictMobileMaui.Auxilary.IndicatorDrawable;

namespace DictMobileMaui.Auxilary
{
    public abstract class BaseIndicatior: GraphicsView
    { 
        protected IndicatorDrawable Rate;
        public static readonly BindableProperty rateValueProperty = BindableProperty.Create
            ("rateValue",
            typeof(byte),
            typeof(BaseIndicatior),
            (byte)0,
            propertyChanged: OnChanged
            );
        public byte rateValue 
        {
            get => (byte)GetValue(rateValueProperty);
            set => SetValue(rateValueProperty, value); 
        }
        
        private static void OnChanged(BindableObject bindable, object OldValue, object NewValue)
        {
            var view = (BaseIndicatior)bindable;
            view.Rate.rateValue = (byte)NewValue;
            view.Invalidate();
        }
    }
    public class Indicator : BaseIndicatior
    {
        public Indicator(byte _Rate)
        {
            Rate = new IndicatorDrawable(_Rate);
            Drawable = Rate;
        }
    }
    public class GradeIndicator : Indicator 
    {
        public GradeIndicator(byte _Rate):base(_Rate)
        {
            Rate = new GradeDrawable(_Rate);
            Drawable = Rate;
        }
    }
    public class IndicatorDrawable:IDrawable
    {
        private byte totalAmount;
        int space = 5;
        int sqWidth;
        Color RatedColor = Colors.SandyBrown;
        private const byte padding = 5;
        public IndicatorDrawable(byte _totalAmount)
        {
            totalAmount = _totalAmount;
        }
        public byte rateValue { set {
                _rateValue = value;
                //Draw();
            } }
        private byte _rateValue;
        private int selector = 0;
        public virtual void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (dirtyRect.Width <= 0 || dirtyRect.Height <= 0)
                return;
            sqWidth = (int)(dirtyRect.Width / totalAmount) - space;
            for (byte i = 0; i < _rateValue; i++)
            {
                canvas.FillColor = RatedColor;
                canvas.FillRectangle(selector, 0, sqWidth, dirtyRect.Height/3-padding);
                selector = selector + space + sqWidth;    
            }
        }

        public class GradeDrawable : IndicatorDrawable
        {
            Color DefaultColor = Colors.LightGray;
            public GradeDrawable(byte _totalAmount) : base(_totalAmount)
            {
            }
            public override void Draw(ICanvas canvas, RectF dirtyRect)
            {
                base.Draw(canvas, dirtyRect);
                //int selector = 0;    
                for (byte i = _rateValue; i<totalAmount; i++)
                {
                    canvas.FillColor = DefaultColor;
                    canvas.FillRectangle(selector, 0, sqWidth, dirtyRect.Height / 3 - padding);
                    selector = selector + space + sqWidth;
                }
            }
        }
    }

}
