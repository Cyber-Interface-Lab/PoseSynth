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
    }
}