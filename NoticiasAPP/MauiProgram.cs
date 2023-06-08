using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NoticiasAPP.Helpers;
using NoticiasAPP.Services;
using NoticiasAPP.Views;

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


            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<LoginService>();
            builder.Services.AddSingleton<NoticiasService>();
            builder.Services.AddSingleton<CategoriaService>();

            //WORKAROUND
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<NoticiasView>();
            builder.Services.AddSingleton<NoticiaView>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}