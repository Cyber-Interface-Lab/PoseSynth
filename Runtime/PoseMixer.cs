using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Mixer that mixes several Poses into one Pose.
    /// </summary>
    [RequireComponent(typeof(Pose))]

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    public class PoseMixer : MonoBehaviour, ISynthesizer, IGUIDrawer, IObservable<PoseMixer>
    {
        /// <summary>
        /// The result pose.
        /// </summary>
        [SerializeField]
        private Pose m_poseMixed;
        public Pose Pose
        {
            get => m_poseMixed;
            set => m_poseMixed = value;
        }
        /// <summary>
        /// The poses that are mixed.
        /// </summary>
        public List<Pose> Poses = new(64);
        /// <summary>
        /// The poses one frame ago.
        /// This is used to update MixedBoneGroups in inspector.
        /// </summary>
        private List<Pose> m_previousPoses = new List<Pose>();

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
        public struct MixedBoneGroup
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
            /// Constructor
            /// </summary>
            /// <param name="label"></param>
            /// <param name="poses"></param>
            public MixedBoneGroup(string label, List<Pose> poses, bool normalize = true)
            {
                Label = label;
                WeightsForPoses = new List<PoseNameAndWeights>();
                for (int i = 0; i < poses.Count; i++)
                {
                    if (poses[i] is null) { continue; }

                    // 各Poseに同じラベルのGroupがあれば，それに対応するPoseNameAndWeightsを作る
                    if (Pose.BoneGroup.SearchFromLabel(poses[i].BoneGroups, label, out var result))
                    {
                        WeightsForPoses.Add(new PoseNameAndWeights(poses[i]));
                    }
                }
            }
            public bool HasSamePose(MixedBoneGroup other)
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
            /// Create a new MixedBoneGroup that merges two MixedBoneGroups.
            /// - If two MixedBoneGroups has the same Pose, the older one is adopted.
            /// - Poses held only by old MixedBoneGroup are ignored.
            /// - Poses held only by new MixedBoneGroup are adopted. In this case, the weights of adopted Poses are 1.
            /// </summary>
            /// <returns></returns>
            public static MixedBoneGroup Merge(MixedBoneGroup oldGroup, MixedBoneGroup newGroup)
            {
                var result = new MixedBoneGroup(oldGroup.Label, new List<Pose>());

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
            public MixedBoneGroup Merge(MixedBoneGroup newGroup) => Merge(this, newGroup);
            /// <summary>
            /// Set the weight of input Pose.
            /// </summary>
            /// <param name="pose"></param>
            /// <param name="weight">The new weight.</param>
            public void SetWeightOf(Pose pose, float weight)
            {
                for (int i = 0; i < WeightsForPoses.Count; i++)
                {
                    if (WeightsForPoses[i].Pose == pose)
                    {
                        var w = WeightsForPoses[i];
                        w.Weight = weight;
                        WeightsForPoses[i] = w;
                    }
                }
            }
        }
        /// <summary>
        /// All MixedBoneGroups based on Poses.
        /// </summary>
        [SerializeField]
        public List<MixedBoneGroup> MixedBoneGroups = new(64);

        /// <summary>
        /// Add a Pose to the list Poses.
        /// </summary>
        /// <param name="pose"></param>
        public void AddPose(Pose pose)
        {
            if (Poses.Contains(pose))
            {
                Debug.LogWarning($"PoseMixer.AddPose: This PoseMixer already contains Pose {pose.name}!");
            }

            Poses.Add(pose);
            InitializeMixedBoneGroups();
        }
        /// <summary>
        /// Remove a Pose from the list Poses.
        /// </summary>
        /// <param name="pose"></param>
        public void RemovePose(Pose pose)
        {
            if (Poses.Contains(pose))
            {
                Poses.Remove(pose);
                InitializeMixedBoneGroups();
            }
        }

        /// <summary>
        /// Initialize MixedBoneGroups based on Poses.
        /// </summary>
        public void InitializeMixedBoneGroups(bool mergeCurrentValue = true)
        {
            var result = new List<MixedBoneGroup>();
            var oldGroup = new List<MixedBoneGroup>(MixedBoneGroups);

            // ラベルごとにMixedBoneGroupを作る
            for (int i = 0; i < m_poseMixed.BoneGroups.Count; i++)
            {
                var group = new MixedBoneGroup(m_poseMixed.BoneGroups[i].Label, Poses);

                // 古いMixedBoneGroupに同じラベルのものがあれば，
                // マージする
                for (int j = 0; j < MixedBoneGroups.Count; j++)
                {
                    if (group.Label == MixedBoneGroups[j].Label && mergeCurrentValue)
                    {
                        //group = oldGroup[j].GetMergedGroup(group);
                        group = MixedBoneGroup.Merge(oldGroup[j], group);
                    }
                }

                result.Add(group);
            }

            MixedBoneGroups = new List<MixedBoneGroup>(result);

            //Debug.Log($"{typeof(PoseMixer)}: InitializeMixedBoneGroups");
        }
        /// <summary>
        /// Get the weight corresponding to the input label from the input Pose.
        /// </summary>
        /// <param name="pose"></param>
        /// <param name="label"></param>
        /// <param name="weight"></param>
        /// <returns>The weight that matches the condition exists.</returns>
        public bool SearchWeightOf(Pose pose, string label, out float weight)
        {
            try
            {
                weight = MixedBoneGroups.Where(x => x.Label == label).First()
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
        public void SetWeightOf(Pose pose, string label, float weight, float minWeight = 0f, float maxWeight = 1f)
        {
            for (int i = 0; i < MixedBoneGroups.Count; i++)
            {
                if (MixedBoneGroups[i].Label == label)
                {
                    weight = Mathf.Clamp(weight, minWeight, maxWeight);
                    MixedBoneGroups[i].SetWeightOf(pose, weight);
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
        public IEnumerable SetGraduallyWeightOf(Pose pose, string label, float weight, float time = 0f, float minWeight = 0f, float maxWeight = 1f)
        {
            int frame = 0;
            if (SearchWeightOf(pose, label, out float initialWeight))
            {
                while (frame * Time.deltaTime < time)
                {
                    // tは0~1を取る
                    float t = (frame * Time.deltaTime) / time;
                    float currentWeight = (1 - t) * initialWeight + t * weight;

                    SetWeightOf(pose, label, currentWeight, minWeight, maxWeight);

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
        public void AddWeightOf(Pose pose, string label, float delta, float minWeight = 0f, float maxWeight = 1f)
        {
            if (SearchWeightOf(pose, label, out float weight))
            {
                float newWeight = Mathf.Clamp(weight + delta, 0f, 1f);
                SetWeightOf(pose, label, newWeight, minWeight, maxWeight);
            }
        }

        #region observable
        private HashSet<IObserver<PoseMixer>> m_observers = new(64);
        public void AddObserver(IObserver<PoseMixer> observer) {  m_observers.Add(observer); }
        public void RemoveObserver(IObserver<PoseMixer> observer) => m_observers.Remove(observer);
        public void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        [SerializeField]
        private bool m_isValid = true;
        public bool IsValid
        {
            get
            {
                if (!m_poseMixed)
                {
                    m_isValid = false;
                }
                return m_isValid;
            }
            set
            {
                if (!m_poseMixed)
                {
                    m_isValid = false;
                    return;
                }
                m_isValid = value;
            }
        }
        private void OnValidate()
        {
            m_poseMixed = GetComponent<Pose>();

            // Check if the previous poses are the same as the current one.
            // If not the same, initialize MixedBoneGroups.
            bool posesHasChanged = false;
            if (Poses.Count == m_previousPoses.Count)
            {
                for (int i = 0; i < Poses.Count; i++)
                {
                    if (Poses[i] != m_previousPoses[i])
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
                InitializeMixedBoneGroups();
            }
            m_previousPoses = new List<Pose>(Poses);
        }

        /// <summary>
        /// Take weighted average of several Poses and apply to m_poseMixed.
        /// </summary>
        /// <param name="poses"></param>
        /// <returns></returns>
        public void Mix(params Pose[] poses)
        {
            // 重みを正規化して足し合わせる
            int groupCount = m_poseMixed.BoneGroups.Count;
            for (int i = 0; i < groupCount; i++)
            {
                var group = m_poseMixed.BoneGroups[i];
                // mixされるポーズのi番目のグループのラベル
                string label = group.Label;
                // 同じラベルを持つグループのキャッシュ
                List<Pose.BoneGroup> groupCache = new List<Pose.BoneGroup>();

                // 各Poseに対して，labelと同じラベルを持つグループがあるならば，
                // そのグループをキャッシュに登録する
                for (int j = 0; j < poses.Length; j++)
                {
                    if (Pose.BoneGroup.SearchFromLabel(poses[j].BoneGroups, label, out Pose.BoneGroup g))
                    {
                        // 重みを変更する
                        var modifiedG = new Pose.BoneGroup();
                        modifiedG.Bones = new List<Pose.Bone>(g.Bones);
                        modifiedG.Label = g.Label;

                        // 
                        if (SearchWeightOf(poses[j], g.Label, out var weight))
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
                // i番目のグループ内のボーンに対して
                // それらのlocalRotationの重み付き平均を取る
                if (Pose.BoneGroup.Add(ref group, groupCache.ToArray()))
                {
                    m_poseMixed.BoneGroups[i] = group;
                }
                else
                {
                    Debug.LogWarning($"PoseEditor.PoseMixer: Failed to mix!");
                }
            }
        }
        public void Mix(List<Pose> poses) => Mix(poses.ToArray());

        protected virtual void FixedUpdate()
        {
            if (IsValid)
            {
                // check if there is a null content in Poses.
                if (Poses.RemoveAll(p => p == null) > 0)
                {
                    // then re-initialize MixedBoneGroups.
                    InitializeMixedBoneGroups();
                }

                Mix(Poses);
                Notify();
            }
        }

        #region GUI
        private int _windowId = Utilities.GetWindowId();
        private Rect _windowRect = new Rect(0, 0, 1, 1);
        private readonly int _windowLabelWidth = 80;
        private readonly int _windowSlideBarWidth = 100;
        private readonly int _windowSpaceWidth = 10;
        private readonly int _windowSpaceHeight = 20;
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
            _windowRect.width = _windowLabelWidth + Poses.Count * (_windowSlideBarWidth + _windowSpaceWidth) + 20;
            _windowRect.height = _windowSpaceHeight * (4 + m_poseMixed.BoneGroups.Count);
            _windowRect = GUI.Window(_windowId, _windowRect, (id) =>
            {
                DrawGUI(id);

            }, $"{gameObject.name} PoseMixer");
        }
        public void DrawGUI(int windowId) => ShowWeights(windowId);
        protected void ShowMasterWeights(int windowId)
        {
            for (int i = 0; i < Poses.Count; i++)
            {
                Poses[i].ShowWeightHandlerForMixer(windowId, iter: i);
            }

            for (int i = 0; i < m_poseMixed.BoneGroups.Count; i++)
            {
                var group = m_poseMixed.BoneGroups[i];
                GUI.Label(new Rect(10, 40 + 20 * i, 80, 20), group.Label);
            }

            GUILayout.ExpandWidth(true);
            GUILayout.ExpandHeight(true);
            GUI.DragWindow();
        }
        protected void ShowWeights(int windowId)
        {
            // Posesの名前表示
            for (int i = 0; i < Poses.Count; i++)
            {
                GUI.Label(new Rect(
                    x: _windowLabelWidth + _windowSpaceWidth + (_windowSlideBarWidth + _windowSpaceWidth) * i,
                    y: _windowSpaceHeight,
                    width: _windowSlideBarWidth,
                    height: _windowSpaceHeight
                    ), Poses[i].name);
            }

            // スライドバー表示
            for (int j = 0; j < MixedBoneGroups.Count; j++)
            {
                //MixedBoneGroups[i].ShowWeights(windowId, new Vector2(0, _windowSpaceHeight * (2 + i)));

                // グループのラベル表示
                GUI.Label(new Rect(
                    x: 10,
                    y: _windowSpaceHeight * (2 + j),
                    width: _windowLabelWidth,
                    height: _windowSpaceHeight
                    ), MixedBoneGroups[j].Label);

                // スライドバー表示
                for (int i = 0; i < Poses.Count; i++)
                {
                    if (SearchWeightOf(Poses[i], MixedBoneGroups[j].Label, out float weight))
                    {
                        weight = GUI.HorizontalSlider(new Rect(
                            x: _windowLabelWidth + _windowSpaceWidth + (_windowSlideBarWidth + _windowSpaceWidth) * i,
                            y: _windowSpaceHeight * (2 + j) + 5, // 5は気持ち程度
                            width: _windowSlideBarWidth,
                            height: _windowSpaceHeight
                            ), weight, 0f, 1f);
                        SetWeightOf(Poses[i], MixedBoneGroups[j].Label, weight);
                    }
                }
            }

            // 乗っ取るためのボタンをつける
            for (int i = 0; i < Poses.Count; i++)
            {
                // 全部乗っ取るためのボタンをつける
                if (GUI.Button(new Rect(
                    x: _windowLabelWidth + _windowSpaceWidth + (_windowSlideBarWidth + _windowSpaceWidth) * i,
                    y: _windowSpaceHeight * (2 + MixedBoneGroups.Count),
                    width: _windowSlideBarWidth,
                    height: _windowSpaceHeight
                    ), "Override"))
                {

                    // 乗っ取る
                    for (int i2 = 0; i2 < MixedBoneGroups.Count; i2++)
                    {

                        var mainPose = Poses[i];
                        var label = MixedBoneGroups[i2].Label;

                        for (int i3 = 0; i3 < Poses.Count; i3++)
                        {
                            if (SearchWeightOf(Poses[i3], label, out float weight))
                            {
                                SetWeightOf(Poses[i3], label, Poses[i3] == mainPose ? 1f : 0f);
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