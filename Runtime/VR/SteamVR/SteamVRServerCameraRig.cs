using CyberInterfaceLab.PoseSynth.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.VR
{
    public class SteamVRServerCameraRig : ServerCameraRig
    {
        public override CameraRigType Type => CameraRigType.SteamVR;
    }
}
