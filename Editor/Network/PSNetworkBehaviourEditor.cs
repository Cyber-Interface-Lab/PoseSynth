using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    [CustomEditor(typeof(PSNetworkBehaviour), editorForChildClasses: true)]
    public class PSNetworkBehaviourEditor : Editor
    {
        //PSNetworkBehaviour m_target;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            /*
            if (m_target == null)
            {
                m_target = target as PSNetworkBehaviour;
            }

            if (m_target.IsSpawned)
            {
                if (GUILayout.Button("Despawn"))
                {
                    m_target.Despawn();
                }
            }
            else
            {
                if (GUILayout.Button("Spawn"))
                {
                    m_target.Spawn();
                }
            }
            */
        }
    }
}