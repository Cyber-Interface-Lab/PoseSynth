using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static CyberInterfaceLab.PoseSynth.TrackerType;

namespace CyberInterfaceLab.PoseSynth.VR
{
    /// <summary>
    /// Custom CameraRig for XR Interaction Toolkit (XROrigin prefab).
    /// Attach it to XROrigin and press the Initialize button.
    /// </summary>
    public class XRILocalCameraRig : CustomLocalCameraRig
    {
        public override CameraRigType Type => CameraRigType.XRI;

        #region protected variable
        [SerializeField]
        protected XROrigin m_xrOrigin;
        protected ActionBasedController[] m_controllers;
        [SerializeField]
        protected ActionBasedController m_controllerLeft;
        [SerializeField]
        protected ActionBasedController m_controllerRight;
        #endregion

        #region public method
        public override void Initialize()
        {
            // get xr components
            m_xrOrigin = GetComponent<XROrigin>();
            m_controllers = GetComponentsInChildren<ActionBasedController>();

            // select left and right controller
            m_controllerLeft = m_controllers
                .Where(x => x.name.Contains("Left Controller"))
                .FirstOrDefault();
            m_controllerRight = m_controllers
                .Where(x => x.name.Contains("Right Controller"))
                .FirstOrDefault();

            m_trackerTransform = new TrackerDictionary
            {
                // CameraRig original
                { Head, m_xrOrigin.Camera.transform },
                { HandLeft, m_controllerLeft.transform },
                { ControllerLeft, m_controllerLeft.transform },
                { HandRight, m_controllerRight.transform },
                { ControllerRight, m_controllerRight.transform },
            };
        }
        #endregion
    }
}
