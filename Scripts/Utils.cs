using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SheetAnimationGenerator
{
    public class Utils
    {
        public static bool GetSavePathOnDesktopWithDate(out string path)
        {
            var name = $"{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.png";
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            path = Path.Combine(dir, name);
            Debug.Log(path);
            return CheckPathDirExistance(path);
        }

        public static bool GetFrameSavePath(int i, out string path)
        {
            path = PathNameUtil.GetFrameResultPath(i);
            Debug.Log(path);
            return CheckPathDirExistance(path);
        }

        public static bool GetGeneratedAnimationSavePath(int x, int y, bool addEvents, out string path)
        {
            path = PathNameUtil.GetGeneratedAnimationPath(x, y);
            if (!addEvents)
            {
                path = InsertPostfixToFileName(path, "_noevents");
            }
            Debug.Log(path);
            return CheckPathDirExistance(path);
        }

        static bool CheckPathDirExistance(string path)
        {
            if (!System.IO.Directory.Exists(Path.GetDirectoryName(path)))
            {
                Debug.LogError($"ファイル {Path.GetFileName(path)} の保存先ディレクトリ {Path.GetDirectoryName(path)} が存在しません");
                return false;
            }

            return true;
        }

        public static string GetSavePath(string path, string postfix, string defaultPath)
        {
            var savePath = string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                savePath = InsertPostfixToFileName(defaultPath, postfix);
            }
            else
            {
                savePath = InsertPostfixToFileName(path, postfix);
            }

            int dupCount = 0;
            while (File.Exists(savePath))
            {
                savePath = InsertPostfixToFileName(path, $" ({++dupCount})");
            }

            return savePath;
        }

        static string InsertPostfixToFileName(string path, string postfix)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Path.Combine(Path.GetDirectoryName(path), $"{Path.GetFileNameWithoutExtension(path)}{postfix}{Path.GetExtension(path)}");
        }

        public static Texture2D CropTexture(Texture2D tex, int width, int height)
        {
            Texture2D croppedTex = new Texture2D(width, height, tex.format, false);
            int minX = (tex.width - width) / 2;
            int minY = (tex.height - height) / 2;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    croppedTex.SetPixel(x, y, tex.GetPixel(minX + x, minY + y));
                }
            }
            return croppedTex;
        }

        /// <summary>
        /// isReadableではないテクスチャ向け
        /// </summary>
        public static Texture2D ReadTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        public static Texture2D CombineFrameTextures(Dictionary<int, Texture2D> frameTextures, int frameCountX, int frameCountY)
        {
            var frameTexture = GetAnyFrameTexture(frameTextures);
            if (frameTexture == null)
            {
                return null;
            }
            int frameWidth = frameTexture.width;
            int frameHeight = frameTexture.height;
            Texture2D combinedTex = new Texture2D(frameWidth * frameCountX, frameHeight * frameCountY, frameTexture.format, false);

            foreach (KeyValuePair<int, Texture2D> kvp in frameTextures)
            {
                var frame = kvp.Key - 1;
                var tex = kvp.Value;
                if (tex == null)
                {
                    continue;
                }

                var _px = (frame % frameCountX) * frameWidth;
                var _py = (frameCountY - 1 - frame / frameCountX) * frameHeight;
                for (int y = 0; y < frameHeight; y++)
                {
                    for (int x = 0; x < frameWidth; x++)
                    {
                        combinedTex.SetPixel(_px + x, _py + y, tex.GetPixel(x, y));
                    }
                }
            }

            return combinedTex;
        }

        static Texture2D GetAnyFrameTexture(Dictionary<int, Texture2D> frameTextures)
        {
            foreach (var tex in frameTextures.Values)
            {
                if (tex == null)
                {
                    continue;
                }
                return tex;
            }
            return null;
        }
    }
}
