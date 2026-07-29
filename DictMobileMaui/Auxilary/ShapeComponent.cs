
using IndDictionary;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DictMobile.Auxilary
{
    public class ShapeComponent : ContentView
    {
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
            bool put = false;
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
            Content.WidthRequest = MakeShapeSizeX();
            Content.HeightRequest = MakeShapeSizeY();
            if (Shape.Content is Label label)
            {
                //label.Text = isWord ? word : translation;
                label.HorizontalOptions = LayoutOptions.Center;
                label.VerticalOptions = LayoutOptions.Center;
            }

            // уникальный ключ для передачи через DataPackage
            dragKey = System.Guid.NewGuid().ToString();
            // регистрируем текущий компонент в реестре
            DragRegistry[dragKey] = this;

            var drag = new DragGestureRecognizer
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
                        if (source.Parent is FlexLayout parent)
                        {
                            parent.Children.Remove(source);
                        }
                        label.Text = source.isWord
                            ? label.Text + "=" + (source as ShapeComponent).word
                            : label.Text + "=" + (source as ShapeComponent).translation;
                        if (source.translation == this.translation) Answer(true); else Answer(false);

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

        public Border Shape = new Border
        {
            Content = new Label(),

        };
        private readonly bool isWord;
        private string translation;
        private string word;
        private int id;

        // ключ реестра для этого экземпляра
        private readonly string dragKey;


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
        public string Translation
        {
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