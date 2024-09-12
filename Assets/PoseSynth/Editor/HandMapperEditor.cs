using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Custom editor of HandMapper
    /// </summary>
    /// <seealso cref="HandMapper"/>
    [CustomEditor(typeof(HandMapper), true)]
    public class HandMapperEditor : Editor
    {
        private HandMapper m_target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (m_target == null)
            {
                m_target = target as HandMapper;
            }
            if (GUILayout.Button("Reset Offset"))
            {
                m_target.InitializeOffset();
            }
        }
    }
}
