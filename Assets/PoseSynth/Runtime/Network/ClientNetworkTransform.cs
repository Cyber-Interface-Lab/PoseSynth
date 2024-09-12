using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// Used for syncing a transform with client side changes.
    /// This includes host. Pure server as owner isn't supported by this.
    /// Please use NetworkTransform for tranasforms that'll be owned by the server.
    /// (cf https://qiita.com/tamutamuta/items/7e2a04d201714deb0a95)
    /// </summary>
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}