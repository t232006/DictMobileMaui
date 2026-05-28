

namespace DictMobile.Auxilary
{
    public class SectorComponent: GraphicsView
    {
        private readonly SectorDrawable sector;
        public static readonly BindableProperty AlphaProperty = BindableProperty.Create
            ("alpha",
            typeof(byte),
            typeof(SectorComponent),
            (byte)0,
            propertyChanged: OnChanged
            );
        public byte alpha 
        {
            get => (byte)GetValue(AlphaProperty);
            set => SetValue(AlphaProperty, value); 
        }
        public SectorComponent()
        {
            sector = new SectorDrawable();
            Drawable = sector;
        }
        private static void OnChanged(BindableObject bindable, object OldValue, object NewValue)
        {
            var view = (SectorComponent)bindable;
            view.sector.alphaValue = (byte)NewValue;
            view.Invalidate();
        }

    }
    public class SectorDrawable:IDrawable
    {
        public byte alphaValue { set {
                _alpha = value;
                //Draw();
            } }
        private byte _alpha;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (dirtyRect.Width <= 0 || dirtyRect.Height <= 0)
                return;
            float centerX = dirtyRect.Center.X;
            float centerY = dirtyRect.Center.Y;
            float radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2;

            canvas.FillColor = Colors.Orange;
            canvas.FillCircle(centerX, centerY, radius);

            var path = new PathF();

            path.MoveTo(centerX, centerY);
            path.LineTo(centerX + radius, centerY);
            

            canvas.FillColor = Colors.DarkSeaGreen;
            path.AddArc(
                centerX - radius,
                centerY - radius,
                radius * 2,
                radius * 2,
                0,
                60 * _alpha,
                false);

            path.Close();

            canvas.FillPath(path);
        }
    }
}
