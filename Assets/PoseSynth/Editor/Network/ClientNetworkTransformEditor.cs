using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// ClientNetworkTransformにボタンを追加する
    /// </summary>
    [CustomEditor(typeof(ClientNetworkTransform))]
    public class ClientNetworkTransformEditor : Editor
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
                    if (children[i] == (target as ClientNetworkTransform).transform)
                    {
                        continue;
                    }
                    children[i].gameObject.AddComponent<ClientNetworkTransform>();
                }
            }
            if (GUILayout.Button("Remove ClientNetworkTransform from Children"))
            {
                var children = target.GetComponentsInChildren<Transform>();
                for (int i = 0; i < children.Length; i++)
                {
                    var cnt = children[i].GetComponent<ClientNetworkTransform>();
                    if (cnt != null)
                    {
                        DestroyImmediate(cnt);
                    }
                }
            }
        }
    }
}