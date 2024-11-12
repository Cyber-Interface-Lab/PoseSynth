using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public class AvatarPoolManager : PoolManager<PooledAvatar>
    {
        private void Awake()
        {
            Initialize();
        }
    }
}
