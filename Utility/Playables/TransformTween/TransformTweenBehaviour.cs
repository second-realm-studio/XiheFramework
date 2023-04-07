using System;
using UnityEngine;
using UnityEngine.Playables;

namespace XiheFramework.Utility.Playables.TransformTween {
    [Serializable]
    public class TransformTweenBehaviour : PlayableBehaviour {
        public enum TweenType {
            Linear,
            Deceleration,
            Harmonic,
            Custom
        }

        private const float k_RightAngleInRads = Mathf.PI * 0.5f;

        public Transform startLocation;
        public Transform endLocation;
        public bool tweenPosition = true;
        public bool tweenRotation = true;
        public TweenType tweenType;
        public AnimationCurve customCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public Vector3 startingPosition;
        public Quaternion startingRotation = Quaternion.identity;

        private AnimationCurve m_DecelerationCurve = new(
            new Keyframe(0f, 0f, -k_RightAngleInRads, k_RightAngleInRads),
            new Keyframe(1f, 1f, 0f, 0f)
        );

        private AnimationCurve m_HarmonicCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private AnimationCurve m_LinearCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public override void PrepareFrame(Playable playable, FrameData info) {
            if (startLocation) {
                startingPosition = startLocation.position;
                startingRotation = startLocation.rotation;
            }
        }

        public float EvaluateCurrentCurve(float time) {
            if (tweenType == TweenType.Custom && !IsCustomCurveNormalised()) {
                Debug.LogError("Custom Curve is not normalised.  Curve must start at 0,0 and end at 1,1.");
                return 0f;
            }

            switch (tweenType) {
                case TweenType.Linear:
                    return m_LinearCurve.Evaluate(time);
                case TweenType.Deceleration:
                    return m_DecelerationCurve.Evaluate(time);
                case TweenType.Harmonic:
                    return m_HarmonicCurve.Evaluate(time);
                default:
                    return customCurve.Evaluate(time);
            }
        }

        private bool IsCustomCurveNormalised() {
            if (!Mathf.Approximately(customCurve[0].time, 0f))
                return false;

            if (!Mathf.Approximately(customCurve[0].value, 0f))
                return false;

            if (!Mathf.Approximately(customCurve[customCurve.length - 1].time, 1f))
                return false;

            return Mathf.Approximately(customCurve[customCurve.length - 1].value, 1f);
        }
    }
}