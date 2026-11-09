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
        protected BaseIndicatior(IndicatorDrawable drawable)
        {
            Rate = drawable;
            Drawable = Rate;
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
        public Indicator(byte _Rate):base(new IndicatorDrawable(_Rate))
        {
            HeightRequest = 18;
        }
    }
    public class GradeIndicator : BaseIndicatior 
    {
        public GradeIndicator(byte _Rate) : base(new GradeDrawable(_Rate))
        {
            HeightRequest = 18;
        }
    }
    public class IndicatorDrawable:IDrawable
    {
        protected byte totalAmount;
        int space = 05;
        int sqWidth;
        protected Color RatedColor; //= Colors.SandyBrown;
        protected byte padding;
        public IndicatorDrawable(byte _totalAmount)
        {
            totalAmount = _totalAmount;
            padding = 11;
            RatedColor = Colors.LightCoral;
        }
        public byte rateValue { set {
                _rateValue = value;
                //Draw();
            } }
        protected byte _rateValue;
        protected int selector = 0;
        public virtual void Draw(ICanvas canvas, RectF dirtyRect)
        {
            selector = 0; //to prevent shift in future
            if (dirtyRect.Width <= 0 || dirtyRect.Height <= 0)
                return;
            sqWidth = (int)(dirtyRect.Width / totalAmount) - space;
            for (byte i = 0; i < _rateValue; i++)
            {
                canvas.FillColor = RatedColor;
                canvas.FillRectangle(selector, padding, sqWidth, dirtyRect.Height/3);
                selector = selector + space + sqWidth;    
            }
        }

        public class GradeDrawable : IndicatorDrawable
        {
            Color DefaultColor = Colors.LightGray;
            public GradeDrawable(byte _totalAmount) : base(_totalAmount)
            {
                RatedColor = Colors.SandyBrown;
                padding = 5;
            }
            public override void Draw(ICanvas canvas, RectF dirtyRect)
            {
                base.Draw(canvas, dirtyRect);  
                for (byte i = _rateValue; i<totalAmount; i++)
                {
                    canvas.FillColor = DefaultColor;
                    canvas.FillRectangle(selector, padding, sqWidth, dirtyRect.Height / 3);
                    selector = selector + space + sqWidth;
                }
                
            }
        }
    }

}
