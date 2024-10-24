using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;
using static CyberInterfaceLab.PoseSynth.TrackerType;
using static UnityEngine.XR.Hands.XRHandJointID;

namespace CyberInterfaceLab.PoseSynth.VR
{
    /// <summary>
    /// Custom CameraRig for XR Interaction Toolkit (XROrigin prefab) with XRHands.
    /// Attach it to XROrigin and press the Initialize button.
    /// </summary>
    public class XRIHandsLocalCameraRig : XRILocalCameraRig
    {
        #region private variable
        // Hand Tracking
        private XRHandSkeletonDriver[] m_hands;
        [SerializeField]
        private XRHandSkeletonDriver m_handLeft;
        [SerializeField]
        private XRHandSkeletonDriver m_handRight;
        #endregion

        #region public method
        public override void Initialize()
        {
            // get xr components
            m_xrOrigin = GetComponent<XROrigin>();
            m_controllers = GetComponentsInChildren<ActionBasedController>();
            m_hands = GetComponentsInChildren<XRHandSkeletonDriver>();

            // select left and right controller
            m_controllerLeft = m_controllers
                .Where(x => x.name.Contains("Left Controller"))
                .FirstOrDefault();
            m_controllerRight = m_controllers
                .Where(x => x.name.Contains("Right Controller"))
                .FirstOrDefault();

            // select left and right hand
            m_handLeft = m_hands
                .Where(x => x.name.Contains("Left Hand"))
                .FirstOrDefault();
            m_handRight = m_hands
                .Where(x => x.name.Contains("Right Hand"))
                .FirstOrDefault();

            m_trackerTransform = new TrackerDictionary
            {
                // CameraRig original
                { Head, m_xrOrigin.Camera.transform },
                { ControllerLeft, m_controllerLeft.transform },
                { ControllerRight, m_controllerRight.transform },

                // Hand Tracking Left
                { HandLeft, GetHandJointTransform(m_handLeft, BeginMarker) },
                { HandWristRootLeft, GetHandJointTransform(m_handLeft, BeginMarker) },
                // Thumb
                { Thumb1Left, GetHandJointTransform(m_handLeft, ThumbMetacarpal) },
                { Thumb2Left, GetHandJointTransform(m_handLeft, ThumbProximal) },
                { Thumb3Left, GetHandJointTransform(m_handLeft, ThumbDistal) },
                // Index
                { Index1Left, GetHandJointTransform(m_handLeft, IndexProximal) },
                { Index2Left, GetHandJointTransform(m_handLeft, IndexIntermediate) },
                { Index3Left, GetHandJointTransform(m_handLeft, IndexDistal) },
                // Middle
                { Middle1Left, GetHandJointTransform(m_handLeft, MiddleProximal) },
                { Middle2Left, GetHandJointTransform(m_handLeft, MiddleIntermediate) },
                { Middle3Left, GetHandJointTransform(m_handLeft, MiddleDistal) },
                // Ring
                { Ring1Left, GetHandJointTransform(m_handLeft, RingProximal) },
                { Ring2Left, GetHandJointTransform(m_handLeft, RingIntermediate) },
                { Ring3Left, GetHandJointTransform(m_handLeft, RingDistal) },
                // Pinky
                { Pinky1Left, GetHandJointTransform(m_handLeft, LittleProximal) },
                { Pinky2Left, GetHandJointTransform(m_handLeft, LittleIntermediate) },
                { Pinky3Left, GetHandJointTransform(m_handLeft, LittleDistal) },

                // Hand Tracking Right
                { HandRight, GetHandJointTransform(m_handRight, BeginMarker) },
                { HandWristRootRight, GetHandJointTransform(m_handRight, BeginMarker) },
                // Thumb
                { Thumb1Right, GetHandJointTransform(m_handRight, ThumbMetacarpal) },
                { Thumb2Right, GetHandJointTransform(m_handRight, ThumbProximal) },
                { Thumb3Right, GetHandJointTransform(m_handRight, ThumbDistal) },
                // Index
                { Index1Right, GetHandJointTransform(m_handRight, IndexProximal) },
                { Index2Right, GetHandJointTransform(m_handRight, IndexIntermediate) },
                { Index3Right, GetHandJointTransform(m_handRight, IndexDistal) },
                // Middle
                { Middle1Right, GetHandJointTransform(m_handRight, MiddleProximal) },
                { Middle2Right, GetHandJointTransform(m_handRight, MiddleIntermediate) },
                { Middle3Right, GetHandJointTransform(m_handRight, MiddleDistal) },
                // Ring
                { Ring1Right, GetHandJointTransform(m_handRight, RingProximal) },
                { Ring2Right, GetHandJointTransform(m_handRight, RingIntermediate) },
                { Ring3Right, GetHandJointTransform(m_handRight, RingDistal) },
                // Pinky
                { Pinky1Right, GetHandJointTransform(m_handRight, LittleProximal) },
                { Pinky2Right, GetHandJointTransform(m_handRight, LittleIntermediate) },
                { Pinky3Right, GetHandJointTransform(m_handRight, LittleDistal) },
            };
        }
        #endregion
        #region private method
        private Transform GetHandJointTransform(XRHandSkeletonDriver hand, XRHandJointID jointID)
        {
            return hand.jointTransformReferences.Find(x => x.xrHandJointID == jointID).jointTransform;
        }
        #endregion
    }
}
