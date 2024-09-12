using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CyberInterfaceLab.PoseSynth.Utilities;

namespace CyberInterfaceLab.PoseSynth
{

    /// <summary>
    /// Rotate the target bone based on the rotation of the reference bone.
    /// </summary>
    public class PoseRemapper_RotationTransformation : PoseRemapper
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

        protected override void RemapOnUpdate()
        {
            foreach (var mapping in Mappings)
            {
                mapping.Remap();
            }
        }
    }
}