using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public abstract class IKMapper : PoseMapper
    {
        public override ICameraRig CameraRig
        {
            set
            {
                m_cameraRig = value;
                if (m_cameraRig is ICameraRig)
                {
                    SetIKTargets(m_cameraRig);
                }
                else
                {
                    ResetIKTargets();
                }
            }
        }
        protected override void MapOnUpdate() { } // Do nothing on update.
        protected abstract void SetIKTargets(ICameraRig cameraRig);
        protected abstract void ResetIKTargets();
    }
}