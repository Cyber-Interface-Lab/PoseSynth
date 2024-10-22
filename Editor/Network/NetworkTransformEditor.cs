using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using Unity.Netcode.Components;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// NetworkTransformにボタンを追加する
    /// </summary>
    [CustomEditor(typeof(NetworkTransform))]
    public class NetworkTransformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // ボタンで子オブジェクト全てにClientNetworkTransformをアタッチする
            if (GUILayout.Button("Add ClientNetworkTransform to Children"))
            {
                var children = target.GetComponentsInChildren<Transform>();
                for (int i = 0; i < children.Length; i++)
                {
                    if (children[i] == (target as NetworkTransform).transform)
                    {
                        continue;
                    }
                    children[i].gameObject.AddComponent<NetworkTransform>();
                }
            }
            if (GUILayout.Button("Remove ClientNetworkTransform from Children"))
            {
                var children = target.GetComponentsInChildren<Transform>();
                for (int i = 0; i < children.Length; i++)
                {
                    var cnt = children[i].GetComponent<NetworkTransform>();
                    if (cnt != null)
                    {
                        DestroyImmediate(cnt);
                    }
                }
            }
        }
    }
}