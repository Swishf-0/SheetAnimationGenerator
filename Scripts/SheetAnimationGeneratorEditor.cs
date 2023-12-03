#if  UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;

namespace SheetAnimationGenerator
{
    [CustomEditor(typeof(SheetAnimationGenerator))]
    public class SheetAnimationGeneratorEditor : Editor
    {
        SheetAnimationGenerator _target;
        SerializedProperty _frameCountX, _frameCountY, _frameWidth, _frameHeight, _fps, _hideReferenceAnimationOnCapture, _transparentBackground, _saveEachFrame, _pileOnReferenece, _refereneceTexture;
        bool _requiresConstantRepaint = false;
        bool _showGenerateAnimationGui, _showSheetAnimationFrameExpander;

        private void OnEnable()
        {
            _target = target as SheetAnimationGenerator;
            _frameCountX = serializedObject.FindProperty("_frameCountX");
            _frameCountY = serializedObject.FindProperty("_frameCountY");
            _frameWidth = serializedObject.FindProperty("_frameWidth");
            _frameHeight = serializedObject.FindProperty("_frameHeight");
            _fps = serializedObject.FindProperty("_fps");
            _hideReferenceAnimationOnCapture = serializedObject.FindProperty("_hideReferenceAnimationOnCapture");
            _transparentBackground = serializedObject.FindProperty("_transparentBackground");
            _saveEachFrame = serializedObject.FindProperty("_saveEachFrame");
            _pileOnReferenece = serializedObject.FindProperty("_pileOnReferenece");
            _refereneceTexture = serializedObject.FindProperty("_refereneceTexture");
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);
        }

        public override bool RequiresConstantRepaint()
        {
            return _requiresConstantRepaint;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            int space = 10;
            GUIHowToUse();
            EditorGUILayout.Space(space);
            GUICaptureParams();
            EditorGUILayout.Space(space);
            GUIStartButton();
            EditorGUILayout.Space(space);
            GUIState();
            EditorGUILayout.Space(space);
            GUISheetAnimationFrameExpander();
            EditorGUILayout.Space(space);
            GUIGenerateAnimation();
            EditorGUILayout.Space(space);
            GUIPileTextures();
            EditorGUILayout.Space(space);
            EditorGUILayout.Space(space);

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }

