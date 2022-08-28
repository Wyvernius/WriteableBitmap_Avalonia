using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slidecrew_Interfaces
{
    public interface IScreenShare
    {
        /// <summary>
        /// output width of the screencapture
        /// </summary>
        int Width { get; }

        /// <summary>
        /// output height of the screen capture
        /// </summary>
        int Height { get; }

        /// <summary>
        /// output ratio of the screen capture
        /// </summary>
        float Ratio { get; }

        EventHandler<ScreenshareBuffer> NextFrameReady { get; set; }
        EventHandler<ScreenshareBuffer> NextScreenshotReady { get; set; }

        void GetFrame(IntPtr buffer, int width = 0, int height = 0);

        void TakeScreenshot(bool primary);

        /// <summary>
        /// Setup ScreenShare
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="Callback">Callback to invoke when new buffer is ready</param>
        /// <param name="frameEnumerator"></param>
        /// <param name="frameDenumerator"></param>
        void StartScreenCapture(bool primary, float frameRate = 25);
        void StopScreenCapture();
    }

    public struct ScreenshareBuffer
    {
        public IntPtr Buffer;
        public int Width;
        public int Height;
    }
}
