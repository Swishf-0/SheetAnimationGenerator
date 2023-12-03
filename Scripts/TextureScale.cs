using System.Threading;
using UnityEngine;

namespace SheetAnimationGenerator
{
    /// <summary>
    /// http://wiki.unity3d.com/index.php/TextureScale
    /// </summary>
    public class TextureScale
    {
        class ThreadData
        {
            public int Start { get; private set; }
            public int End { get; private set; }

            public ThreadData(int start, int end)
            {
                Start = start;
                End = end;
            }
        }

        static Color32[] originalTexColors;
        static Color32[] newColors;
        static int originalTexWidth;
        static float ratioX;
        static float ratioY;
        static int newTexWidth;
        static int finishCount;
        static Mutex mutex;

        public static void Point(Texture2D tex, int newWidth, int newHeight)
        {
            ThreadedScale(tex, newWidth, newHeight, false);
        }

        public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
        {
            ThreadedScale(tex, newWidth, newHeight, true);
        }

        static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
        {
            originalTexColors = tex.GetPixels32();
            newColors = new Color32[newWidth * newHeight];
            if (useBilinear)
            {
                ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
                ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
            }
            else
            {
                ratioX = ((float)tex.width) / newWidth;
                ratioY = ((float)tex.height) / newHeight;
            }
            originalTexWidth = tex.width;
            newTexWidth = newWidth;
            var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
            var slice = newHeight / cores;

            finishCount = 0;
            if (mutex == null)
            {
                mutex = new Mutex(false);
            }
            if (cores > 1)
            {
                int i = 0;
                ThreadData threadData;
                for (i = 0; i < cores - 1; i++)
                {
                    threadData = new ThreadData(slice * i, slice * (i + 1));
                    var ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
                    var thread = new Thread(ts);
                    thread.Start(threadData);
                }
                threadData = new ThreadData(slice * i, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }
                while (finishCount < cores)
                {
                    Thread.Sleep(1);
                }
            }
            else
            {
                ThreadData threadData = new ThreadData(0, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }
            }

            tex.Resize(newWidth, newHeight);
            tex.SetPixels32(newColors);
            tex.Apply();

            originalTexColors = null;
            newColors = null;
        }

        static void BilinearScale(System.Object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (var y = threadData.Start; y < threadData.End; y++)
            {
                int yFloor = (int)Mathf.Floor(y * ratioY);
                var y1 = yFloor * originalTexWidth;
                var y2 = (yFloor + 1) * originalTexWidth;
                var yw = y * newTexWidth;

                for (var x = 0; x < newTexWidth; x++)
                {
                    int xFloor = (int)Mathf.Floor(x * ratioX);
                    var xLerp = x * ratioX - xFloor;
                    newColors[yw + x] = ColorLerpUnclamped(
                        ColorLerpUnclamped(originalTexColors[y1 + xFloor], originalTexColors[y1 + xFloor + 1], xLerp),
                        ColorLerpUnclamped(originalTexColors[y2 + xFloor], originalTexColors[y2 + xFloor + 1], xLerp),
                        y * ratioY - yFloor);
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        static void PointScale(System.Object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (var y = threadData.Start; y < threadData.End; y++)
            {
                var thisY = (int)(ratioY * y) * originalTexWidth;
                var yw = y * newTexWidth;
                for (var x = 0; x < newTexWidth; x++)
                {
                    newColors[yw + x] = originalTexColors[(int)(thisY + ratioX * x)];
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        static Color ColorLerpUnclamped(Color c1, Color c2, float value)
        {
            return new Color(
                c1.r + (c2.r - c1.r) * value,
                c1.g + (c2.g - c1.g) * value,
                c1.b + (c2.b - c1.b) * value,
                c1.a + (c2.a - c1.a) * value);
        }
    }
}
