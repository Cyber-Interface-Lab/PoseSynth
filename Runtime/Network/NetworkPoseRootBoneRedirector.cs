using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// <see cref="PoseRootBoneRedirector"/> for network.
    /// Attach it to the gameobject with it.
    /// </summary>
    [RequireComponent (typeof (PoseRootBoneRedirector))]
    public class NetworkPoseRootBoneRedirector : NetworkPoseRemapper<PoseRootBoneRedirector> { }
}
