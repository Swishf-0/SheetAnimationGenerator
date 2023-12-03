using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SheetAnimationGenerator
{
    public class AnimationFileGenerator
    {
        public static void GenerateAnimationFile(int frameCountX, int frameCountY, int fps, bool addEvents)
        {
            if (frameCountX <= 0)
            {
                Debug.LogError(string.Format(Localize.Text.MSG_TARGET_TEXTURE_SIZE_SHORTAGE, Localize.Text.FRAME_COUNT_X_TEXT));
                return;
            }

            if (frameCountY <= 0)
            {
                Debug.LogError(string.Format(Localize.Text.MSG_TARGET_TEXTURE_SIZE_SHORTAGE, Localize.Text.FRAME_COUNT_Y_TEXT));
                return;
            }

            if (fps <= 0)
            {
                Debug.LogError(string.Format(Localize.Text.MSG_TARGET_TEXTURE_SIZE_SHORTAGE, Localize.Text.FPS_TEXT));
                return;
            }

            var animation = new AnimationClip();

            var settings = AnimationUtility.GetAnimationClipSettings(animation);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(animation, settings);

            var curveBindingEmX = new EditorCurveBinding() { path = "Sheet", type = typeof(MeshRenderer), propertyName = "material._EmissionMap_ST.x" };
            var curveBindingEmY = new EditorCurveBinding() { path = "Sheet", type = typeof(MeshRenderer), propertyName = "material._EmissionMap_ST.y" };
            var curveBindingEmZ = new EditorCurveBinding() { path = "Sheet", type = typeof(MeshRenderer), propertyName = "material._EmissionMap_ST.z" };
            var curveBindingEmW = new EditorCurveBinding() { path = "Sheet", type = typeof(MeshRenderer), propertyName = "material._EmissionMap_ST.w" };
            var curveBindingMtX = new EditorCurveBinding() { path = "Sheet", type = typeof(MeshRenderer), propertyName = "material._MainTex_ST.x" };
            var curveBindingMtY = new EditorCurveBinding() { path = "Sheet", type = typeof(MeshRenderer), propertyName = "material._MainTex_ST.y" };
            var curveBindingMtZ = new EditorCurveBinding() { path = "Sheet", type = typeof(MeshRenderer), propertyName = "material._MainTex_ST.z" };
            var curveBindingMtW = new EditorCurveBinding() { path = "Sheet", type = typeof(MeshRenderer), propertyName = "material._MainTex_ST.w" };

            var curveEmX = new AnimationCurve();
            var curveEmY = new AnimationCurve();
            var curveEmZ = new AnimationCurve();
            var curveEmW = new AnimationCurve();
            var curveMtX = new AnimationCurve();
            var curveMtY = new AnimationCurve();
            var curveMtZ = new AnimationCurve();
            var curveMtW = new AnimationCurve();

            var events = new List<AnimationEvent>();

            float xValue = 1 / (float)frameCountX;
            float yValue = 1 / (float)frameCountY;
            float zValue = 0;
            float wValue = 0;
            float time = 0;

            curveEmX.AddKey(new Keyframe(time, xValue, float.PositiveInfinity, float.PositiveInfinity));
            curveEmY.AddKey(new Keyframe(time, yValue, float.PositiveInfinity, float.PositiveInfinity));
            for (int y = 0; y < frameCountY; y++)
            {
                wValue = 1 - (y + 1) / (float)frameCountY;
                for (int x = 0; x < frameCountX; x++)
                {
                    var frameCount = x + y * frameCountX;
                    time = frameCount / (float)fps;
                    zValue = x / (float)frameCountX;
                    curveEmZ.AddKey(new Keyframe(time, zValue, float.PositiveInfinity, float.PositiveInfinity));
                    curveEmW.AddKey(new Keyframe(time, wValue, float.PositiveInfinity, float.PositiveInfinity));
                    events.Add(new AnimationEvent() { functionName = "AnimationEventOnFrame", time = time, intParameter = frameCount + 1 });
                }
            }
            time += 1 / (float)fps;
            curveEmX.AddKey(new Keyframe(time, xValue, float.PositiveInfinity, float.PositiveInfinity));
            curveEmY.AddKey(new Keyframe(time, yValue, float.PositiveInfinity, float.PositiveInfinity));
            curveEmZ.AddKey(new Keyframe(time, zValue, float.PositiveInfinity, float.PositiveInfinity));
            curveEmW.AddKey(new Keyframe(time, wValue, float.PositiveInfinity, float.PositiveInfinity));

            AnimationUtility.SetEditorCurve(animation, curveBindingEmX, curveEmX);
            AnimationUtility.SetEditorCurve(animation, curveBindingEmY, curveEmY);
            AnimationUtility.SetEditorCurve(animation, curveBindingEmZ, curveEmZ);
            AnimationUtility.SetEditorCurve(animation, curveBindingEmW, curveEmW);
            AnimationUtility.SetEditorCurve(animation, curveBindingMtX, curveEmX);
            AnimationUtility.SetEditorCurve(animation, curveBindingMtY, curveEmY);
            AnimationUtility.SetEditorCurve(animation, curveBindingMtZ, curveEmZ);
            AnimationUtility.SetEditorCurve(animation, curveBindingMtW, curveEmW);

            if (addEvents)
            {
                AnimationUtility.SetAnimationEvents(animation, events.ToArray());
            }

            if (Utils.GetGeneratedAnimationSavePath(frameCountX, frameCountY, addEvents, out var path))
            {
                AssetDatabase.CreateAsset(animation, path);
            }
        }
    }
}
