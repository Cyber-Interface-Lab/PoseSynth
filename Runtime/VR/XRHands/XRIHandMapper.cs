using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.VR
{
    /// <summary>
    /// Map Hand Tracking to Hand Pose.
    /// </summary>
    public class XRIHandMapper : HandMapper, IObservable<XRIHandMapper>
    {
        #region private variable
        [SerializeField] private Finger WristRoot;
        [SerializeField] private Finger Thumb1;
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
        [SerializeField] private Finger Pinky1;
        [SerializeField] private Finger Pinky2;
        [SerializeField] private Finger Pinky3;

        private HashSet<Finger> m_fingers;
        HashSet<IObserver<XRIHandMapper>> observers = new(16);
        #endregion

        #region observable
        public void AddObserver(IObserver<XRIHandMapper> observer) => observers.Add(observer);
        public void RemoveObserver(IObserver<XRIHandMapper> observer) => observers.Remove(observer);
        public override void Notify()
        {
            foreach (var observer in observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        #region private method
        private void SetProperties(Finger f)
        {
            if (f.Target == null) { return; }
            if (m_reference.TryGetTransform(f.Type, out var t))
            {
                f.Target.localRotation = Utilities.Multiply(t.localRotation, f.Offset);
            }
        }
        protected override void Initialize()
        {
            // set hand type
            WristRoot.Type = Type == HandType.Left ? TrackerType.HandWristRootLeft : TrackerType.HandWristRootRight;
            Thumb1.Type = Type == HandType.Left ? TrackerType.Thumb1Left : TrackerType.Thumb1Right;
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
            Pinky1.Type = Type == HandType.Left ? TrackerType.Pinky1Left : TrackerType.Pinky1Right;
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
                Thumb1, Thumb2, Thumb3,
                Index1, Index2, Index3,
                Middle1, Middle2, Middle3,
                Ring1, Ring2, Ring3,
                Pinky1, Pinky2, Pinky3,
            };
        }
        public override void InitializeOffset()
        {
            throw new NotImplementedException();
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
        }
        #endregion
    }
}
