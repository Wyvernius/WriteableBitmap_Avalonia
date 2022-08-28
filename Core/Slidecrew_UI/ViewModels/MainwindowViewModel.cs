using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Slidecrew_Interfaces;
using System.Threading.Tasks;
using SkiaSharp;
using System.IO;
using System.Collections.Concurrent;

namespace Slidecrew.ViewModels
{

    internal class MainwindowViewModel : NotifyPropertyChanged
    {
        private IScreenShare _screenShare;

        Queue<WriteableBitmap> _bitmapQueue;

        public EventHandler OnImageUpdated;

        public MainwindowViewModel()
        {
            StartScreenShare = new DelegateCommand(OnStartScreenshare);
            StopScreenShare = new DelegateCommand(OnStopScreenshare);
            TakeScreenshot = new DelegateCommand(OnTakeScreenshot);
            _screenShare = DependancyContainer.Get<IScreenShare>();
            // new WriteableBitmap(new Avalonia.PixelSize(e.Width, e.Height), new Avalonia.Vector(96, 96), Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul);

            WriteableBitmap bitmap = null;

            // this does not update the ui!!!!
            /*
            _screenShare.NextFrameReady += (s, e) =>
            {
                if (bitmap == null)
                {
                    bitmap = new WriteableBitmap(new Avalonia.PixelSize(e.Width, e.Height), new Avalonia.Vector(96, 96), Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul);
                }

                var lockedBuffer = ImageBrush.Lock();
                _screenShare.GetFrame(lockedBuffer.Address);
                ImageBrush = bitmap;
                lockedBuffer.Dispose();
            };
            
            _screenShare.NextFrameReady += (s, e) =>
            {
                if (bitmap == null)
                {
                    bitmap = new WriteableBitmap(new Avalonia.PixelSize(e.Width, e.Height), new Avalonia.Vector(96, 96), Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul);
                    ImageBrush = bitmap;
                }

                var lockedBuffer = ImageBrush.Lock();
                _screenShare.GetFrame(lockedBuffer.Address);
                //ImageBrush = bitmap;
                lockedBuffer.Dispose();

                OnPropertyChanged(nameof(ImageBrush));
            };

            */

            // this does update the UI
            _screenShare.NextFrameReady += (s, e) =>
            {
                var bitmap = new WriteableBitmap(new Avalonia.PixelSize(e.Width, e.Height), new Avalonia.Vector(96, 96), Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul);

                var lockedBuffer = bitmap.Lock();
                _screenShare.GetFrame(lockedBuffer.Address);
                ImageBrush = bitmap;
                lockedBuffer.Dispose();
            };
            
        }

        public DelegateCommand StartScreenShare
        {
            get { return GetValue<DelegateCommand>(); }
            set { SetValue(value); }
        }

        public DelegateCommand StopScreenShare
        {
            get { return GetValue<DelegateCommand>(); }
            set { SetValue(value); }
        }

        public DelegateCommand TakeScreenshot
        {
            get { return GetValue<DelegateCommand>(); }
            set { SetValue(value); }
        }

        public void OnStartScreenshare(object arg)
        {

            _screenShare.StartScreenCapture(!CapturePorS, 24);
        }

        public void OnStopScreenshare(object arg)
        {
            _screenShare.StopScreenCapture();

        }

        public void OnTakeScreenshot(object arg)
        {
            void ScreenReceived(object sender, ScreenshareBuffer screenshareBuffer)
            {
                WriteableBitmap bitmap = new WriteableBitmap(new Avalonia.PixelSize(screenshareBuffer.Width, screenshareBuffer.Height), new Avalonia.Vector(96, 96), Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul);
                var bitlock = bitmap.Lock();

                _screenShare.GetFrame(bitlock.Address);
                bitlock.Dispose();
                ScreenShareImage = bitmap;
            }
            _screenShare.NextScreenshotReady += ScreenReceived;
            _screenShare.TakeScreenshot(!CapturePorS);
        }

        public bool CapturePorS
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public WriteableBitmap ImageBrush
        {
            get { return GetValue<WriteableBitmap>(); }
            set { SetValue(value); }
        }

        public WriteableBitmap ScreenShareImage
        {
            get { return GetValue<WriteableBitmap>(); }
            set { SetValue(value); }
        }
    }
}
