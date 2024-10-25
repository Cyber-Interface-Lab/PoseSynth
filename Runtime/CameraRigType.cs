using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// CameraRig type.
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