using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    [CustomEditor(typeof(CustomLocalCameraRig), editorForChildClasses: true)]
    public class CustomLocalCameraRigEditor : Editor
    {
        CustomLocalCameraRig m_target;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (m_target == null)
            {
                m_target = target as CustomLocalCameraRig;
            }

            if (GUILayout.Button("Initialize"))
            {
                m_target.Initialize();
            }
        }
    }
}
