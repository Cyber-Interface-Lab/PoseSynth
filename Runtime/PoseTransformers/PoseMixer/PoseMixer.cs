using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Mixer that mixes several Poses into one Pose.
    /// </summary>
    [RequireComponent(typeof(Pose))]

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    public class PoseMixer : PoseRemapperMultipleReferences, IGUIDrawer, IObservable<PoseMixer>
    {
        /// <summary>
        /// The poses one frame ago.
        /// This is used to update MixedBoneGroups in inspector.
        /// </summary>
        private List<Pose> m_lastReferences = new List<Pose>();

        /// <summary>
        /// The collection of weight for each pose by label.
        /// </summary>
        [System.Serializable]
        public struct PoseNameAndWeights
        {
            [HideInInspector]
            public string PoseName;
            [HideInInspector]
            public Pose Pose;
            [Range(0f, 1f)]
            public float Weight;

            public PoseNameAndWeights(Pose pose, float weight = 1f)
            {
                Pose = pose;
                PoseName = pose.name;
                Weight = weight;
            }
            public override bool Equals(object obj)
            {
                return GetHashCode() == obj.GetHashCode();
            }
            public override int GetHashCode()
            {
                return Pose.GetHashCode();// * Weight.GetHashCode();
            }
            public static bool operator ==(PoseNameAndWeights left, PoseNameAndWeights right)
            {
                return Equals(left, right);
            }
            public static bool operator !=(PoseNameAndWeights left, PoseNameAndWeights right)
            {
                return !(left == right);
            }
        }
        /// <summary>
        /// Bone groups in Poses that have the same label.
        /// </summary>
        [System.Serializable]
        public struct MixedJointGroup
        {
            /// <summary>
            /// The label of this group.
            /// </summary>
            [HideInInspector]
            public string Label;
            /// <summary>
            /// Weights for each pose.
            /// </summary>
            public List<PoseNameAndWeights> WeightsForPoses;

            /// <summary>
            /// ctr.
            /// </summary>
            /// <param name="label"></param>
            /// <param name="poses"></param>
            public MixedJointGroup(string label, List<Pose> poses, bool normalize = true)
            {
                Label = label;
                WeightsForPoses = new List<PoseNameAndWeights>();
                for (int i = 0; i < poses.Count; i++)
                {
                    if (poses[i] is null) { continue; }

                    // 各Poseに同じラベルのGroupがあれば，それに対応するPoseNameAndWeightsを作る
                    if (Pose.JointGroup.TrySearchFromLabel(poses[i].JointGroups, label, out var result))
                    {
                        WeightsForPoses.Add(new PoseNameAndWeights(poses[i]));
                    }
                }
            }
            public bool HasSamePose(MixedJointGroup other)
            {
                for (int i = 0; i < WeightsForPoses.Count; i++)
                {
                    for (int j = 0; j < other.WeightsForPoses.Count; j++)
                    {
                        if (WeightsForPoses[i] == other.WeightsForPoses[j])
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            /// <summary>
            /// Create a new <see cref="MixedJointGroup"/>s that merges two <see cref="MixedJointGroup"/>s.
            /// </summary>
            /// <remarks>
            /// (1) If two <see cref="MixedJointGroup"/>s has the same Pose, the older one is adopted.
            /// (2) Poses held only by old <see cref="MixedJointGroup"/> are ignored.
            /// (3) Poses held only by new <see cref="MixedJointGroup"/> are newly added. In this case, the weights of the added Poses are 1.
            /// </remarks>
            /// <returns></returns>
            public static MixedJointGroup Merge(MixedJointGroup oldGroup, MixedJointGroup newGroup)
            {
                var result = new MixedJointGroup(oldGroup.Label, new List<Pose>());

                for (int i = 0; i < newGroup.WeightsForPoses.Count; i++)
                {
                    // 共通するPoseは，古い方の重みで登録する
                    if (oldGroup.WeightsForPoses.Contains(newGroup.WeightsForPoses[i]))
                    {
                        // 古い方でのindex
                        int oldIndex = oldGroup.WeightsForPoses.IndexOf(newGroup.WeightsForPoses[i]);
                        result.WeightsForPoses.Add(oldGroup.WeightsForPoses[oldIndex]);
                    }

                    // 新しいグループのみが保持するPoseは，重みを1として登録する
                    else
                    {
                        result.WeightsForPoses.Add(newGroup.WeightsForPoses[i]);
                        //Debug.Log($"merge: Added new {newGroup.WeightsForPoses[j].PoseName}");
                    }
                }

                return result;
            }
            public MixedJointGroup Merge(MixedJointGroup newGroup) => Merge(this, newGroup);
            /// <summary>
            /// Set the weight of input Pose.
            /// </summary>
            /// <param name="pose"></param>
            /// <param name="weight">The new weight.</param>
            public void SetWeight(Pose pose, float weight, out bool hasModified)
            {
                hasModified = false;
                for (int i = 0; i < WeightsForPoses.Count; i++)
                {
                    if (WeightsForPoses[i].Pose == pose)
                    {
                        var w = WeightsForPoses[i];
                        if (w.Weight != weight) hasModified = true;
                        w.Weight = weight;
                        WeightsForPoses[i] = w;
                    }
                }
            }
            public bool TryGetWeight(Pose pose, out float weight)
            {
                for (int i = 0; i < WeightsForPoses.Count; i++)
                {
                    if (WeightsForPoses[i].Pose == pose)
                    {
                        weight = WeightsForPoses[i].Weight;
                        return true;
                    }
                }
                weight = 0f;
                return false;
            }
        }

        #region public variable
        /// <summary>
        /// All <see cref="MixedJointGroup"/>s based on Poses.
        /// </summary>
        [SerializeField]
        public List<MixedJointGroup> MixedJointGroups = new(64);
        #endregion

        #region protected variable
        HashSet<IObserver<PoseMixer>> m_observers = new(64);
        #endregion

        #region observable
        public void AddObserver(IObserver<PoseMixer> observer) => m_observers.Add(observer);
        public void RemoveObserver(IObserver<PoseMixer> observer) => m_observers.Remove(observer);
        public override void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        #region public method
        /// <summary>
        /// Add a Pose to the list Poses.
        /// </summary>
        /// <param name="pose"></param>
        public override void AddPose(Pose pose)
        {
            if (m_references.Contains(pose))
            {
                Debug.LogWarning($"PoseMixer.AddPose: This PoseMixer already contains Pose {pose.name}!");
            }

            m_references.Add(pose);
            InitializeMixedJointGroups();
        }
        /// <summary>
        /// Remove a Pose from the list Poses.
        /// </summary>
        /// <param name="pose"></param>
        public override void RemovePose(Pose pose)
        {
            if (m_references.Contains(pose))
            {
                m_references.Remove(pose);
                InitializeMixedJointGroups();
            }
        }

        /// <summary>
        /// Initialize <see cref="MixedJointGroup"/>s based on Poses.
        /// </summary>
        public void InitializeMixedJointGroups(bool mergeCurrentValue = true)
        {
            var result = new List<MixedJointGroup>();
            var oldGroup = new List<MixedJointGroup>(MixedJointGroups);

            // ラベルごとにMixedJointGroupを作る
            // Create a new MixedJointGroup for each label
            for (int i = 0; i < m_target.JointGroups.Count; i++)
            {
                var group = new MixedJointGroup(m_target.JointGroups[i].Label, m_references);

                // 古いMixedJointGroupに同じラベルのものがあれば，マージする
                // If there is an old MixedJointGroup with the same label, merge it
                for (int j = 0; j < MixedJointGroups.Count; j++)
                {
                    if (group.Label == MixedJointGroups[j].Label && mergeCurrentValue)
                    {
                        group = MixedJointGroup.Merge(oldGroup[j], group);
                    }
                }

                result.Add(group);
            }

            MixedJointGroups = new List<MixedJointGroup>(result);
        }
        /// <summary>
        /// Get the weight corresponding to the input label from the input Pose.
        /// </summary>
        /// <param name="pose"></param>
        /// <param name="label"></param>
        /// <param name="weight">the weight value as a result.</param>
        /// <returns>The weight that matches the condition exists.</returns>
        public bool TryGetWeight(Pose pose, string label, out float weight)
        {
            try
            {
                weight = MixedJointGroups.Where(x => x.Label == label).First()
                        .WeightsForPoses.Where(x => x.Pose == pose).First()
                        .Weight;
                return true;
            }
            catch
            {
                weight = 0f;
                return false;
            }
        }
        /// <summary>
        /// Set the weight corresponding to the input label from the input Pose.
        /// </summary>
        /// <param name="pose"></param>
        /// <param name="label"></param>
        /// <param name="weight"></param>
        public void SetWeight(Pose pose, string label, float weight, float minWeight = 0f, float maxWeight = 1f)
        {
            for (int i = 0; i < MixedJointGroups.Count; i++)
            {
                if (MixedJointGroups[i].Label == label)
                {
                    weight = Mathf.Clamp(weight, minWeight, maxWeight);
                    MixedJointGroups[i].SetWeight(pose, weight, out bool hasModified);
                    m_hasModified |= hasModified;
                }
            }
        }
        /// <summary>
        /// Set the weight corresponding to the input label from the input Pose.
        /// The value of the weight will change gradually in the input seconds.
        /// </summary>
        /// <param name="pose"></param>
        /// <param name="label"></param>
        /// <param name="weight"></param>
        /// <param name="second">The execution time (sec).</param>
        /// <returns></returns>
        [System.Obsolete]
        public IEnumerable SetGraduallyWeight(Pose pose, string label, float weight, float time = 0f, float minWeight = 0f, float maxWeight = 1f)
        {
            int frame = 0;
            if (TryGetWeight(pose, label, out float initialWeight))
            {
                while (frame * Time.deltaTime < time)
                {
                    // tは0~1を取る
                    float t = (frame * Time.deltaTime) / time;
                    float currentWeight = (1 - t) * initialWeight + t * weight;

                    SetWeight(pose, label, currentWeight, minWeight, maxWeight);

                    frame++;

                    yield return null;
                }
            }

            // 指定したラベルのグループが存在しなかった
            yield break;
        }
        /// <summary>
        /// Add a value to the weight corresponding to the input label from the input Pose.
        /// </summary>
        /// <param name="pose"></param>
        /// <param name="label"></param>
        /// <param name="delta"></param>
        public void AddWeight(Pose pose, string label, float delta, float minWeight = 0f, float maxWeight = 1f)
        {
            if (TryGetWeight(pose, label, out float weight))
            {
                float newWeight = Mathf.Clamp(weight + delta, 0f, 1f);
                SetWeight(pose, label, newWeight, minWeight, maxWeight);
            }
        }
        #endregion

        #region protected method
        /// <summary>
        /// Take weighted average of several Poses and apply to m_poseMixed.
        /// </summary>
        /// <param name="poses"></param>
        /// <returns></returns>
        protected override void RemapOnUpdate()
        {
            // 重みを正規化して足し合わせる
            // Normalize the weights and sum them up
            int groupCount = m_target.JointGroups.Count;
            for (int i = 0; i < groupCount; i++)
            {
                var group = m_target.JointGroups[i];
                // mixされるポーズのi番目のグループのラベル
                // The label of the i-th group of the pose to be mixed
                string label = group.Label;
                // 同じラベルを持つグループのキャッシュ
                // Cache of groups with the same label
                List<Pose.JointGroup> groupCache = new List<Pose.JointGroup>();

                // 各Poseに対して，labelと同じラベルを持つグループがあるならば，
                // そのグループをキャッシュに登録する
                // If there is a group with the same label in each Pose,
                // register that group in the cache
                for (int j = 0; j < m_references.Count; j++)
                {
                    if (Pose.JointGroup.TrySearchFromLabel(m_references[j].JointGroups, label, out Pose.JointGroup g))
                    {
                        // 重みを変更する
                        // Modify the weight
                        var modifiedG = new Pose.JointGroup();
                        modifiedG.Contents = new List<Pose.Joint>(g.Contents);
                        modifiedG.Label = g.Label;

                        // 
                        if (TryGetWeight(m_references[j], g.Label, out var weight))
                        {
                            modifiedG.MasterWeight = g.MasterWeight * weight;
                        }
                        else
                        {
                            modifiedG.MasterWeight = g.MasterWeight;
                        }

                        //groupCache.Add(g);
                        groupCache.Add(modifiedG);
                    }
                }

                // ここまでで同じラベルを持つグループを抽出できたので，
                // i番目のグループ内のjointに対して
                // それらのlocalRotationの重み付き平均を取る
                // Now that we have extracted the groups with the same label,
                // we take the weighted average of the localRotation of the joints in the i-th group.
                if (Pose.JointGroup.TryAdd(ref group, groupCache.ToArray()))
                {
                    m_target.JointGroups[i] = group;
                }
                else
                {
                    Debug.LogWarning($"{typeof(This).FullName}.Mix: Failed to mix!");
                }
            }
        }
        #endregion

        #region event
        protected override void OnValidate()
        {
            base.OnValidate();

            // Check if the previous poses are the same as the current one.
            // If not the same, initialize MixedJointGroups.
            bool posesHasChanged = false;
            if (m_references.Count == m_lastReferences.Count)
            {
                for (int i = 0; i < m_references.Count; i++)
                {
                    if (m_references[i] != m_lastReferences[i])
                    {
                        posesHasChanged = true;
                        break;
                    }
                }
            }
            else
            {
                posesHasChanged = true;
            }

            if (posesHasChanged)
            {
                InitializeMixedJointGroups();
            }
            m_lastReferences = new List<Pose>(m_references);
        }


        protected override void Update()
        {
            if (IsValid)
            {
                // check if there is a null content in Poses.
                if (m_references.RemoveAll(p => p == null) > 0)
                {
                    // then re-initialize MixedBoneGroups.
                    InitializeMixedJointGroups();
                }
            }
            base.Update();
        }
        #endregion

        #region GUI
        private int m_windowId = Utilities.GetWindowId();
        private Rect m_windowRect = new Rect(0, 0, 1, 1);
        private readonly int m_windowLabelWidth = 80;
        private readonly int m_windowSlideBarWidth = 100;
        private readonly int m_windowSpaceWidth = 10;
        private readonly int m_windowSpaceHeight = 20;
        [SerializeField]
        private bool m_isDrawingGUI = true;
        public bool IsDrawingGUI
        {
            get => m_isDrawingGUI;
            set => m_isDrawingGUI = value;
        }
        private void OnGUI()
        {
            if (!IsValid) { return; }
            if (!IsDrawingGUI) { return; }

            // 大きさ変更
            m_windowRect.width = m_windowLabelWidth + m_references.Count * (m_windowSlideBarWidth + m_windowSpaceWidth) + 20;
            m_windowRect.height = m_windowSpaceHeight * (4 + m_target.JointGroups.Count);
            m_windowRect = GUI.Window(m_windowId, m_windowRect, (id) =>
            {
                DrawGUI(id);

            }, $"[PoseMixer] {gameObject.name}");
        }
        public void DrawGUI(int windowId) => ShowWeights(windowId);
        protected void ShowMasterWeights(int windowId)
        {
            for (int i = 0; i < m_references.Count; i++)
            {
                m_references[i].ShowWeightHandlerForMixer(windowId, iter: i);
            }

            for (int i = 0; i < m_target.JointGroups.Count; i++)
            {
                var group = m_target.JointGroups[i];
                GUI.Label(new Rect(10, 40 + 20 * i, 80, 20), group.Label);
            }

            GUILayout.ExpandWidth(true);
            GUILayout.ExpandHeight(true);
            GUI.DragWindow();
        }
        protected void ShowWeights(int windowId)
        {
            // Posesの名前表示
            for (int i = 0; i < m_references.Count; i++)
            {
                GUI.Label(new Rect(
                    x: m_windowLabelWidth + m_windowSpaceWidth + (m_windowSlideBarWidth + m_windowSpaceWidth) * i,
                    y: m_windowSpaceHeight,
                    width: m_windowSlideBarWidth,
                    height: m_windowSpaceHeight
                    ), m_references[i].name);
            }

            // スライドバー表示
            for (int j = 0; j < MixedJointGroups.Count; j++)
            {
                // グループのラベル表示
                GUI.Label(new Rect(
                    x: 10,
                    y: m_windowSpaceHeight * (2 + j),
                    width: m_windowLabelWidth,
                    height: m_windowSpaceHeight
                    ), MixedJointGroups[j].Label);

                // スライドバー表示
                for (int i = 0; i < m_references.Count; i++)
                {
                    if (TryGetWeight(m_references[i], MixedJointGroups[j].Label, out float weight))
                    {
                        weight = GUI.HorizontalSlider(new Rect(
                            x: m_windowLabelWidth + m_windowSpaceWidth + (m_windowSlideBarWidth + m_windowSpaceWidth) * i,
                            y: m_windowSpaceHeight * (2 + j) + 5, // 5は気持ち程度
                            width: m_windowSlideBarWidth,
                            height: m_windowSpaceHeight
                            ), weight, 0f, 1f);
                        SetWeight(m_references[i], MixedJointGroups[j].Label, weight);
                    }
                }
            }

            // 乗っ取るためのボタンをつける
            for (int i = 0; i < m_references.Count; i++)
            {
                // 全部乗っ取るためのボタンをつける
                if (GUI.Button(new Rect(
                    x: m_windowLabelWidth + m_windowSpaceWidth + (m_windowSlideBarWidth + m_windowSpaceWidth) * i,
                    y: m_windowSpaceHeight * (2 + MixedJointGroups.Count),
                    width: m_windowSlideBarWidth,
                    height: m_windowSpaceHeight
                    ), "Override"))
                {

                    // 乗っ取る
                    for (int i2 = 0; i2 < MixedJointGroups.Count; i2++)
                    {

                        var mainPose = m_references[i];
                        var label = MixedJointGroups[i2].Label;

                        for (int i3 = 0; i3 < m_references.Count; i3++)
                        {
                            if (TryGetWeight(m_references[i3], label, out float weight))
                            {
                                SetWeight(m_references[i3], label, m_references[i3] == mainPose ? 1f : 0f);
                            }
                        }
                    }
                }
            }

            GUI.DragWindow();
        }
        #endregion
    }
}