using CyberInterfaceLab.PoseSynth.Network;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// <see cref="PoseBoneRedirector"/> for network.
    /// Attach it to the gameObject with it.
    /// </summary>
    [RequireComponent(typeof(PoseBoneRedirector))]
    public class NetworkPoseBoneRedirector : NetworkPoseRemapper<PoseBoneRedirector>
    {
        
    }
}
