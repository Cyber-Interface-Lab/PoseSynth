using System;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// �g���b�J�̎�ނ�Transform���C���X�y�N�^�őΉ��t���ł��鎫���ł��B
    /// Special dictionary class that you can edit in the inspector.
    /// </summary>
    [Serializable]
    public class TrackerDictionary : UDictionary<TrackerType, Transform> { }
}