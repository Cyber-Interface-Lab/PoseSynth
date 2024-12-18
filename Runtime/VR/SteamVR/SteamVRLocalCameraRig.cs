using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public class SteamVRLocalCameraRig : CustomLocalCameraRig
    {
        public override CameraRigType Type => CameraRigType.SteamVR;

        public override void Initialize()
        {
            // none
        }
    }
}
