using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// VRプラグインごとに異なる<see cref="ServerCameraRig"/>のPrefabを選択するためのenumです。
    /// Enum to select the prefab of <see cref="ServerCameraRig"/> for each VR plugin.
    /// </summary>
    public enum CameraRigType
    {
        /// <summary>
        /// Simple (non-VR) CameraRig
        /// </summary>
        Simple = 0,
        /// <summary>
        /// OVR CameraRig
        /// </summary>
        OVR = 1,
        /// <summary>
        /// XR Interaction Toolkit CameraRig
        /// </summary>
        XRI = 2,
        /// <summary>
        /// XR Interaction Toolkit CameraRig with hand tracking
        /// </summary>
        XRIHands = 3,
    }
}