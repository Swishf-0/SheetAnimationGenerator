#if  UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SheetAnimationGenerator
{
    public class ScreenshotEditorTool : EditorWindow
    {
        [MenuItem("SheetAnimTools/Screenshot x1")]
        static void CaptureSceneCameraViewX1()
        {
            CaptureGameViewCameraView(1);
        }

        [MenuItem("SheetAnimTools/Screenshot x2")]
        static void CaptureSceneCameraViewX2()
        {
            CaptureGameViewCameraView(2);
        }

        [MenuItem("SheetAnimTools/Screenshot x4")]
        static void CaptureSceneCameraViewX4()
        {
            CaptureGameViewCameraView(4);
        }

        static void CaptureGameViewCameraView(int superSize)
        {
            if (Utils.GetSavePathOnDesktopWithDate(out var path))
            {
                ScreenCapture.CaptureScreenshot(path, superSize: superSize);
            }
        }
    }
}

#endif
