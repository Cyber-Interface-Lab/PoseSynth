using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// VRに限らず一般的に使用可能なカメラリグを表すクラスです。
    /// This class represents a camera rig that can be used in general, not limited to VR.
    /// </summary>
    public class SimpleLocalCameraRig : LocalCameraRig
    {
        public override CameraRigType Type => CameraRigType.Simple;
    }
}