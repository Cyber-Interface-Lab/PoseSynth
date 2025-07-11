using System;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// トラッカの種類とTransformをインスペクタで対応付けできる辞書です。
    /// Special dictionary class that you can edit in the inspector.
    /// </summary>
    [Serializable]
    public class TrackerDictionary : UDictionary<TrackerType, Transform> { }
}