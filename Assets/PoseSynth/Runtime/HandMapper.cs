using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public abstract class HandMapper : PoseMapper
    {
        /// <summary>
        /// Joint of a pose's finger.
        /// </summary>
        [Serializable]
        public struct Finger
        {
            public TrackerType Type;
            public Quaternion Offset;
            public Transform Target;
            public bool HasTarget => Target != null;
        }
        /// <summary>
        /// Joint of a pose's finger.
        /// Determine the local rotation using information from 2 trackers.
        /// </summary>
        [Serializable]
        public struct DoubleFinger
        {
            public TrackerType Type0;
            public Quaternion Offset0;

            public TrackerType Type1;
            public Quaternion Offset1;

            public Transform Target;
            public bool HasTarget => Target != null;
        }
        public enum HandType
        {
            Left, Right,
        }
        public HandType Type;

        #region public method
        /// <summary>
        /// Initialize offset of each finger joint.
        /// </summary>
        public virtual void InitializeOffset() { }
        #endregion

        #region private method
        /// <summary>
        /// Initialize this instance.
        /// Set Type and Target of each finger joint.
        /// Called in OnValidate() and Awake().
        /// </summary>
        protected abstract void Initialize();
        #endregion

        protected override void OnValidate()
        {
            base.OnValidate();
            Initialize();
        }
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }
    }
}
