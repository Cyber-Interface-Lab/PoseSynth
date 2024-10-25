using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Controller of <see cref="PoseMapper"/>.
    /// 
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

            // set active
            gameObject.SetActive(cameraRig is object);

            // set cameraRigs
            m_mappers = GetComponents<PoseMapper>();
            foreach (var mappers in m_mappers)
            {
                mappers.CameraRig = cameraRig;
            }
        }
        public void ResetMapperCameraRig() => SetMapperCameraRig(null);
        #endregion

        #region private method
        #endregion

        #region event
        #endregion

        #region GUI
        #endregion
    }
}