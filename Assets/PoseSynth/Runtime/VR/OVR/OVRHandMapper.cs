using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.VR
{
    /// <summary>
    /// Map Hand Tracking to Hand Pose.
    /// </summary>
    public class OVRHandMapper : HandMapper
    {
        [SerializeField] private Finger WristRoot;
        [SerializeField] private Finger ForearmStub;
        [SerializeField] private DoubleFinger Thumb1;
        [SerializeField] private Finger Thumb2;
        [SerializeField] private Finger Thumb3;
        [SerializeField] private Finger Index1;
        [SerializeField] private Finger Index2;
        [SerializeField] private Finger Index3;
        [SerializeField] private Finger Middle1;
        [SerializeField] private Finger Middle2;
        [SerializeField] private Finger Middle3;
        [SerializeField] private Finger Ring1;
        [SerializeField] private Finger Ring2;
        [SerializeField] private Finger Ring3;
        [SerializeField] private DoubleFinger Pinky1;
        [SerializeField] private Finger Pinky2;
        [SerializeField] private Finger Pinky3;

        private HashSet<Finger> m_fingers;
        private HashSet<DoubleFinger> m_doubleFingers;

        private void SetProperties(Finger f)
        {
            if (f.Target == null) { return; }
            if (m_cameraRig.TryGetTransform(f.Type, out var t))
            {
                f.Target.localRotation = Utilities.Multiply(t.localRotation, f.Offset);
            }
        }
        private void SetProperties(DoubleFinger df)
        {
            if (df.Target == null) { return; }
            if (m_cameraRig.TryGetTransform(df.Type0, out var t0) && m_cameraRig.TryGetTransform(df.Type1, out var t1))
            {
                var offset0 = Utilities.Multiply(t0.localRotation, df.Offset0);
                var offset1 = Utilities.Multiply(t1.localRotation, df.Offset1);
                df.Target.localRotation = offset0 * offset1;
            }
        }
        protected override void Initialize()
        {
            // set hand type
            WristRoot.Type = Type == HandType.Left ? TrackerType.HandWristRootLeft : TrackerType.HandWristRootRight;
            ForearmStub.Type = Type == HandType.Left ? TrackerType.HandForearmStubLeft : TrackerType.HandForearmStubRight;
            Thumb1.Type0 = Type == HandType.Left ? TrackerType.Thumb0Left : TrackerType.Thumb0Right;
            Thumb1.Type1 = Type == HandType.Left ? TrackerType.Thumb1Left : TrackerType.Thumb1Right;
            Thumb2.Type = Type == HandType.Left ? TrackerType.Thumb2Left : TrackerType.Thumb2Right;
            Thumb3.Type = Type == HandType.Left ? TrackerType.Thumb3Left : TrackerType.Thumb3Right;
            Index1.Type = Type == HandType.Left ? TrackerType.Index1Left : TrackerType.Index1Right;
            Index2.Type = Type == HandType.Left ? TrackerType.Index2Left : TrackerType.Index2Right;
            Index3.Type = Type == HandType.Left ? TrackerType.Index3Left : TrackerType.Index3Right;
            Middle1.Type = Type == HandType.Left ? TrackerType.Middle1Left : TrackerType.Middle1Right;
            Middle2.Type = Type == HandType.Left ? TrackerType.Middle2Left : TrackerType.Middle2Right;
            Middle3.Type = Type == HandType.Left ? TrackerType.Middle3Left : TrackerType.Middle3Right;
            Ring1.Type = Type == HandType.Left ? TrackerType.Ring1Left : TrackerType.Ring1Right;
            Ring2.Type = Type == HandType.Left ? TrackerType.Ring2Left : TrackerType.Ring2Right;
            Ring3.Type = Type == HandType.Left ? TrackerType.Ring3Left : TrackerType.Ring3Right;
            Pinky1.Type0 = Type == HandType.Left ? TrackerType.Pinky0Left : TrackerType.Pinky0Right;
            Pinky1.Type1 = Type == HandType.Left ? TrackerType.Pinky1Left : TrackerType.Pinky1Right;
            Pinky2.Type = Type == HandType.Left ? TrackerType.Pinky2Left : TrackerType.Pinky2Right;
            Pinky3.Type = Type == HandType.Left ? TrackerType.Pinky3Left : TrackerType.Pinky3Right;

            // set hand target
            var animator = GetComponentInChildren<Animator>();
            var avatar = animator.avatar;
            if (avatar.isHuman)
            {
                WristRoot.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
                Thumb1.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftThumbProximal : HumanBodyBones.RightThumbProximal);
                Thumb2.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftThumbIntermediate : HumanBodyBones.RightThumbIntermediate);
                Thumb3.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal);
                Index1.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftIndexProximal : HumanBodyBones.RightIndexProximal);
                Index2.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftIndexIntermediate : HumanBodyBones.RightIndexIntermediate);
                Index3.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal);
                Middle1.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftMiddleProximal : HumanBodyBones.RightMiddleProximal);
                Middle2.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftMiddleIntermediate : HumanBodyBones.RightMiddleIntermediate);
                Middle3.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal);
                Ring1.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftRingProximal : HumanBodyBones.RightRingProximal);
                Ring2.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftRingIntermediate : HumanBodyBones.RightRingIntermediate);
                Ring3.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal);
                Pinky1.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftLittleProximal : HumanBodyBones.RightLittleProximal);
                Pinky2.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftLittleIntermediate : HumanBodyBones.RightLittleIntermediate);
                Pinky3.Target = animator.GetBoneTransform(Type == HandType.Left ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal);
            }

            // create hashset
            m_fingers = new(16)
            {
                //WristRoot,
                //ForearmStub,
                Thumb2,
                Thumb3,
                Index1,
                Index2,
                Index3,
                Middle1,
                Middle2,
                Middle3,
                Ring1,
                Ring2,
                Ring3,
                Pinky2,
                Pinky3,
            };
            m_doubleFingers = new(4)
            {
                Thumb1,
                Pinky1,
            };
        }
        /// <summary>
        /// Initialize the offset value of each finger joint.
        /// </summary>
        public override void InitializeOffset()
        {
            if (Type == HandType.Left)
            {
                WristRoot.Offset = Quaternion.Euler(0f, 0f, 0f);
                ForearmStub.Offset = Quaternion.Euler(0f, 0f, 0f);
                Thumb1.Offset0 = Quaternion.Euler(315f, 180f, 225f);
                Thumb1.Offset1 = Quaternion.Euler(270f, 135f, 0f);
                Thumb2.Offset = Quaternion.Euler(0f, 180f, 270f);
                Thumb3.Offset = Quaternion.Euler(0f, 180f, 270f);
                Index1.Offset = Quaternion.Euler(270f, 90f, 0f);
                Index2.Offset = Quaternion.Euler(270f, 90f, 0f);
                Index3.Offset = Quaternion.Euler(270f, 90f, 0f);
                Middle1.Offset = Quaternion.Euler(270f, 90f, 0f);
                Middle2.Offset = Quaternion.Euler(270f, 90f, 0f);
                Middle3.Offset = Quaternion.Euler(270f, 90f, 0f);
                Ring1.Offset = Quaternion.Euler(270f, 90f, 0f);
                Ring2.Offset = Quaternion.Euler(270f, 90f, 0f);
                Ring3.Offset = Quaternion.Euler(270f, 90f, 0f);
                Pinky1.Offset0 = Quaternion.Euler(270f, 150f, 0f);
                Pinky1.Offset1 = Quaternion.Euler(300f, 355f, 90f);
                Pinky2.Offset = Quaternion.Euler(270f, 90f, 0f);
                Pinky3.Offset = Quaternion.Euler(270f, 90f, 0f);
            }
            else
            {
                WristRoot.Offset = Quaternion.Euler(0f, 0f, 0f);
                ForearmStub.Offset = Quaternion.Euler(0f, 0f, 0f);
                Thumb1.Offset0 = Quaternion.Euler(45f, 180f, 225f);
                Thumb1.Offset1 = Quaternion.Euler(90f, 135f, 0f);
                Thumb2.Offset = Quaternion.Euler(0f, 0f, 270f);
                Thumb3.Offset = Quaternion.Euler(0f, 0f, 270f);
                Index1.Offset = Quaternion.Euler(90f, 90f, 0f);
                Index2.Offset = Quaternion.Euler(90f, 90f, 0f);
                Index3.Offset = Quaternion.Euler(90f, 90f, 0f);
                Middle1.Offset = Quaternion.Euler(90f, 90f, 0f);
                Middle2.Offset = Quaternion.Euler(90f, 90f, 0f);
                Middle3.Offset = Quaternion.Euler(90f, 90f, 0f);
                Ring1.Offset = Quaternion.Euler(90f, 90f, 0f);
                Ring2.Offset = Quaternion.Euler(90f, 90f, 0f);
                Ring3.Offset = Quaternion.Euler(90f, 90f, 0f);
                Pinky1.Offset0 = Quaternion.Euler(90f, 150f, 0f);
                Pinky1.Offset1 = Quaternion.Euler(60f, 175f, 90f);
                Pinky2.Offset = Quaternion.Euler(90f, 90f, 0f);
                Pinky3.Offset = Quaternion.Euler(90f, 90f, 0f);
            }
        }

        protected override void MapOnUpdate()
        {
            foreach (var f in m_fingers)
            {
                if (f.HasTarget)
                {
                    SetProperties(f);
                }
            }

            foreach (var f in m_doubleFingers)
            {
                if (f.HasTarget)
                {
                    SetProperties(f);
                }
            }
        }
    }
}
