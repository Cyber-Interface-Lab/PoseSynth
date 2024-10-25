using Unity.VisualScripting;
using UnityEngine;
using static OVRSkeleton.BoneId;
using static CyberInterfaceLab.PoseSynth.TrackerType;

namespace CyberInterfaceLab.PoseSynth.VR
{
    /// <summary>
    /// Custom CameraRig for OVR.
    /// Attach it to OVR CameraRig and press the Initialize button.
    /// </summary>
    public class OVRLocalCameraRig : CustomLocalCameraRig
    {
        [SerializeField]
        private OVRCameraRig m_ovrCameraRig;
        private OVRCustomSkeleton[] m_hands;
        [SerializeField]
        private OVRCustomSkeleton m_handLeft;
        [SerializeField]
        private OVRCustomSkeleton m_handRight;

        public override CameraRigType Type => CameraRigType.OVR;

        // set anchors because they get unassigned when entering play mode.
        private void SetAnchors()
        {
            if (m_ovrCameraRig == null) { return; }

            if (m_trackerTransform[Head] == null)
            {
                m_trackerTransform[Head] = m_ovrCameraRig.centerEyeAnchor;
            }
            if (m_trackerTransform[HandLeft] == null)
            {
                m_trackerTransform[HandLeft] = m_ovrCameraRig.leftHandAnchor;
            }
            if (m_trackerTransform[ControllerLeft] == null)
            {
                m_trackerTransform[ControllerLeft] = m_ovrCameraRig.leftControllerAnchor;
            }
            if (m_trackerTransform[HandRight] == null)
            {
                m_trackerTransform[HandRight] = m_ovrCameraRig.rightHandAnchor;
            }
            if (m_trackerTransform[ControllerRight] == null)
            {
                m_trackerTransform[ControllerRight] = m_ovrCameraRig.rightControllerAnchor;
            }
        }

        private void OnValidate()
        {
            SetAnchors();
        }
        private void Awake()
        {
            SetAnchors();
        }
        public override void Initialize()
        {
            m_ovrCameraRig = GetComponent<OVRCameraRig>();
            m_hands = GetComponentsInChildren<OVRCustomSkeleton>();
            for (int i = 0; i < m_hands.Length; i++)
            {
                if (m_hands[i].GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft)
                {
                    m_handLeft = m_hands[i];
                    continue;
                }
                if (m_hands[i].GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight)
                {
                    m_handRight = m_hands[i];
                    continue;
                }
            }

            if (m_ovrCameraRig == null) { return; }

            // attach transform of OVRCameraRig
            m_trackerTransform = new TrackerDictionary
            {
                // Anchors
                { Head, m_ovrCameraRig.centerEyeAnchor },
                { HandLeft, m_ovrCameraRig.leftHandAnchor },
                { ControllerLeft, m_ovrCameraRig.leftControllerAnchor },
                { HandRight, m_ovrCameraRig.rightHandAnchor },
                { ControllerRight, m_ovrCameraRig.rightControllerAnchor },
            };

            if (m_handLeft != null)
            {
                var bones = m_handLeft.CustomBones;
                m_trackerTransform.AddRange(new TrackerDictionary
                {
                    // wrist
                    { HandWristRootLeft, bones[(int)Hand_WristRoot] },
                    { HandForearmStubLeft, bones[(int) Hand_ForearmStub] },

                    // thumb
                    { Thumb0Left, bones[(int) Hand_Thumb0] },
                    { Thumb1Left, bones[(int) Hand_Thumb1] },
                    { Thumb2Left, bones[(int) Hand_Thumb2] },
                    { Thumb3Left, bones[(int) Hand_Thumb3] },

                    // index
                    { Index1Left, bones[(int) Hand_Index1] },
                    { Index2Left, bones[(int) Hand_Index2] },
                    { Index3Left, bones[(int) Hand_Index3] },

                    // middle
                    { Middle1Left, bones[(int) Hand_Middle1] },
                    { Middle2Left, bones[(int) Hand_Middle2] },
                    { Middle3Left, bones[(int) Hand_Middle3] },

                    // ring
                    { Ring1Left, bones[(int) Hand_Ring1] },
                    { Ring2Left, bones[(int) Hand_Ring2] },
                    { Ring3Left, bones[(int) Hand_Ring3] },

                    // pinky
                    { Pinky0Left, bones[(int) Hand_Pinky0] },
                    { Pinky1Left, bones[(int) Hand_Pinky1] },
                    { Pinky2Left, bones[(int) Hand_Pinky2] },
                    { Pinky3Left, bones[(int) Hand_Pinky3] },
                });
            }

            if (m_handRight != null)
            {
                var bones = m_handRight.CustomBones;
                m_trackerTransform.AddRange(new TrackerDictionary
                {
                    // wrist
                    { HandWristRootRight, bones[(int) Hand_WristRoot] },
                    { HandForearmStubRight, bones[(int) Hand_ForearmStub] },

                    // thumb
                    { Thumb0Right, bones[(int) Hand_Thumb0] },
                    { Thumb1Right, bones[(int) Hand_Thumb1] },
                    { Thumb2Right, bones[(int) Hand_Thumb2] },
                    { Thumb3Right, bones[(int) Hand_Thumb3] },

                    // index
                    { Index1Right, bones[(int) Hand_Index1] },
                    { Index2Right, bones[(int) Hand_Index2] },
                    { Index3Right, bones[(int) Hand_Index3] },

                    // middle
                    { Middle1Right, bones[(int) Hand_Middle1] },
                    { Middle2Right, bones[(int) Hand_Middle2] },
                    { Middle3Right, bones[(int) Hand_Middle3] },

                    // ring
                    { Ring1Right, bones[(int) Hand_Ring1] },
                    { Ring2Right, bones[(int) Hand_Ring2] },
                    { Ring3Right, bones[(int) Hand_Ring3] },

                    // pinky
                    { Pinky0Right, bones[(int) Hand_Pinky0] },
                    { Pinky1Right, bones[(int) Hand_Pinky1] },
                    { Pinky2Right, bones[(int) Hand_Pinky2] },
                    { Pinky3Right, bones[(int) Hand_Pinky3] },
                });
            }
        }
    }
}