        void GUIHowToUse()
        {
            EditorGUILayout.LabelField(Localize.Text.HOW_TO_USE_TEXT, EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.Space(3);
                    EditorGUILayout.LabelField($"1. Game ビュー左上のDisplayサイズで {_frameWidth.intValue}x{_frameHeight.intValue} を作成/設定します");
                    EditorGUILayout.LabelField("2. プレイボタンを押します");
                    EditorGUILayout.LabelField("3. この下にあるキャプチャー開始ボタンを押します");
                    EditorGUILayout.LabelField("――――――――");
                    EditorGUILayout.LabelField("撮影例のアニメーションは PlayGround オブジェクト以下に設置されています");
                    EditorGUILayout.Space(3);
                    EditorGUILayout.LabelField("8x8 のアニメーションを使用する場合は OriginalSheet、ResultSheet の");
                    EditorGUILayout.LabelField("下にあるオブジェクトの表示状態を切り替えます");
                    EditorGUILayout.Space(3);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUI.indentLevel--;
        }

        void GUICaptureParams()
        {
            EditorGUILayout.LabelField(Localize.Text.CAPTURE_SETTINGS_TEXT, EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            {
                EditorGUILayout.PropertyField(_frameCountX, new GUIContent(Localize.Text.FRAME_COUNT_X_TEXT));
                if (_frameCountX.intValue <= 0)
                {
                    _frameCountX.intValue = Constants.DefaultValue.FRAME_COUNT_X;
                }

                EditorGUILayout.PropertyField(_frameCountY, new GUIContent(Localize.Text.FRAME_COUNT_Y_TEXT));
                if (_frameCountY.intValue <= 0)
                {
                    _frameCountY.intValue = Constants.DefaultValue.FRAME_COUNT_Y;
                }

                EditorGUILayout.PropertyField(_frameWidth, new GUIContent(Localize.Text.FRAME_SIZE_X_TEXT));
                if (_frameWidth.intValue <= 0)
                {
                    _frameWidth.intValue = Constants.DefaultValue.FRAME_WIDTH;
                }

                EditorGUILayout.PropertyField(_frameHeight, new GUIContent(Localize.Text.FRAME_SIZE_Y_TEXT));
                if (_frameHeight.intValue <= 0)
                {
                    _frameHeight.intValue = Constants.DefaultValue.FRAME_HEIGHT;
                }

                EditorGUILayout.PropertyField(_fps, new GUIContent(Localize.Text.FPS_TEXT));
                if (_fps.intValue <= 0)
                {
                    _fps.intValue = Constants.DefaultValue.FPS;
                }

                EditorGUILayout.Space(5);
                string helpText = $"{Localize.Text.GENERATED_IMAGE_SIZE_TEXT}: {_frameWidth.intValue}x{_frameCountX.intValue} x {_frameHeight.intValue}x{_frameCountY.intValue} → {_frameWidth.intValue * _frameCountX.intValue} x {_frameHeight.intValue * _frameCountY.intValue}\n";
                helpText += $"{Localize.Text.GENERATED_ANIMATION_DURATION_TEXT}: {(_frameCountX.intValue * _frameCountY.intValue / (float)_fps.intValue).ToString("F4")}sec";
                EditorGUILayout.HelpBox(helpText, MessageType.Info);

                EditorGUILayout.Space(10);
                EditorGUI.BeginDisabledGroup(_target.IsAnimationCapturing);
                {
                    EditorGUILayout.PropertyField(_hideReferenceAnimationOnCapture, new GUIContent("撮影時に後ろのアニメーションを隠す"));

                    EditorGUILayout.PropertyField(_transparentBackground, new GUIContent("背景を透明にする"));
                    EditorGUI.indentLevel++;
                    {
                        if (_transparentBackground.boolValue)
                        {
                            EditorGUILayout.PropertyField(_pileOnReferenece, new GUIContent("参照画像と結合する"));
                            EditorGUI.indentLevel++;
                            {
                                if (_pileOnReferenece.boolValue)
                                {
                                    EditorGUILayout.PropertyField(_refereneceTexture, new GUIContent("参照画像"));
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    EditorGUI.indentLevel--;

                    EditorGUILayout.PropertyField(_saveEachFrame, new GUIContent("各フレームも画像保存する (動作確認用)"));
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.indentLevel--;
        }

        void GUIStartButton()
        {
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying || _target.IsAnimationCapturing);
            {
                if (GUILayout.Button(!EditorApplication.isPlaying ? Localize.Text.START_ANIMATION_CAPTURE_TEXT_DISABLED : _target.IsAnimationCapturing ? Localize.Text.IN_ANIMATION_CAPTURING_TEXT : Localize.Text.START_ANIMATION_CAPTURE_TEXT))
                {
                    _target.StartAnimationCapture();
                    _requiresConstantRepaint = true;
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        void GUIState()
        {
            EditorGUILayout.LabelField(Localize.Text.LOG_TEXT, EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.SelectableLabel(GetStateText());
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUI.indentLevel--;
        }

        string GetStateText()
        {
            if (!_target.IsAnimationCapturing)
            {
                _requiresConstantRepaint = false;
                return Localize.Text.WAITING_START_ANIMATION_CAPTURE_TEXT;
            }

            if (_target.HasAnimationCaptureStarted)
            {
                return Localize.Text.IN_ANIMATION_CAPTURING_TEXT;
            }

            return Localize.Text.PREPARING_ANIMATION_CAPTURE_TEXT;
        }

        bool _addAnimationEventsOnGenerate = true;
        void GUIGenerateAnimation()
        {
            if (_showGenerateAnimationGui = EditorGUILayout.Foldout(_showGenerateAnimationGui, Localize.Text.GENERATE_ANIMATION_FILE_TEXT))
            {
                EditorGUILayout.LabelField(Localize.Text.HOW_TO_USE_TEXT, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        EditorGUILayout.Space(3);
                        EditorGUILayout.LabelField("8x4、8x8 以外のサイズでのキャプチャに必要なアニメーションファイルを生成します");
                        EditorGUILayout.LabelField("生成場所: SheetAnimationGenerator/SheetAnimations/GeneratedAnimation_～.anim");
                        EditorGUILayout.LabelField("――――――――");
                        EditorGUILayout.LabelField("1. キャプチャ設定で以下を設定します");
                        EditorGUILayout.LabelField($"　{Localize.Text.FRAME_COUNT_X_TEXT}、{Localize.Text.FRAME_COUNT_Y_TEXT}、{Localize.Text.FPS_TEXT}");
                        EditorGUILayout.LabelField("2. この下にあるアニメーション生成ボタンを押します");
                        EditorGUILayout.LabelField("3. Hierarchy の ResultSheet/Sheet_8x8 をコピーして対象サイズに合わせた設定をします");
                        EditorGUILayout.Space(3);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUI.indentLevel--;

                _addAnimationEventsOnGenerate = EditorGUILayout.Toggle("イベントを付与する", _addAnimationEventsOnGenerate);
                if (GUILayout.Button(Localize.Text.GENERATE_ANIMATION_FILE_TEXT))
                {
                    AnimationFileGenerator.GenerateAnimationFile(_frameCountX.intValue, _frameCountY.intValue, _fps.intValue, _addAnimationEventsOnGenerate);
                }
            }
        }

        void GUISheetAnimationFrameExpander()
        {
            if (_showSheetAnimationFrameExpander = EditorGUILayout.Foldout(_showSheetAnimationFrameExpander, "アニメーションフレーム倍化"))
            {
                SheetAnimationFrameExpanderEditorHelper.OnGUISheetAnimationFrameExpander();
            }
        }

        Texture2D _pileBaseTex, _pileOverTex;
        bool _showPileTextures;
        void GUIPileTextures()
        {
            if (_showPileTextures = EditorGUILayout.Foldout(_showPileTextures, "画像結合"))
            {
                EditorGUILayout.LabelField(Localize.Text.HOW_TO_USE_TEXT, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        EditorGUILayout.Space(3);
                        EditorGUILayout.LabelField("選択した2枚の画像を重ね合わせた画像を出力します");
                        EditorGUILayout.Space(3);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUI.indentLevel--;

                _pileBaseTex = (Texture2D)EditorGUILayout.ObjectField("背景画像", _pileBaseTex, typeof(Texture2D), true);
                _pileOverTex = (Texture2D)EditorGUILayout.ObjectField("重ねる画像", _pileOverTex, typeof(Texture2D), true);
                if (GUILayout.Button("画像結合"))
                {
                    if (!_pileBaseTex.isReadable)
                    {
                        _pileBaseTex = Utils.ReadTexture(_pileBaseTex);
                    }
                    if (!_pileOverTex.isReadable)
                    {
                        _pileOverTex = Utils.ReadTexture(_pileOverTex);
                    }

                    if (PileTextures.Pile(_pileBaseTex, _pileOverTex, out var resultTex))
                    {
                        if (Utils.GetSavePathOnDesktopWithDate(out var path))
                        {
                            File.WriteAllBytes(path, resultTex.EncodeToPNG());
                        }
                    }
                }
            }
        }

    }
}

#endif
