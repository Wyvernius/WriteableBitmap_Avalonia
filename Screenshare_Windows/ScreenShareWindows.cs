using System;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using Timers;
using Slidecrew_Interfaces;
using System.Runtime.InteropServices;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using System.Collections.Generic;

namespace Slidecrew.ScreenShare.Windows
{
    public class ScreenShare : IScreenShare
    {
        #region Windows
        Texture2D _screenTexture;
        Device _device;
        HighResTimer _timer;

        bool _doneSetup = false;
        bool _isRecording = false;
        bool _frameDisposed = false;
        bool? _primaryScreen = null;
        OutputDuplication _duplicatedOutput { get; set; }

        GCHandle _bufferBGRAPinned;
        byte[] _bufferBGRA;
        IntPtr _pbufferBGRA;

        // save current active outputs and refcounts so we can have multiple instances using the same screen.
        static Dictionary<bool, OutputDuplication> _outputs = new Dictionary<bool, OutputDuplication>();
        static Dictionary<bool, int> _outputRefCount = new Dictionary<bool, int>();

        public int Width { get; private set; }
        public int Height { get; private set; }
        public float Ratio { get; private set; }

        public EventHandler<ScreenshareBuffer> NextFrameReady { get; set; }
        public EventHandler<ScreenshareBuffer> NextScreenshotReady { get; set; }

        public ScreenShare()
        {
            
        }


        private void Setup(float frameRate = 30f )
        {
            _timer = new HighResTimer((1f / frameRate) * 1000f, OnGetNextFrame);

            // Create DXGI Factory1
            var factory = new Factory1();
            int numAdapter = 0;
            int numOutput = -1;

            for (int i = 0; i < factory.GetAdapterCount(); i++)
            {
                numAdapter = i;
                var tmpadapter = factory.GetAdapter(i);
                for (int j = 0; j < tmpadapter.GetOutputCount(); j++)
                {
                    var Out = tmpadapter.GetOutput(j);
                    if ((Out.Description.DesktopBounds.Left == 0 && (bool)_primaryScreen) || (Out.Description.DesktopBounds.Left != 0 && !(bool)_primaryScreen))
                    {
                        numOutput = j;
                        break; // break inner loop
                    }
                    Out.Dispose();
                }
                tmpadapter.Dispose();
                if (numOutput != -1)
                    break; // break outer loop
            }

            // Create DXGI Factory1
           // var factory = new Factory1();
            var adapter = factory.GetAdapter1(numAdapter);

            // Create device from Adapter
            _device = new Device(adapter);

            // Get DXGI.Output
            var output = adapter.GetOutput(numOutput);
            var output1 = output.QueryInterface<Output1>();

            // Width/Height of desktop to capture
            Width = output.Description.DesktopBounds.Right - output.Description.DesktopBounds.Left;
            Height = output.Description.DesktopBounds.Bottom - output.Description.DesktopBounds.Top;
            Ratio = (float)Width / (float)Height;

            // Create Staging texture CPU-accessible
            var textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = Width,
                Height = Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };
            _screenTexture = new Texture2D(_device, textureDesc);


            if (_outputs.ContainsKey((bool)_primaryScreen))
            {
                _duplicatedOutput = _outputs[(bool)_primaryScreen];
                _outputRefCount[(bool)_primaryScreen]++;
            }
            else
            { 
                // Duplicate the output
                _duplicatedOutput = output1.DuplicateOutput(_device);
                _outputs.Add((bool)_primaryScreen, _duplicatedOutput);
                _outputRefCount.Add((bool)_primaryScreen, 1);
            }

            _bufferBGRA = new byte[Width * Height * 4];
            _bufferBGRAPinned = GCHandle.Alloc(_bufferBGRA, GCHandleType.Pinned);
            _pbufferBGRA = _bufferBGRAPinned.AddrOfPinnedObject();
        }

