using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace CyberInterfaceLab.PoseSynth
{
    public abstract class IKMapper : PoseMapper
    {
        public override ICameraRig CameraRig
        {
            set
            {
                SetCameraRigWithoutNotice(value);
                Notify();
            }
        }
        public override void SetCameraRigWithoutNotice(ICameraRig value)
        {
            m_cameraRig = value;
            if (m_cameraRig != null)
            {
                SetIKTargets(m_cameraRig);
            }
            else
            {
                ResetIKTargets();
            }
        }
        protected override void MapOnUpdate() { }
        protected abstract void SetIKTargets(ICameraRig cameraRig);
        protected abstract void ResetIKTargets();
    }
}