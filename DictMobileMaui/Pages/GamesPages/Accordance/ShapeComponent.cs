using DictMobile.Pages.GamesPages.BreakWord;
using IndDictionary;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DictMobile.Pages.GamesPages.Accordance
{
    public class ShapeComponent : ContentView
    {
        public MyBorder Shape;
        private readonly bool isWord;
        private string translation;
        public string Translaiton { get => translation; }
        private string word;
        private DragGestureRecognizer drag;
        public bool CanDrag { set => drag.CanDrag =value; }
        public class AnswerEventArgs: EventArgs
        {
            public ShapeComponent Source { get; }
            public AnswerEventArgs(ShapeComponent source) => Source = source;
        }
        public EventHandler<AnswerEventArgs> onAnswer;

        // ключ реестра для этого экземпляра
        private readonly string dragKey;

        // Регистр для сопоставления текстового ключа -> ShapeComponent (чтобы передавать ссылки между платформами)
        private static readonly ConcurrentDictionary<string, ShapeComponent> DragRegistry = new();
        double MakeShapeSizeX()
        {
            var info = DeviceDisplay.Current.MainDisplayInfo;
            //if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                if (DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
                    return info.Width / info.Density * 0.3;
                else
                    return info.Width / info.Density * 0.8;
            }
            //else return info.Width / info.Density * 0.3;


        }
        double MakeShapeSizeY()
        {
            var info = DeviceDisplay.Current.MainDisplayInfo;
           // if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                if (DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
                    return info.Height / info.Density * 0.12;
                else
                    return info.Height / info.Density * 0.07;
            }
            //else return this.Window.Height * 0.15;
                
        }
        public void Answer(byte selector) //0 - false, 1 - true, 2 - right but not selected answer
        {

            //string curStyle = selector==1 ? "var2" : "var21";
            string curStyle; string curTextStyle;
                switch (selector)
                {
                case 1: curStyle = "var2";
                    curTextStyle = "var2Text";
                    break;
                case 0: curStyle = "var21";
                    curTextStyle = "var1TextStrike";
                    break;
                default: curTextStyle = "var1Text";
                    curStyle = "var21";
                    break;
                }
            if (Application.Current.Resources.TryGetValue(curStyle, out var style))
                Shape.Shape.Style = (Style)style;
            if (Application.Current.Resources.TryGetValue(curTextStyle, out var textstyle))
                (Shape.Shape.Content as Label).Style = (Style)textstyle;
        }
        public ShapeComponent(bool _isWord, string _Word, string _Translation)
        {
            bool put = false;
            Shape = _isWord ? new MyBorder(_Word, "var41", "var4Text") : new MyBorder(_Translation, "var3Round", "var3Text");
            isWord = _isWord;
            translation = _Translation;
            word = _Word;
            Content = Shape.Shape;
            Content.WidthRequest = MakeShapeSizeX();
            Content.HeightRequest = MakeShapeSizeY();
            
            Shape.Content.HorizontalOptions = LayoutOptions.Center;
            Shape.Content.VerticalOptions = LayoutOptions.Center;
            

            // уникальный ключ для передачи через DataPackage
            dragKey = System.Guid.NewGuid().ToString();
            // регистрируем текущий компонент в реестре
            DragRegistry[dragKey] = this;

            drag = new DragGestureRecognizer
            {
                CanDrag = true,
            };
            drag.DragStarting += (s, e) =>
            {
                // На Android/Android WebView нельзя хранить произвольный объект в Data.Properties — используем текстовый ключ
                e.Data.Text = dragKey;
            };
            GestureRecognizers.Add(drag);
            var drop = new DropGestureRecognizer();

            drop.DragOver += (s, e) =>
            {
                try
                {
                    // Получаем текстовый ключ синхронно (короткая блокировка допустима для быстрого запроса)
                    var key = e.Data.Text;
                    if (!string.IsNullOrEmpty(key) && DragRegistry.TryGetValue(key, out var val))
                    {
                        if (val.isWord != this.isWord) e.AcceptedOperation = DataPackageOperation.Copy;
                        else
                            e.AcceptedOperation = DataPackageOperation.None;
                    }
                    else
                    {
                        e.AcceptedOperation = DataPackageOperation.None;
                    }
                }
                catch
                {
                    e.AcceptedOperation = DataPackageOperation.None;
                }
            };

            drop.Drop += async (s, e) =>
            {
                try
                {
                    //var key = e.Data.GetTextAsync().GetAwaiter().GetResult();
                    string? key = await e.Data.GetTextAsync();
                    if (!string.IsNullOrEmpty(key) && DragRegistry.TryGetValue(key, out var source))
                    {
                        if (put) return;
                        put = true;
                        var label = Shape.Content as Label;
                        label.Text = source.isWord
                            ? label.Text + "=" + (source as ShapeComponent).word
                            : label.Text + "=" + (source as ShapeComponent).translation;
                        //if (source.translation == this.translation) Answer(true); else Answer(false);
                        onAnswer?.Invoke(this,new AnswerEventArgs(source));
                        // удаляем из реестра после использования
                        DragRegistry.TryRemove(key, out _);
                    }
                }
                catch
                {
                    // игнорируем ошибки чтения данных
                }
            };
            GestureRecognizers.Add(drop);
        }

    }
}