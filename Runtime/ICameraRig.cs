using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public interface ICameraRig
    {
        /// <summary>
        /// Try to get transform of the tracker.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public bool TryGetTransform(TrackerType type, out Transform transform);
        /// <summary>
        /// Try to get <see cref="TrackerType"/> from a transform.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="type"></param>
        /// <returns>Whether successfully get tracker or not.</returns>
        public bool TryGetType(Transform transform, out TrackerType type);
        public CameraRigType Type { get; }
    }
}