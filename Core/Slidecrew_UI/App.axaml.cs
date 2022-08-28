using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Slidecrew_UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Slidecrew_UI
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();
            OnAuthorizationComplete();
        }

        private void OnAuthorizationComplete()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Exit += Desktop_Exit;

                LoadSystemComponents();

                if (desktop.Args.Length == 0)
                {
                    new MainWindow().Show();
                    return;
                }
            }
        }

        private void Desktop_Exit(object sender, ControlledApplicationLifetimeExitEventArgs e)
        {
        }

        // load everything that is cross-platform here.
        private void LoadSystemComponents()
        {
           
        }
    }
}