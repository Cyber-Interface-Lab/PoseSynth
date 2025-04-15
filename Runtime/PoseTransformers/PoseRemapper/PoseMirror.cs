using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{

    /// <summary>
    /// Take a mirrored pose of the reference pose (left <-> right).
    /// Create pairs of Bones semi-automatically and mirror them with X, Y, or Z plane.
    /// </summary>
    public class PoseMirror : PoseRemapper, IObservable<PoseMirror>
    {
        /// <summary>
        /// The pair of the reference bone and the result bone.
        /// This also decides with which plane to mirror them (X, Y, or Z plane).
        /// </summary>
        [System.Serializable]
        public struct MirroringPair
        {
            /// <summary>
            /// The reference bone.
            /// </summary>
            public Transform refBone;
            /// <summary>
            /// The result bone.
            /// Must be contained in this pose.
            /// </summary>
            public Transform thisBone;
            /// <summary>
            /// Mirror in X plane.
            /// </summary>
            public bool x;
            /// <summary>
            /// Mirror in Y plane.
            /// </summary>
            public bool y;
            /// <summary>
            /// Mirror in Z plane.
            /// </summary>
            public bool z;

            /// <summary>
            /// ctr
            /// </summary>
            /// <param name="refBone"></param>
            /// <param name="thisBone"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            public MirroringPair(Transform refBone, Transform thisBone, bool x, bool y, bool z)
            {
                this.refBone = refBone;
                this.thisBone = thisBone;
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
        public List<MirroringPair> Pairs;
        /// <summary>
        /// The key word in labels that refers left.
        /// </summary>
        [SerializeField]
        private string m_leftLabel = "Left";
        /// <summary>
        /// The key word in labels that refers right.
        /// </summary>
        [SerializeField]
        private string m_rightLabel = "Right";

        HashSet<IObserver<PoseMirror>> observers = new(16);
        #region observable
        public void AddObserver(IObserver<PoseMirror> observer) => observers.Add(observer);
        public void RemoveObserver(IObserver<PoseMirror> observer) => observers.Remove(observer);
        public override void Notify()
        {
            foreach (var observer in observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        protected override void RemapOnUpdate()
        {
            for (int i = 0; i < Pairs.Count; i++)
            {
                var pair = Pairs[i];
                Vector3 rot = pair.refBone.localEulerAngles;
                rot.x *= pair.x ? -1 : 1;
                rot.y *= pair.y ? -1 : 1;
                rot.z *= pair.z ? -1 : 1;
                pair.thisBone.localEulerAngles = rot;
            }
        }
        /// <summary>
        /// Initialize Pairs.
        /// 
        /// This compares the group labels of the reference pose and the result pose.
        /// If a label of a bone group contains m_leftLabel and a label of another bone group contains m_rightLabel, they are paired.
        /// The paired bone groups are mirrored between the pairs.
        /// The single bone groups are mirrored in group.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void InitializePairs(bool x = false, bool y = true, bool z = true)
        {
            var BoneGroups = m_target.JointGroups;
            var result = new List<MirroringPair>();
            string[][] pairLabels = GetPairLabels();
            for (int i = 0; i < pairLabels.Length; i++)
            {
                var pair = pairLabels[i];

                // 要素数が0ならエラー，何もしない
                if (pair.Length == 0)
                {
                    Debug.LogWarning($"PoseEditor.PoseMirror.InitializePairs: The {Utilities.GetNumberWord(i)} pair has no content!");
                    continue;
                }

                // 要素が1つなら中身をx，y，zにしたがって反転する
                else if (pair.Length == 1)
                {
                    var refGroup = m_reference.JointGroups.Find(x => x.Label == pair[0]);
                    var group = BoneGroups.Find(x => x.Label == pair[0]);
                    for (int j = 0; j < group.Contents.Count; j++)
                    {
                        result.Add(new MirroringPair(refGroup.Contents[j].transform, group.Contents[j].transform, x, y, z));
                    }
                }

                // 要素が2つならそれぞれをx，y，zにしたがって反転した後，互いの値を交換する
                else if (pair.Length == 2)
                {
                    var refLeftGroup = m_reference.JointGroups.Find(x => x.Label == pair[0]);
                    var refRightGroup = m_reference.JointGroups.Find(x => x.Label == pair[1]);
                    var leftGroup = BoneGroups.Find(x => x.Label == pair[0]);
                    var rightGroup = BoneGroups.Find(x => x.Label == pair[1]);
                    for (int j = 0; j < refLeftGroup.Contents.Count; j++)
                    {
                        result.Add(new MirroringPair(refLeftGroup.Contents[j].transform, rightGroup.Contents[j].transform, x, y, z));
                        result.Add(new MirroringPair(refRightGroup.Contents[j].transform, leftGroup.Contents[j].transform, x, y, z));
                    }
                }

                // 要素が3つ以上ならエラー
                else
                {
                    Debug.LogWarning($"PoseEditor.PoseMirror.InitializePairs: The {Utilities.GetNumberWord(i)} pair has {pair.Length} contents!");
                    continue;
                }
            }

            Pairs = result;
        }
        /// <summary>
        /// Create a pair of left and right bone group.
        /// 
        /// 例：
        /// "Spine" -> not paired since the label does not contain any key words.
        /// "Arm Right" & "Arm Left" -> paired since one label contains m_leftLabel and another label contains m_rightLabel, and the remained part of the labels are the same.
        /// </summary>
        /// <returns>The array of paired labels or single labels.
        /// e.g. [["Spine"], ["Arm Left", "Arm Right"]]</returns>
        public string[][] GetPairLabels()
        {
            var BoneGroups = m_target.JointGroups;
            var result = new List<string[]>();
            string resultLog = "";

            for (int i = 0; i < BoneGroups.Count; i++)
            {
                string label = BoneGroups[i].Label;
                if (label.Contains(m_leftLabel)) // "Left"があれば片割れを見つける
                {
                    string labelMinusLeft = label.Replace(m_leftLabel, "");
                    for (int j = 0; j < BoneGroups.Count; j++)
                    {
                        string label2 = BoneGroups[j].Label;
                        if (label2.Replace(m_rightLabel, "") == labelMinusLeft && label2.Contains(m_rightLabel)) // 片割れが見つかった
                        {
                            var pair = new string[2] { label, label2 }; // left, rightの順
                            if (!result.Contains(pair))
                            {
                                result.Add(pair);
                                resultLog = resultLog == "" ? $"[ [{label}, {label2}]" : resultLog + $", [{label}, {label2}]";
                                break;
                            }
                        }
                    }
                }
                else if (label.Contains(m_rightLabel))
                {
                    // left -> rightの順の検索で網羅しているはずなので，right -> leftの順の検索は行わない
                    //continue;
                }
                else // 両方とも無ければそのままリストに入れる
                {
                    result.Add(new string[] { label });
                    resultLog = resultLog == "" ? $"[ [{label}]" : resultLog + $", [{label}]";
                }
            }

            Debug.Log($"PoseEditor.PoseMirror.GetPairLabels: Result is {resultLog} ]");
            return result.ToArray();
        }
    }
}