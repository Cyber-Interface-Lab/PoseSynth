using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// A pose of a character.
    /// This component holds all bones of a character with labels.
    /// </summary>
    public class Pose : MonoBehaviour, IGUIDrawer
    {
        #region struct
        /// <summary>
        /// A bone of a character.
        /// </summary>
        [System.Serializable]
        public struct Bone
        {
            /// <summary>
            /// Transform of the bone
            /// </summary>
            public Transform transform;
            /// <summary>
            /// The local rotation of the bone
            /// </summary>
            public Quaternion LocalRotation
            {
                get => transform.localRotation;
                set => transform.localRotation = value;
            }
        }
        /// <summary>
        /// Bones grouped by label.
        /// </summary>
        [System.Serializable]
        public struct BoneGroup
        {
            /// <summary>
            /// The name of the group.
            /// Head，Right Arm，Forward Left Foot, etc.
            /// </summary>
            public string Label;
            /// <summary>
            /// The master weight of the group.
            /// PoseMixer follows this weight.
            /// </summary>
            [Range(0f, 1f)]
            public float MasterWeight;
            /// <summary>
            /// The bones that make up this group.
            /// </summary>
            [SerializeField]
            public List<Bone> Bones;

            /// <summary>
            /// グループのリストから，ラベルを元にグループを検索する
            /// Search a group by input label from a list of groups.
            /// </summary>
            /// <param name="groups"></param>
            /// <param name="label"></param>
            /// <returns></returns>
            public static bool SearchFromLabel(List<BoneGroup> groups, string label, out BoneGroup result)
            {
                foreach (var group in groups)
                {
                    if (group.Label == label)
                    {
                        result = group;
                        return true;
                    }
                }
                //Debug.LogWarning($"Pose.BoneGroup.SearchFromLabel: " +
                //    $"Failed to find the BoneGroup labeled as {label}!");
                result = groups[0];
                return false;
            }

            /// <summary>
            /// Add two BoneGroups into one BoneGroup.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <param name="result"></param>
            /// <returns>Successfully added two BoneGroups.</returns>
            public static bool Add(ref BoneGroup result, BoneGroup left, BoneGroup right)
            {
                // leftとrightでLabelが異なるなら左辺を返す
                if (left.Label != right.Label)
                {
                    Debug.LogError($"Pose.BoneGroup.operator+: " +
                        $"Failed to add groups! " +
                        $"The label of the left group is {left.Label} but the right one is {right.Label}." +
                        $"Return the left group as it is.");
                    result = left;
                    return false;
                }

                // leftとrightで要素数が異なるなら左辺を返す
                if (left.Bones.Count != right.Bones.Count)
                {
                    Debug.LogError($"Pose.BoneGroup.operator+: " +
                        $"Failed to add groups! " +
                        $"The number of Bones of the left group is {left.Bones.Count} but the right one is {right.Bones.Count}." +
                        $"Return the left group as it is.");
                    result = left;
                    return false;
                }

                // 2つのグループの重み付き平均を取る
                //result = new BoneGroup();
                result.Label = left.Label;
                result.MasterWeight = left.MasterWeight + right.MasterWeight; // これで3項以上の加算も可能なことは数学的に証明した
                var bones = new List<Bone>();
                for (int i = 0; i < left.Bones.Count; i++)
                {
                    // 重み付き平均を取る
                    var leftBone = left.Bones[i];
                    var rightBone = right.Bones[i];

                    var localRotation = Quaternion.Slerp(leftBone.LocalRotation, rightBone.LocalRotation, (left.MasterWeight + right.MasterWeight == 0f) ? 0.5f : right.MasterWeight / (left.MasterWeight + right.MasterWeight));
                    //var localRotation = Quaternion.Slerp(leftBone.LocalRotation, rightBone.LocalRotation, right.Weight / (left.Weight + right.Weight));
                    var bone = new Bone();
                    bone.transform = result.Bones[i].transform;
                    bone.LocalRotation = localRotation;
                    bones.Add(bone);
                }

                result.Bones = bones;
                return true;
            }
            /// <summary>
            /// Add two or more BoneGroups into one BoneGroup recurrently.
            /// </summary>
            /// <param name="groups"></param>
            /// <param name="result"></param>
            /// <returns>Successfully added two or more BoneGroups.</returns>
            public static bool Add(ref BoneGroup result, params BoneGroup[] groups)
            {
                if (groups.Length == 0)
                {
                    return false;
                }
                if (groups.Length == 1) // groups[0]のrotationをそのまま反映させる
                {
                    //result = groups[0];
                    for (int i = 0; i < result.Bones.Count; i++)
                    {
                        var bone = result.Bones[i];
                        bone.LocalRotation = groups[0].Bones[i].LocalRotation;
                        result.Bones[i] = bone;
                    }
                    return true;
                }

                // 2項以上の加算の場合
                for (int i = 0; i < groups.Length - 1; i++)
                {
                    bool success = Add(ref result, groups[i], groups[i + 1]);
                    groups[i + 1] = result;
                    if (!success)
                    {
                        return false;
                    }
                }
                result = groups[groups.Length - 1];
                //Debug.Log($"add"); // ちゃんと足せている
                return true;
            }
            /// <summary>
            /// Get the labels of the list of BoneGroups.
            /// </summary>
            /// <param name="groups"></param>
            /// <returns></returns>
            public static List<string> LabelsOf(List<BoneGroup> groups)
            {
                var result = new List<string>();
                for (int i = 0; i < groups.Count; i++)
                {
                    result.Add(groups[i].Label);
                }
                return result;
            }
        }
        #endregion

        #region public variable
        /// <summary>
        /// Get all bones from the BoneGroups.
        /// </summary>
        public List<Bone> Bones
        {
            get
            {
                var result = new List<Bone>();
                for (int i = 0; i < BoneGroups.Count; i++)
                {
                    result.AddRange(BoneGroups[i].Bones);
                }
                return result;
            }
        }
        /// <summary>
        /// The local rotations of each bone.
        /// </summary>
        public List<Quaternion> BoneLocalRotations
        {
            get
            {
                var result = new List<Quaternion>();
                var bones = Bones;
                for (int i = 0; i < bones.Count; i++)
                {
                    result.Add(bones[i].LocalRotation);
                }
                return result;
            }
            set
            {
                // そもそもListのサイズが違うなら何もしない
                var bones = Bones;
                if (bones.Count != value.Count)
                {
                    Debug.LogError(
                        $"set_BoneLocalRotations: " +
                        $"Failed to set value. " +
                        $"The size of Bones is {bones.Count}, but the size of input value is {value.Count}.");
                    return;
                }

                // BoneGroupsの先頭から数値を代入していく
                // nはvalueのイテレータ
                int n = 0;
                for (int i = 0; i < BoneGroups.Count; i++)
                {
                    var group = BoneGroups[i];
                    for (int j = 0; j < group.Bones.Count; j++, n++)
                    {
                        var bone = group.Bones[j];
                        bone.LocalRotation = value[n];
                    }
                }
            }
        }
        /// <summary>
        /// Root bone.
        /// </summary>
        public Bone RootBone;
        /// <summary>
        /// The list of BoneGroups.
        /// Include root bone manually.
        /// </summary>
        public List<BoneGroup> BoneGroups;
        #endregion


        #region GUI
        [SerializeField]
        private bool m_isDrawingGUI = false;
        public bool IsDrawingGUI
        {
            set => m_isDrawingGUI = value;
            get => m_isDrawingGUI;
        }
        public void DrawGUI(int windowId) => ShowWeightHandler(windowId);
        /// <summary>
        /// 重みをいじるGUIを出す
        /// </summary>
        /// <param name="windowId"></param>
        public void ShowWeightHandler(int windowId)
        {
            for (int i = 0; i < BoneGroups.Count; i++)
            {
                GUILayout.BeginHorizontal();

                var group = BoneGroups[i];
                GUILayout.Label(group.Label);
                GUILayout.FlexibleSpace();
                group.MasterWeight = GUILayout.HorizontalSlider(group.MasterWeight, 0.01f, 1f);
                BoneGroups[i] = group;

                GUILayout.EndHorizontal();
            }
            GUI.DragWindow();
        }
        /// <summary>
        /// This function is called in PoseMixer that contains this.
        /// </summary>
        /// <param name="windowId"></param>
        /// <param name="iter"></param>
        public void ShowWeightHandlerForMixer(int windowId, int iter = 0)
        {
            GUI.Label(new Rect(90 + 110 * iter, 20, 100, 20), $"{name}");

            for (int i = 0; i < BoneGroups.Count; i++)
            {
                var group = BoneGroups[i];
                group.MasterWeight = GUI.HorizontalSlider(
                    new Rect(90 + 110 * iter, 45 + 20 * i, 100, 20),
                    group.MasterWeight,
                    0.01f,
                    1f);
                BoneGroups[i] = group;
            }
        }
        #endregion
    }
}