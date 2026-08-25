using Microsoft.Extensions.Logging;

namespace IndDictionary
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("WINGDING.TTF", "Wingdings");
                    fonts.AddFont("WINGDING2.ttf", "Wingdings2");
                    fonts.AddFont("WINGDING3.ttf", "Wingdings3");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
