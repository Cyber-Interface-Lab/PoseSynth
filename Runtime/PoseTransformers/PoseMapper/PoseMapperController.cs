using CyberInterfaceLab.PoseSynth.Network;
using CyberInterfaceLab.PoseSynth.Network.UserInterfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// <see cref="PoseMapper"/>を管理するためのクラスです。
    /// Controller of <see cref="PoseMapper"/>.
    /// Add a button to embody the avatar (<see cref="PoseMapper.Target"/>).
    /// </summary>
    /// <remarks>
    /// 実験設計に応じてこのクラスを改変してカスタマイズすることを推奨します。
    /// We recommend customizing this class according to your experimental design.
    /// </remarks>
    [RequireComponent(typeof(PoseMapper))]
    public class PoseMapperController : MonoBehaviour
    {
        #region public variable
        #endregion

        #region private variable
        //[SerializeField]
        private PoseMapper[] m_mappers;
        #endregion

        #region public method
        /// <summary>
        /// 全ての<see cref="PoseMapper.Target"/>を変更します。
        /// Set all <see cref="PoseMapper.Target"/>.
        /// </summary>
        /// <param name="cameraRig"></param>
        public void SetMapperCameraRig(ICameraRig cameraRig)
        {
            // set cameraRigs
            m_mappers = GetComponentsInChildren<PoseMapper>();
            foreach (var mapper in m_mappers)
            {
                mapper.Reference = cameraRig;
            }
        }
        /// <summary>
        /// 全ての<see cref="PoseMapper.Target"/>をnullに変更します。
        /// Set all <see cref="PoseMapper.Target"/> to null.
        /// </summary>
        public void ResetMapperCameraRig()
        {
            // reset cameraRigs
            m_mappers = GetComponentsInChildren<PoseMapper>();
            foreach (var mapper in m_mappers)
            {
                mapper.Reference = null;
            }
        }
        #endregion

        #region private method
        #endregion

        #region event
        #endregion
    }
}