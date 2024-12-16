using CyberInterfaceLab.PoseSynth.Network;
using CyberInterfaceLab.PoseSynth.Network.UserInterfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Controller of <see cref="PoseMapper"/>.
    /// Add a button to embody the avatar (<see cref="PoseMapper.Pose"/>).
    /// </summary>
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
        public void SetMapperCameraRig(ICameraRig cameraRig)
        {
            // set cameraRigs
            m_mappers = GetComponents<PoseMapper>();
            foreach (var mapper in m_mappers)
            {
                mapper.CameraRig = cameraRig;
            }
        }
        public void ResetMapperCameraRig()
        {
            // reset cameraRigs
            m_mappers = GetComponents<PoseMapper>();
            foreach (var mapper in m_mappers)
            {
                mapper.CameraRig = null;
            }
        }
        #endregion

        #region private method
        #endregion

        #region event
        #endregion
    }
}