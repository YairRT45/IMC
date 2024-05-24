using Microsoft.Extensions.Logging;

using IMCApp.DataAccess;
using IMCApp.ViewModels;
using IMCApp.Views;

namespace IMCApp
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
                });

            var dbContext = new PacienteDBContext();
            dbContext.Database.EnsureCreated();
            dbContext.Dispose();

            builder.Services.AddDbContext<PacienteDBContext>();

            builder.Services.AddTransient<PacientePage>();
            builder.Services.AddTransient<DatosPacientePage>();
            builder.Services.AddTransient<PacienteViewModel>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();

            Routing.RegisterRoute(nameof(PacientePage), typeof(PacientePage));
            Routing.RegisterRoute(nameof(DatosPacientePage), typeof(DatosPacientePage));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
