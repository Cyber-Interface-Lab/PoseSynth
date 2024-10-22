using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CyberInterfaceLab.PoseSynth
{
    // Editor拡張
    [CustomEditor(typeof(PoseMixer))]
    //[CustomPropertyDrawer(typeof(PoseMixer))]
    public class PoseMixerEditor : Editor
    {


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var mixer = target as PoseMixer;


            if (GUILayout.Button($"Initialize Mixed Bone Groups"))
            {
                mixer.InitializeMixedBoneGroups(mergeCurrentValue: true);
            }

        }
    }
}