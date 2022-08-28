using Avalonia.Controls;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Slidecrew_Interfaces;
using Slidecrew.ViewModels;
using Avalonia.Input;

namespace Slidecrew_UI
{

    public partial class MainWindow : Window
    {
        IScreenShare screenShare = null;

        Image _image;

        MainwindowViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            this.SystemDecorations = SystemDecorations.Full;
#if DEBUG
            this.AttachDevTools();
#endif
            this.DataContext = vm = new MainwindowViewModel();
            this.Closed += MainWindow_Closed;

            AddHandler(DragDrop.DropEvent, Drop);

            _image = this.Get<Image>("ScreenShareImage");

            vm.OnImageUpdated += (s, e) => 
            {
            };
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            
        }

        public void OnOpenWindowClick(object sender, RoutedEventArgs args)
        {
            MainWindow mainWindow2 = new MainWindow();
            mainWindow2.Show();
        }

        public void OnChangeStyle(object sender, RoutedEventArgs args)
        {
            if (this.SystemDecorations < SystemDecorations.Full)
                this.SystemDecorations = (SystemDecorations)(int)this.SystemDecorations + 1;
            else
                this.SystemDecorations = SystemDecorations.None;
        }

        public void OnFullScreenToggle(object sender, RoutedEventArgs args)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.FullScreen;
            else if (this.WindowState == WindowState.FullScreen)
                this.WindowState = WindowState.Normal;
        }

        private bool light = false;
        public void OnColorResourceChanged(object sender, RoutedEventArgs args)
        {
            var bla = (ResourceInclude)Application.Current.Resources.MergedDictionaries[0];
            if (!light)
                Application.Current.Resources.MergedDictionaries[0] = new ResourceInclude() { Source = new Uri("avares://Slidecrew_UI/LightBrushes.axaml") };
            else
                Application.Current.Resources.MergedDictionaries[0] = new ResourceInclude() { Source = new Uri("avares://Slidecrew_UI/DarkBrushes.axaml") };

            light = !light;
        }


        void Drop(object? sender, DragEventArgs e)
        {
            if (!e.Data.Contains(DataFormats.FileNames))
                return;

            Console.WriteLine(string.Join(Environment.NewLine, e.Data.GetFileNames() ?? Array.Empty<string>()));
        }
    }
}