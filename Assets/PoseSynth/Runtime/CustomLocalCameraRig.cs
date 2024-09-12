using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// If you want to link PoseEditor with VR packages like Oculus Integraton,
    /// You have to create new CameraRig wrapper for it.
    /// Inherit this abstract class so that you can initialize it easily.
    /// </summary>
    public abstract class CustomLocalCameraRig : LocalCameraRig
    {
        /// <summary>
        /// Initialize the dictionary m_trackerTransform.
        /// You can call this by a button in the Inspector.
        /// </summary>
        public abstract void Initialize();
    }
}
