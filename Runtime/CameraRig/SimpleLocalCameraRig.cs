using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public class SimpleLocalCameraRig : LocalCameraRig
    {
        public override CameraRigType Type => CameraRigType.Simple;
    }
}