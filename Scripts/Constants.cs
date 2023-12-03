using System.IO;
using UnityEngine;

namespace SheetAnimationGenerator
{
    public class Constants
    {
        public class DefaultValue
        {
            public const int FRAME_COUNT_X = 8;
            public const int FRAME_COUNT_Y = 4;
            public const int FRAME_WIDTH = 256;
            public const int FRAME_HEIGHT = 256;
            public const int FPS = 15;
        }

        public class PathName
        {
            public const string FRAME_RESULT_PARENT_DIR = "Assets/SheetAnimationGenerator";
            public const string FRAME_RESULT_FOLDER_NAME = "out";
            public const string FRAME_RESULT_FILE_NAME = "anim_frame_{0}.png";
            public const string GENERATED_ANIMATION_PATH = "Assets/SheetAnimationGenerator/SheetAnimations/GeneratedAnimation_{0}x{1}.anim";
        }
    }

    public class PathNameUtil
    {
        public static string GetFrameResultPath(int i)
        {
            return Path.Combine(Constants.PathName.FRAME_RESULT_PARENT_DIR, Constants.PathName.FRAME_RESULT_FOLDER_NAME, GetFrameResultFileName(i));
        }

        static string GetFrameResultFileName(int i)
        {
            return string.Format(Constants.PathName.FRAME_RESULT_FILE_NAME, i.ToString());
        }

        static string GetFrameResultDir()
        {
            return Path.Combine(Constants.PathName.FRAME_RESULT_PARENT_DIR, Constants.PathName.FRAME_RESULT_FOLDER_NAME);
        }

        static bool ExistsFrameResultParentDir()
        {
            return System.IO.Directory.Exists(Constants.PathName.FRAME_RESULT_PARENT_DIR);
        }

        static bool ExistsFrameResultDir()
        {
            return System.IO.Directory.Exists(GetFrameResultDir());
        }

        public static bool CheckAndCreateFrameResultSaveDir()
        {
            if (!ExistsFrameResultParentDir())
            {
                Debug.LogError($"フレーム画像の保存先 {Constants.PathName.FRAME_RESULT_PARENT_DIR} が存在しません");
                return false;
            }

            if (!ExistsFrameResultDir())
            {
                Debug.LogWarning($"フレーム画像の保存先 {GetFrameResultDir()} が存在しないため {Constants.PathName.FRAME_RESULT_FOLDER_NAME} フォルダーを作成します");
                Directory.CreateDirectory(GetFrameResultDir());
                return ExistsFrameResultDir();
            }

            return true;
        }

        public static string GetGeneratedAnimationPath(int x, int y)
        {
            return string.Format(Constants.PathName.GENERATED_ANIMATION_PATH, x.ToString(), y.ToString());
        }
    }
}
