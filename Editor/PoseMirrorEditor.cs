using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CyberInterfaceLab.PoseSynth
{
    [CustomEditor(typeof(PoseMirror))]
    public class PoseMirrorEditor : Editor
    {
        private PoseMirror m_mirror;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (m_mirror == null) { m_mirror = target as PoseMirror; }

            // Create pairs automatically
            if (GUILayout.Button("Initialize Pairs"))
            {
                m_mirror.InitializePairs();
            }
        }
    }
    
}