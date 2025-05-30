using System;
using UnityEngine;
using UnityEditor;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// カメラリグを示すインタフェースです。
    /// Interface for camera rig.
    /// </summary>
    public interface ICameraRig
    {
        /// <summary>
        /// <see cref="TrackerType"/>から目的の<see cref="Transform"/>を取得します。
        /// Try to get transform of the tracker from <see cref="TrackerType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public bool TryGetTransform(TrackerType type, out Transform transform);
        /// <summary>
        /// <see cref="Transform"/>に対応する<see cref="TrackerType"/>を取得します。
        /// Get <see cref="TrackerType"/> from a transform.
        /// </summary>
        /// <param name="transform">needs to be a tracker of this camera rig.</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool TryGetType(Transform transform, out TrackerType type);
        public CameraRigType Type { get; }
    }
}