using Microsoft.Extensions.Logging;

namespace NoticiasAPP
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
                    fonts.AddFont("Gotham-Black.otf", "Black");
                    fonts.AddFont("GothamLight.ttf", "Light");
                    fonts.AddFont("GothamMedium.ttf", "Medium");
                    fonts.AddFont("Gotham-Thin.otf", "Thin");
                });


#if DEBUG
		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}