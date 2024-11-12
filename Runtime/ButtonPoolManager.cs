using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public class ButtonPoolManager : PoolManager<PooledButton>
    {
        #region event
        void Awake()
        {
            Initialize();
        }
        #endregion
    }
}
