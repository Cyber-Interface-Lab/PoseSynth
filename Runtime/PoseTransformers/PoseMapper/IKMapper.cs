using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public abstract class IKMapper : PoseMapper
    {
        public override ICameraRig Reference
        {
            set
            {
                SetCameraRigWithoutNotice(value);
                Notify();
            }
            get => m_reference;
        }
        public override void SetCameraRigWithoutNotice(ICameraRig value)
        {
            m_reference = value;
            if (m_reference != null)
            {
                SetIKTargets(m_reference);
            }
            else
            {
                ResetIKTargets();
            }
        }
        protected abstract void SetIKTargets(ICameraRig cameraRig);
        protected abstract void ResetIKTargets();
    }
}