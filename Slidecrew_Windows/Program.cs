using System;
using Avalonia;
using Slidecrew_Interfaces;
using Slidecrew.ScreenShare.Windows;
using Slidecrew_UI;
namespace Slidecrew_V2
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .AfterSetup((o) => 
                {
                    //DependancyContainer.Screenshare = new ScreenShare();
                    DependancyContainer.Register<ScreenShare>(typeof(IScreenShare));
                })
                .LogToTrace(Avalonia.Logging.LogEventLevel.Debug);
    }
}
