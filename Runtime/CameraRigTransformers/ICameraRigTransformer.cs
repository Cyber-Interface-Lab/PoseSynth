using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Interface who can transform a <see cref="ICameraRig"/>.
    /// </summary>
    public interface ICameraRigTransformer
    {
        /// <summary>
        /// Can update <see cref="Target"/> if true.
        /// </summary>
        /// <remarks>
        /// マルチユーザ時のクライアント側ではfalseになり、サーバと状態が同期されるようになります。
        /// On the client side in multi-user mode, this value is false so that the state is synchronized with the server.
        /// </remarks>
        bool IsValid { get; set; }
        /// <summary>
        /// Target <see cref="ICameraRig"/> to be transformed.
        /// </summary>
        ICameraRig Target { get; set; }
    }
}
