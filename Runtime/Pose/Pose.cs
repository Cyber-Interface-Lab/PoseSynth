using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// A pose of a character.
    /// This component contains all joints of a character with labels.
    /// </summary>
    public class Pose : MonoBehaviour
    {
        #region struct
        /// <summary>
        /// A joint of a character.
        /// </summary>
        [System.Serializable]
        public struct Joint
        {
            public Transform transform;
            /// <summary>
            /// The local rotation of the transform.
            /// </summary>
            public Quaternion LocalRotation
            {
                get => transform.localRotation;
                set => transform.localRotation = value;
            }
        }
        /// <summary>
        /// A group of joints with a certain label.
        /// </summary>
        [System.Serializable]
        public struct JointGroup
        {
            /// <summary>
            /// The name of the group.
            /// Head，Right Arm，Forward Left Foot, etc.
            /// </summary>
            /// <remarks>
            /// "Left" and "Right" are the keywords for <see cref="PoseMirror"/>.
            /// It is recommended that they are used for symmetrical body parts.
            /// </remarks>
            public string Label;
            /// <summary>
            /// The master weight of the group.
            /// <see cref="PoseMixer"/> follows this weight.
            /// </summary>
            [Range(0f, 1f)]
            public float MasterWeight;
            /// <summary>
            /// The joints that make up this group.
            /// </summary>
            [SerializeField]
            public List<Joint> Contents;

            /// <summary>
            /// グループのリストから，ラベルを元にグループを検索する。
            /// Search a group by input label from a list of groups.
            /// </summary>
            /// <param name="groups"></param>
            /// <param name="label"></param>
            /// <returns></returns>
            public static bool TrySearchFromLabel(List<JointGroup> groups, string label, out JointGroup result)
            {
                foreach (var group in groups)
                {
                    if (group.Label == label)
                    {
                        result = group;
                        return true;
                    }
                }
                result = groups[0];
                return false;
            }

            /// <summary>
            /// Add two <see cref="JointGroup"/> into one <see cref="JointGroup"/>.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <param name="result"></param>
            /// <returns>Successfully added two <see cref="JointGroup"/>s.</returns>
            public static bool TryAdd(ref JointGroup result, JointGroup left, JointGroup right)
            {
                // leftとrightでLabelが異なるなら左辺を返す
                if (left.Label != right.Label)
                {
                    Debug.LogError($"{typeof(JointGroup).FullName}.operator+: " +
                        $"Failed to add groups! " +
                        $"The label of the left group is {left.Label} but the right one is {right.Label}." +
                        $"Return the left group as it is.");
                    result = left;
                    return false;
                }

                // leftとrightで要素数が異なるなら左辺を返す
                if (left.Contents.Count != right.Contents.Count)
                {
                    Debug.LogError($"{typeof(JointGroup).FullName}.operator+: " +
                        $"Failed to add groups! " +
                        $"The number of contents of the left group is {left.Contents.Count} but the right one is {right.Contents.Count}." +
                        $"Return the left group as it is.");
                    result = left;
                    return false;
                }

                // 2つのグループの重み付き平均を取る
                // Calcurate the weighted average of the two groups.
                result.Label = left.Label;
                result.MasterWeight = left.MasterWeight + right.MasterWeight;
                var joints = new List<Joint>();
                for (int i = 0; i < left.Contents.Count; i++)
                {
                    // 重み付き平均を取る
                    var leftJoints = left.Contents[i];
                    var rightJoints = right.Contents[i];

                    var localRotation = Quaternion.Slerp(leftJoints.LocalRotation, rightJoints.LocalRotation,
                        (left.MasterWeight + right.MasterWeight == 0f) ? 0.5f : right.MasterWeight / (left.MasterWeight + right.MasterWeight));
                    var joint = new Joint();
                    joint.transform = result.Contents[i].transform;
                    joint.LocalRotation = localRotation;
                    joints.Add(joint);
                }

                result.Contents = joints;
                return true;
            }
            /// <summary>
            /// Add two or more <see cref="JointGroup"/> into one <see cref="JointGroup"/> recurrently.
            /// </summary>
            /// <param name="groups"></param>
            /// <param name="result"></param>
            /// <returns>Successfully added two or more <see cref="JointGroup"/>.</returns>
            public static bool TryAdd(ref JointGroup result, params JointGroup[] groups)
            {
                if (groups.Length == 0)
                {
                    return false;
                }
                if (groups.Length == 1) // groups[0]のrotationをそのまま反映させる
                {
                    //result = groups[0];
                    for (int i = 0; i < result.Contents.Count; i++)
                    {
                        var joint = result.Contents[i];
                        joint.LocalRotation = groups[0].Contents[i].LocalRotation;
                        result.Contents[i] = joint;
                    }
                    return true;
                }

                // 2項以上の加算の場合
                for (int i = 0; i < groups.Length - 1; i++)
                {
                    bool success = TryAdd(ref result, groups[i], groups[i + 1]);
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
            /// Get the labels of the list of <see cref="JointGroup"/>s.
            /// </summary>
            /// <param name="groups"></param>
            /// <returns></returns>
            public static List<string> GetLabels(List<JointGroup> groups)
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
        /// Get all <see cref="Joint"/> from the <see cref="JointGroup"/>s.
        /// </summary>
        public List<Joint> Contents
        {
            get
            {
                var result = new List<Joint>();
                for (int i = 0; i < JointGroups.Count; i++)
                {
                    result.AddRange(JointGroups[i].Contents);
                }
                return result;
            }
        }
        /// <summary>
        /// Get and Set all the local rotations.
        /// </summary>
        public List<Quaternion> LocalRotations
        {
            get
            {
                var result = new List<Quaternion>();
                var joints = Contents;
                for (int i = 0; i < joints.Count; i++)
                {
                    result.Add(joints[i].LocalRotation);
                }
                return result;
            }
            set
            {
                // そもそもListのサイズが違うなら何もしない
                var joints = Contents;
                if (joints.Count != value.Count)
                {
                    Debug.LogError(
                        $"set_LocalRotations: " +
                        $"Failed to set value. " +
                        $"The size of joints is {joints.Count}, but the size of input value is {value.Count}.");
                    return;
                }

                // JointGroupsの先頭から数値を代入していく
                // nはvalueのイテレータ
                int n = 0;
                for (int i = 0; i < JointGroups.Count; i++)
                {
                    var group = JointGroups[i];
                    for (int j = 0; j < group.Contents.Count; j++, n++)
                    {
                        var joint = group.Contents[j];
                        joint.LocalRotation = value[n];
                    }
                }
            }
        }
        /// <summary>
        /// Root joint.
        /// </summary>
        public Joint Root;
        /// <summary>
        /// The list of <see cref="JointGroup"/>.
        /// Include root joint manually.
        /// </summary>
        public List<JointGroup> JointGroups;
        #endregion


        #region GUI
        /// <summary>
        /// This function is called in PoseMixer that contains this.
        /// </summary>
        /// <param name="windowId"></param>
        /// <param name="iter"></param>
        public void ShowWeightHandlerForMixer(int windowId, int iter = 0)
        {
            GUI.Label(new Rect(90 + 110 * iter, 20, 100, 20), $"{name}");

            for (int i = 0; i < JointGroups.Count; i++)
            {
                var group = JointGroups[i];
                float last = group.MasterWeight;
                group.MasterWeight = GUI.HorizontalSlider(
                    new Rect(90 + 110 * iter, 45 + 20 * i, 100, 20),
                    group.MasterWeight,
                    0.01f,
                    1f);
                if (last != group.MasterWeight)
                    JointGroups[i] = group;
            }
        }
        #endregion
    }
}