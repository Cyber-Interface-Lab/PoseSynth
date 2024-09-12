using System;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Special dictionary class that you can edit in the inspector.
    /// 
    /// Key: TrackerType
    /// Value: Transform
    /// </summary>
    [Serializable]
    public class TrackerDictionary : UDictionary<TrackerType, Transform> { }
}