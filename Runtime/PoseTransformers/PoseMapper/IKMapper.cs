using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// IKを用いて<see cref="PoseMapper"/>を実装するための抽象クラスです。
    /// Abstract class for implementing <see cref="PoseMapper"/> using IK.
    /// </summary>
    public abstract class IKMapper : PoseMapper
    {
        /// <inheritdoc/>
        public override ICameraRig Reference
        {
            set
            {
                SetCameraRigWithoutNotice(value);
                Notify();
            }
            get => m_reference;
        }
        /// <inheritdoc/>
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
        /// <summary>
        /// IKターゲットを設定します。
        /// Set IK targets.
        /// </summary>
        /// <param name="cameraRig"></param>
        protected abstract void SetIKTargets(ICameraRig cameraRig);
        /// <summary>
        /// IKターゲットをnullに設定します。
        /// Set IK targets to null.
        /// </summary>
        protected abstract void ResetIKTargets();
    }
}