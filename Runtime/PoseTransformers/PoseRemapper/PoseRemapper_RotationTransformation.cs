using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CyberInterfaceLab.PoseSynth.Utilities;

namespace CyberInterfaceLab.PoseSynth
{

    /// <summary>
    /// Rotate the target joint based on the rotation of the reference joint.
    /// </summary>
    public class PoseRemapper_RotationTransformation : PoseRemapper, IObservable<PoseRemapper_RotationTransformation>
    {
        /// <summary>
        /// A pair of two bones.
        /// </summary>
        [System.Serializable]
        public struct Mapping
        {
            /// <summary>
            /// The reference bone.
            /// </summary>
            public Transform BoneBefore;
            /// <summary>
            /// The result bone.
            /// It rotates based on the rotation of BoneBefore.
            /// </summary>
            public Transform BoneAfter;
            /// <summary>
            /// A of the equation Q' = AQ + B.
            /// </summary>
            public Quaternion Rotation;
            /// <summary>
            /// B of the equation Q' = AQ + B.
            /// </summary>
            public Quaternion Offset;

            public void Remap()
            {
                BoneAfter.localRotation = Multiply(BoneBefore.localRotation, Rotation) * Offset;
            }
        }
        public List<Mapping> Mappings;

        #region observable
        HashSet<IObserver<PoseRemapper_RotationTransformation>> m_observers = new(8);
        public void AddObserver(IObserver<PoseRemapper_RotationTransformation> observer) => m_observers.Add(observer);
        public void RemoveObserver(IObserver<PoseRemapper_RotationTransformation> observer) => m_observers.Remove(observer);
        public override void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        protected override void RemapOnUpdate()
        {
            foreach (var mapping in Mappings)
            {
                mapping.Remap();
            }
        }
    }
}