        private void OnGetNextFrame()
        {
            bool captureDone = false;
            _frameDisposed = false;
            try
            {
                SharpDX.DXGI.Resource screenResource;
                OutputDuplicateFrameInformation duplicateFrameInformation;


                // Try to get duplicated frame within given time
                var res = _duplicatedOutput.TryAcquireNextFrame(1, out duplicateFrameInformation, out screenResource);

                if (res.Success)
                {
                    // copy resource into memory that can be accessed by the CPU
                    using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
                        _device.ImmediateContext.CopyResource(screenTexture2D, _screenTexture);

                    // Get the desktop capture texture
                    var mapSource = _device.ImmediateContext.MapSubresource(_screenTexture, 0, MapMode.Read, MapFlags.None);

                    // Get pointer of mapSource
                    var sourcePtr = mapSource.DataPointer;

                    // Copy Memory into buffer
                    Utilities.CopyMemory(_pbufferBGRA, sourcePtr, Width * Height * 4);

                    // Release source and dest locks
                    _device.ImmediateContext.UnmapSubresource(_screenTexture, 0);
                    screenResource?.Dispose();
                    _duplicatedOutput.ReleaseFrame();
                }

                if (_bufferBGRA[3] != 0) // check if we have a valid texture by checking alpha channel.
                {
                    NextFrameReady?.Invoke(this, new ScreenshareBuffer() { Buffer = _pbufferBGRA, Width = Width, Height = Height });
                }

                // Capture done
                captureDone = true;
                _frameDisposed = true;
            }
            catch (SharpDXException e)
            {
                if (e.ResultCode.Code != SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
                {
                    throw e;
                }
            }
        }

        public void GetFrame(IntPtr buffer, int width = 0, int height = 0)
        {
            Utilities.CopyMemory(buffer, _pbufferBGRA, Width * Height * 4);


            //FPS();
        }

        int i = 0;
        DateTime time = DateTime.Now;
        void FPS()
        {
            if (DateTime.Now - time >= new TimeSpan(0, 0, 1))
            {
                Console.WriteLine($"FPS: {i}");
                i = 0;
                time = DateTime.Now;
            }
            i++;
        }

        public void TakeScreenshot(bool primary)
        {
            if (_primaryScreen == null)
                _primaryScreen = primary;

            bool screenshotTaken = false;

            void OnFrameReady(object sender, ScreenshareBuffer Inbuffer)
            {
                NextFrameReady -= OnFrameReady;

                NextScreenshotReady?.Invoke(this, Inbuffer);

                screenshotTaken = true;
            }

            // install callback
            NextFrameReady += OnFrameReady;

            if (_isRecording)
            {
                // just wait for frame
                System.Threading.SpinWait.SpinUntil(() => { return screenshotTaken; }, -1);
            }
            else
            {
                if (!_doneSetup)
                    Setup(30);

                _timer.Start();
                System.Threading.SpinWait.SpinUntil(() => { return screenshotTaken && _frameDisposed; }, -1);

                if (!_isRecording)
                    StopScreenCapture();
            }
        }

        public void StartScreenCapture(bool primary, float frameRate = 30f)
        {
            if (_primaryScreen == null)
                _primaryScreen = primary;

            if (!_doneSetup)
                Setup(frameRate);

            if (!_timer.Running)
                _timer.Start();

            _isRecording = true;
            _doneSetup = true;
        }

        public void StopScreenCapture()
        {
            _timer.Stop();

            System.Threading.SpinWait.SpinUntil(() => { return _frameDisposed && !_timer.Running; }, -1);

            if (_primaryScreen == null)
                return;

            _outputRefCount[(bool)_primaryScreen]--;
            if (_outputRefCount[(bool)_primaryScreen] == 0)
            {
                _outputs.Remove((bool)_primaryScreen);
                _outputRefCount.Remove((bool)_primaryScreen);

                _duplicatedOutput.Dispose();
            }

            _device.Dispose();
            _screenTexture.Dispose();

            if (_bufferBGRAPinned.IsAllocated)
                _bufferBGRAPinned.Free();
            
            _doneSetup = false;
            _isRecording = false;
            _primaryScreen = null;
        }

        ~ScreenShare()
        {

        }
        #endregion
    }
}
