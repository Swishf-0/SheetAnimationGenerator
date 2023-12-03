using System;
using UnityEngine;

namespace SheetAnimationGenerator
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        public Action<int> AnimationEventOnFrameCallback { get; set; }

        void AnimationEventOnFrame(int i)
        {
            AnimationEventOnFrameCallback?.Invoke(i);
        }
    }
}
