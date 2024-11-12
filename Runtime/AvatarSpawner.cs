using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public class AvatarSpawner : MonoBehaviour
    {
        #region public variable
        #endregion

        #region private variable
        [SerializeField] AvatarPoolManager m_avatarPool;
        [SerializeField] int m_initialAvatarCount = 5;
        #endregion

        #region public method
        #endregion

        #region private method
        #endregion

        #region event
        void Awake()
        {
        
        }
        private void Start()
        {
            for (int i = 0; i < m_initialAvatarCount; i++)
            {

            }
        }
        void FixedUpdate()
        {
        
        }
        #endregion
    
        #region GUI
        #endregion
    }
}
