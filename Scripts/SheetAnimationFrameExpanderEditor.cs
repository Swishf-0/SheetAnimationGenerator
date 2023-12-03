#if  UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace SheetAnimationGenerator
{
    public class SheetAnimationFrameExpanderEditorHelper
    {
        static SheetAnimationFrameExpanderEditor _sheetAnimationFrameExpanderEditor;
        public static void OnGUISheetAnimationFrameExpander()
        {
            if (_sheetAnimationFrameExpanderEditor == null)
            {
                _sheetAnimationFrameExpanderEditor = new SheetAnimationFrameExpanderEditor();
            }

            _sheetAnimationFrameExpanderEditor.OnGUISheetAnimationFrameExpander();
        }
    }

    public class SheetAnimationFrameExpanderEditor
    {
        Texture2D _targetTex;
        int _targetTexFrameCount, _targetTexFrameCountX = 8, _targetTexFrameCountY = 4;
        int _targetTexFrameArea, _targetTexFrameWidth, _targetTexFrameHeight;
        int _expandRateX = 1, _expandRateY = 2;
        string _targetPath;

        public void OnGUISheetAnimationFrameExpander()
        {
            OnGUIHowToUse();

            EditorGUILayout.LabelField("変換元テクスチャ設定", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            {
                OnGUITargetTex();

                _targetTexFrameCountX = EditorGUILayout.IntField("横のフレーム数", _targetTexFrameCountX);
                _targetTexFrameCountY = EditorGUILayout.IntField("縦のフレーム数", _targetTexFrameCountY);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("出力設定", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                _expandRateX = EditorGUILayout.IntField("横の倍率", _expandRateX);
                _expandRateY = EditorGUILayout.IntField("縦の倍率", _expandRateY);

                EditorGUILayout.Space(5);
                OnGUIInfo();
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(5);
            OnGUIExecButton();
        }

        void OnGUIHowToUse()
        {
            EditorGUILayout.LabelField(Localize.Text.HOW_TO_USE_TEXT, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("アニメーションのフレーム数を増やします");
                EditorGUILayout.LabelField("例: 8x4 のアニメーションの各フレームを倍にして 8x8 のアニメーションを生成します");
                EditorGUILayout.LabelField("――――――――");
                EditorGUILayout.LabelField("1. 変換したい画像を選択します");
                EditorGUILayout.LabelField("2. 変換前のフレーム数を入力します");
                EditorGUILayout.LabelField("3. 縦横それぞれ何倍にするか入力します");
                EditorGUILayout.LabelField("4. アニメーションフレーム倍化ボタンを押します");
                EditorGUILayout.Space(3);
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        void OnGUITargetTex()
        {
            var tex = (Texture2D)EditorGUILayout.ObjectField(Localize.Text.TARGET_TEXTURE_TEXT, _targetTex, typeof(Texture2D), true);
            if (tex == null || tex == _targetTex)
            {
                return;
            }

            _targetTex = tex;
            OnSelectTargetTex();
        }

        void OnGUIInfo()
        {
            if (_targetTex == null)
            {
                return;
            }

            string helpText = "";
            helpText += "変換元画像：\n";
            helpText += $"　　{_targetTex.width} x {_targetTex.height}\n";
            helpText += $"　　{_targetTexFrameCountX} x {_targetTexFrameCountY} フレーム\n";
            helpText += "生成される画像：\n";
            helpText += $"　　サイズ：{_targetTex.width}x{_expandRateX} x {_targetTex.height}x{_expandRateY} → {_targetTex.width * _expandRateX} x {_targetTex.height * _expandRateY}\n";
            helpText += $"　　フレーム数：{_targetTexFrameCountX}x{_expandRateX} x {_targetTexFrameCountY}x{_expandRateY} → {_targetTexFrameCountX * _expandRateX} x {_targetTexFrameCountY * _expandRateY} → {_targetTexFrameCountX * _expandRateX * _targetTexFrameCountY * _expandRateY} フレーム";
            EditorGUILayout.HelpBox(helpText, MessageType.Info);
        }

        void OnGUIExecButton()
        {
            if (GUILayout.Button("アニメーションフレーム倍化"))
            {
                SheetAnimationFrameExpanderHelper.ExpandFrames(_targetTex, _targetTexFrameCountX, _targetTexFrameCountY, _expandRateX, _expandRateY, _targetPath);
            }
        }

        void OnSelectTargetTex()
        {
            _targetPath = AssetDatabase.GetAssetPath(_targetTex);
        }
    }
}

#endif
