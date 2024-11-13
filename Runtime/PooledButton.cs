using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace CyberInterfaceLab.PoseSynth
{
    public class PooledButton : MonoBehaviour, IPooledObject<PooledButton>
    {
        #region public variable
        public IObjectPool<PooledButton> ObjectPool { set => m_pool = value; }
        public Button Button => m_button;
        public TMPro.TextMeshProUGUI Text => m_text;
        #endregion

        #region private variable
        private IObjectPool<PooledButton> m_pool;
        [SerializeField] private Button m_button;
        [SerializeField] private TMPro.TextMeshProUGUI m_text;
        #endregion

        #region public method
        public void Initialize() { }
        public void Deactivate()
        {
            // clear button functions
            if (m_button != null)
            {
                m_button.onClick.RemoveAllListeners();
            }
            m_pool.Release(this);
        }
        #endregion
    }
}
