using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SheetAnimationGenerator
{
    public class SheetAnimationGenerator : MonoBehaviour
    {
        public bool IsAnimationCapturing => _isAnimationCapturing;
        public bool HasAnimationCaptureStarted => _capturedFrames != null;

        const float TIME_SCALE_ON_CAPTURING = 0.1f;
        const int SUPER_SIZE = 1;

        static readonly Vector3 REFERENCE_ANIMATION_POSITION_ON_CAPTURING = new Vector3(-2.5f, 0, 0);

        [SerializeField] GameObject _referenceAnimation;
        [SerializeField] GameObject _background;
        [SerializeField] AnimationEventReceiver[] _animationEventReceivers;
        [SerializeField, HideInInspector] int _frameWidth = 256;
        [SerializeField, HideInInspector] int _frameHeight = 256;
        [SerializeField, HideInInspector] int _frameCountX = 8;
        [SerializeField, HideInInspector] int _frameCountY = 8;
#pragma warning disable 0414
        [SerializeField, HideInInspector] int _fps = 15;
#pragma warning restore 0414
        [SerializeField, HideInInspector] bool _hideReferenceAnimationOnCapture = true;
        [SerializeField, HideInInspector] bool _transparentBackground = true;
        [SerializeField, HideInInspector] bool _saveEachFrame = false;
        [SerializeField, HideInInspector] bool _pileOnReferenece = false;
        [SerializeField, HideInInspector] Texture2D _refereneceTexture;

        static SheetAnimationGenerator _instance;
        bool _isAnimationCapturing = false;
        bool _isFrameCapturing = false;
        bool _hasNextFrameCapture = false;
        int _currentFrame;
        Dictionary<int, Texture2D> _capturedFrames;

        public void StartAnimationCapture()
        {
            if (_isAnimationCapturing)
            {
                return;
            }

            if (_saveEachFrame)
            {
                PathNameUtil.CheckAndCreateFrameResultSaveDir();
            }

            _isAnimationCapturing = true;
        }

        void Start()
        {
            _instance = this;
            _animationEventReceivers = GameObject.FindObjectsOfType<AnimationEventReceiver>();

            if (_animationEventReceivers.Length != 1)
            {
                if (_animationEventReceivers.Length <= 0)
                {
                    Debug.LogError("アクティブになっているanimationEventReceiversが検出されませんでした。 OriginalSheetオブジェクト以下のSheetアニメーションは1つだけアクティブになっている必要があります。 割り当てられている_animationEventReceiversはInspector上で確認できます。");
                }
                else
                {
                    Debug.LogError("アクティブになっているanimationEventReceiversが複数検出さました。 OriginalSheetオブジェクト以下のSheetアニメーションは1つだけアクティブになっている必要があります。 割り当てられている_animationEventReceiversはInspector上で確認できます。");
                }
                return;
            }

            foreach (var eventReceivers in _animationEventReceivers)
            {
                eventReceivers.AnimationEventOnFrameCallback = AnimationEventOnFrame;
            }

            _isAnimationCapturing = false;
            _isFrameCapturing = false;
            _hasNextFrameCapture = false;
        }

        void LateUpdate()
        {
            if (_hasNextFrameCapture && !_isFrameCapturing)
            {
                _hasNextFrameCapture = false;
                _isFrameCapturing = true;
                StartCoroutine(CaptureFrame());
            }
        }

        void OnStartAnimationCapture()
        {
            Time.timeScale = TIME_SCALE_ON_CAPTURING;
            _capturedFrames = new Dictionary<int, Texture2D>();
            if (_hideReferenceAnimationOnCapture)
            {
                _referenceAnimation.transform.position = REFERENCE_ANIMATION_POSITION_ON_CAPTURING;
            }
            if (_transparentBackground)
            {
                _background.SetActive(false);
            }
        }

        void OnEndAnimationCapture()
        {
            if (_capturedFrames.Count != _frameCountX * _frameCountY)
            {
                Debug.LogError($"総フレーム数({_frameCountX} x {_frameCountY} → {_frameCountX * _frameCountY})とは異なるフレーム数({_capturedFrames.Count})が検出されました。");
            }

            CombineTextures();

            foreach (var tex in _capturedFrames.Values)
            {
                Destroy(tex);
            }
            _capturedFrames = null;
            Time.timeScale = 1f;
            _referenceAnimation.transform.position = Vector3.zero;
            _background.SetActive(true);

            _isAnimationCapturing = false;
        }

        void AnimationEventOnFrame(int i)
        {
            if (!_isAnimationCapturing)
            {
                return;
            }

            if (i == 1)
            {
                if (_capturedFrames == null)
                {
                    OnStartAnimationCapture();
                }
                else
                {
                    OnEndAnimationCapture();
                    return;
                }
            }
            else if (_capturedFrames == null)
            {
                return;
            }

            if (_frameCountX * _frameCountY < i)
            {
                Debug.LogError($"総フレーム数 ({_frameCountX} x {_frameCountY} → {_frameCountX * _frameCountY})よりも多いフレーム({i})が検出されました。");
                OnEndAnimationCapture();
                return;
            }

            if (_capturedFrames.ContainsKey(i))
            {
                Debug.LogError("重複してフレームが撮影されようとしました。何かエラーが起きています。");
                return;
            }

            if (!_hasNextFrameCapture)
            {
                _capturedFrames[i] = null;
                _currentFrame = i;
                _hasNextFrameCapture = true;
            }
        }

        IEnumerator CaptureFrame()
        {
            yield return new WaitForEndOfFrame();

            var tex = ScreenCapture.CaptureScreenshotAsTexture(SUPER_SIZE);
            if (tex.width != _frameWidth || tex.height != _frameHeight)
            {
                Debug.LogWarning($"フレームサイズを拡大縮小します。品質を落としたくない場合はGameViewのDisplayで {_frameWidth}x{_frameHeight} に設定してください。 撮影サイズ: {tex.width}x{tex.height}、変換後フレームサイズ: {_frameWidth}x{_frameHeight}");

                TextureScale.Bilinear(tex, _frameWidth, _frameHeight);
            }

            if (_capturedFrames.ContainsKey(_currentFrame))
            {
                _capturedFrames[_currentFrame] = tex;
            }

            if (_saveEachFrame)
            {
                if (Utils.GetFrameSavePath(_currentFrame, out var path))
                {
                    File.WriteAllBytes(path, tex.EncodeToPNG());
                }
            }

            _isFrameCapturing = false;
        }

        void CombineTextures()
        {
            var combinedTex = Utils.CombineFrameTextures(_capturedFrames, _frameCountX, _frameCountY);

            if (_pileOnReferenece && _refereneceTexture != null)
            {
                if (!_refereneceTexture.isReadable)
                {
                    _refereneceTexture = Utils.ReadTexture(_refereneceTexture);
                }

                if (PileTextures.Pile(_refereneceTexture, combinedTex, out var resultTex))
                {
                    if (Utils.GetSavePathOnDesktopWithDate(out var path))
                    {
                        File.WriteAllBytes(path, resultTex.EncodeToPNG());
                    }
                }
            }
            else
            {
                if (Utils.GetSavePathOnDesktopWithDate(out var path))
                {
                    File.WriteAllBytes(path, combinedTex.EncodeToPNG());
                }
            }
        }
    }
}
