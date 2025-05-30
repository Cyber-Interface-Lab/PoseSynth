using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Add offset to the pose.
    /// </summary>
    public class PoseOffsetter : PoseRemapper, IObservable<PoseOffsetter>
    {
        #region enum
        public enum OffsetType
        {
            None,
            Global,
            Local,
        }
        #endregion

        #region public variable
        public OffsetType Position;
        public OffsetType Rotation;
        #endregion

        #region private variable
        [SerializeField]
        protected Pose m_offset;
        #endregion

        #region observable
        HashSet<IObserver<PoseOffsetter>> m_observers = new(8);
        public void AddObserver(IObserver<PoseOffsetter> observer) => m_observers.Add(observer);
        public void RemoveObserver(IObserver<PoseOffsetter> observer) => m_observers.Remove(observer);
        public override void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        #region public method
        protected override void RemapOnUpdate()
        {
            var Bones = m_target.Contents;
            for (int i = 0; i < Bones.Count; i++)
            {
                var refBone = m_reference.Contents[i].transform;
                var offsetBone = m_offset.Contents[i].transform;
                var bone = Bones[i].transform;

                switch (Position)
                {
                    default:
                    case OffsetType.None:
                        bone.transform.localPosition = refBone.localPosition;
                        break;
                    case OffsetType.Local:
                        bone.transform.localPosition = refBone.localPosition + offsetBone.localPosition;
                        break;
                    case OffsetType.Global:
                        bone.transform.position = refBone.position + offsetBone.position;
                        break;
                }
                switch (Rotation)
                {
                    default:
                    case OffsetType.None:
                        bone.transform.localRotation = refBone.localRotation;
                        break;
                    case OffsetType.Local:
                        bone.transform.localRotation = offsetBone.localRotation * refBone.localRotation;
                        break;
                    case OffsetType.Global:
                        bone.transform.rotation = offsetBone.rotation * refBone.rotation;
                        break;
                }
            }
        }
        #endregion
    }
}