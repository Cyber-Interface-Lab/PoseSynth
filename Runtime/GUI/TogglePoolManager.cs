using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CyberInterfaceLab.PoseSynth
{
    public class TogglePoolManager : PoolManager<PooledToggle>
    {
        [SerializeField]
        private ToggleGroup m_toggleGroup;

        protected override void OnGetFromPool(PooledToggle pooledObject)
        {
            base.OnGetFromPool(pooledObject);

            // register toggle group if it is set
            if (m_toggleGroup != null)
            {
                pooledObject.Toggle.group = m_toggleGroup;
            }
        }
    }
}
