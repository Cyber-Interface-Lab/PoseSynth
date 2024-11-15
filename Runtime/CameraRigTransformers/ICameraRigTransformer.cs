using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Interface who can transform a <see cref="ICameraRig"/>.
    /// </summary>
    public interface ICameraRigTransformer
    {
        /// <summary>
        /// Can update <see cref="Target"/> if true.
        /// </summary>
        bool IsValid { get; set; }
        /// <summary>
        /// Target <see cref="ICameraRig"/> to be transformed.
        /// </summary>
        ICameraRig Target { get; set; }
    }
}
