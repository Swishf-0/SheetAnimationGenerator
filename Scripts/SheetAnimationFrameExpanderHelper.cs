using System.IO;
using UnityEngine;

namespace SheetAnimationGenerator
{
    public class SheetAnimationFrameExpanderHelper
    {
        const string DEFAULT_SAVE_PATH = "Asset/animation_sheet.png";

        public static void ExpandFrames(Texture2D targetTex, int targetTexFrameCountX, int targetTexFrameCountY, int expandRateX, int expandRateY, string path)
        {
            if (targetTex == null)
            {
                Debug.LogError(string.Format(Localize.Text.MSG_TARGET_TEXTURE_NULL, Localize.Text.TARGET_TEXTURE_TEXT));
                return;
            }

            if (targetTexFrameCountX <= 0)
            {
                Debug.LogError(string.Format(Localize.Text.MSG_TARGET_TEXTURE_SIZE_SHORTAGE, Localize.Text.FRAME_COUNT_X_TEXT));
                return;
            }

            if (targetTexFrameCountY <= 0)
            {
                Debug.LogError(string.Format(Localize.Text.MSG_TARGET_TEXTURE_SIZE_SHORTAGE, Localize.Text.FRAME_COUNT_Y_TEXT));
                return;
            }

            var tex = SheetAnimationFrameExpander.ExpandFrames(targetTex, targetTexFrameCountX, targetTexFrameCountY, expandRateX, expandRateY);
            SaveTexAsPng(tex, path, $"_{targetTexFrameCountX * expandRateX}x{targetTexFrameCountY * expandRateY}");
        }

        static void SaveTexAsPng(Texture2D tex, string basePath, string postfix)
        {
            if (!string.IsNullOrEmpty(basePath) || true)
            {
                var png = tex.EncodeToPNG();
                var path = Utils.GetSavePath(basePath, postfix, DEFAULT_SAVE_PATH);
                File.WriteAllBytes(path, png);
                Debug.Log(string.Format(Localize.Text.SAVED_AT_TEXT, path));
            }
        }
    }
}
