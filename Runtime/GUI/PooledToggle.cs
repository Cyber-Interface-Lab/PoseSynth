using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace CyberInterfaceLab.PoseSynth
{
    public class PooledToggle : MonoBehaviour, IPooledObject<PooledToggle>
    {
        #region public variable
        public IObjectPool<PooledToggle> ObjectPool { set => m_pool = value; }
        public Toggle Toggle => m_toggle;
        public TMPro.TextMeshProUGUI TextOn => m_textOn;
        public TMPro.TextMeshProUGUI TextOff => m_textOff;
        #endregion

        #region private variable
        private IObjectPool<PooledToggle> m_pool;
        [SerializeField] private Toggle m_toggle;
        [SerializeField] private TMPro.TextMeshProUGUI m_textOn;
        [SerializeField] private TMPro.TextMeshProUGUI m_textOff;
        #endregion

        #region public method
        public void Initialize()
        {
            m_toggle.onValueChanged.AddListener(OnValueChanged);
        }
        public void Deactivate()
        {
            // clear button functions
            if (m_toggle != null)
            {
                m_toggle.onValueChanged.RemoveAllListeners();
            }
            m_pool.Release(this);
        }
        #endregion

        private void OnValueChanged(bool isOn)
        {
            m_textOn.gameObject.SetActive(isOn);
            m_textOff.gameObject.SetActive(!isOn);
        }
    }
}
