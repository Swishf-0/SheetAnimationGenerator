using UnityEngine;

namespace SheetAnimationGenerator
{
    public class SheetAnimationFrameExpander
    {
        public static Texture2D ExpandFrames(Texture2D targetTex, int targetTexFrameCountX, int targetTexFrameCountY, int expandRateX, int expandRateY)
        {
            var targetTexFrameCount = targetTexFrameCountX * targetTexFrameCountY;
            var targetTexFrameWidth = targetTex.width / targetTexFrameCountX;
            var targetTexFrameHeight = targetTex.height / targetTexFrameCountY;
            var targetTexFrameArea = targetTexFrameWidth * targetTexFrameHeight;

            if (!targetTex.isReadable)
            {
                targetTex = Utils.ReadTexture(targetTex);
            }

            Color32[][][][] targetTexFrames = new Color32[targetTexFrameCountY][][][];
            for (int y = targetTexFrameCountY - 1; y >= 0; y--)
            {
                targetTexFrames[y] = new Color32[targetTexFrameCountX][][];
                for (int x = 0; x < targetTexFrameCountX; x++)
                {
                    targetTexFrames[y][x] = new Color32[targetTexFrameHeight][];
                    for (int _y = targetTexFrameHeight - 1; _y >= 0; _y--)
                    {
                        targetTexFrames[y][x][_y] = new Color32[targetTexFrameWidth];
                        for (int _x = 0; _x < targetTexFrameWidth; _x++)
                        {
                            targetTexFrames[y][x][_y][_x] = targetTex.GetPixel(x * targetTexFrameWidth + _x, y * targetTexFrameHeight + _y).gamma;
                        }
                    }
                }
            }

            Texture2D expandedTex = new Texture2D(targetTex.width * expandRateX, targetTex.height * expandRateY, targetTex.format, false, true);

            int expandFrameCountX = targetTexFrameCountX * expandRateX;
            int f = 0;
            int ex = targetTexFrameCountX * expandRateX;
            int ey = targetTexFrameCountY * expandRateY;
            for (int y = targetTexFrameCountY - 1; y >= 0; y--)
            {
                for (int x = 0; x < targetTexFrameCountX; x++)
                {
                    for (int c = 0; c < expandRateX * expandRateY; c++)
                    {
                        var _px = (f % ex) * targetTexFrameWidth;
                        var _py = (ey - 1 - f / ex) * targetTexFrameHeight;

                        for (int _y = 0; _y < targetTexFrameHeight; _y++)
                        {
                            for (int _x = 0; _x < targetTexFrameWidth; _x++)
                            {
                                expandedTex.SetPixel(_px + _x, _py + _y, targetTexFrames[y][x][_y][_x]);
                            }
                        }

                        f++;
                    }
                }
            }

            return expandedTex;
        }
    }
}
