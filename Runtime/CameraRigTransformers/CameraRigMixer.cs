using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public class CameraRigMixer : CameraRigRemapperMultipleReferences<CameraRigMixer>, IGUIDrawer
    {
        public enum TransformType
        {
            None,
            Global,
            Local,
        }
        [Serializable]
        public struct CameraRigWeight
        {
            public ICameraRig CameraRig;
            [Range(0f, 1f)] public float Weight;

            public CameraRigWeight(ICameraRig cameraRig, float weight = 1)
            {
                CameraRig = cameraRig;
                Weight = weight;
            }
        }
        [Serializable]
        public struct MixedTrackerGroup
        {
            public TrackerType TrackerType;
            public List<CameraRigWeight> WeightsForCameraRigs;

            public MixedTrackerGroup(TrackerType type, List<ICameraRig> cameraRigs)
            {
                TrackerType = type;
                WeightsForCameraRigs = new List<CameraRigWeight>();
                for (int i=0; i<cameraRigs.Count; i++)
                {
                    if (cameraRigs[i] is null) { continue; }

                    if (cameraRigs[i].TryGetTransform(type, out var t))
                    {
                        WeightsForCameraRigs.Add(new CameraRigWeight(cameraRigs[i]));
                    }
                }
            }

            public static MixedTrackerGroup Merge(MixedTrackerGroup oldGroup, MixedTrackerGroup newGroup)
            {
                var result = new MixedTrackerGroup(oldGroup.TrackerType, new List<ICameraRig>());

                for (int i=0; i<newGroup.WeightsForCameraRigs.Count; i++)
                {
                    // register the old one if both old and new group have the same ICameraRig
                    if (oldGroup.WeightsForCameraRigs.Contains(newGroup.WeightsForCameraRigs[i]))
                    {
                        int oldIndex = oldGroup.WeightsForCameraRigs.IndexOf(newGroup.WeightsForCameraRigs[i]);
                        result.WeightsForCameraRigs.Add(oldGroup.WeightsForCameraRigs[oldIndex]);
                    }

                    // register the new ICameraRig that only the new group contains
                    else
                    {
                        result.WeightsForCameraRigs.Add(newGroup.WeightsForCameraRigs[i]);
                    }
                }
                return result;
            }
            public MixedTrackerGroup Merge(MixedTrackerGroup newGroup) => Merge(this, newGroup);
            public void SetWeightOf(ICameraRig cameraRig, float weight, out bool hasModified)
            {
                hasModified = false;
                for (int i=0; i<WeightsForCameraRigs.Count; i++)
                {
                    if (WeightsForCameraRigs[i].CameraRig == cameraRig)
                    {
                        var w = WeightsForCameraRigs[i];
                        if (w.Weight != weight) hasModified = true;
                        w.Weight = weight;
                        WeightsForCameraRigs[i] = w;
                    }
                }
            }

            public Vector3 GetMixedPosition(bool isLocal)
            {
                Vector3 result = Vector3.zero;
                float sum = 0f;
                for (int i=0; i<WeightsForCameraRigs.Count; i++)
                {
                    // add all the positions
                    if (WeightsForCameraRigs[i].CameraRig.TryGetTransform(TrackerType, out var t))
                    {
                        result += WeightsForCameraRigs[i].Weight * (isLocal ? t.localPosition : t.position);
                        sum += WeightsForCameraRigs[i].Weight;
                    }
                }

                return (sum == 0f) ? Vector3.zero : result / sum;
            }
            public Quaternion GetMixedRotation(bool isLocal)
            {
                var args = new (Quaternion q, float w)[WeightsForCameraRigs.Count];
                for (int i=0; i< WeightsForCameraRigs.Count; i++)
                {
                    if (WeightsForCameraRigs[i].CameraRig.TryGetTransform(TrackerType, out var t))
                    {
                        args[i].q = isLocal ? t.localRotation : t.rotation;
                        args[i].w = WeightsForCameraRigs[i].Weight;
                    }
                    else
                    {
                        args[i].q = Quaternion.identity;
                        args[i].w = 0f;
                    }
                }
                return GetMixedQuaternion(args);
            }

            private (Quaternion q, float w) GetMixedQuaternion((Quaternion q, float w) left, (Quaternion q, float w) right)
            {
                // early return
                if (left.w == 0f) { return right; }
                if (right.w == 0f) { return left; }

                // do spherical linear interpolation for the args.
                var q = Quaternion.Slerp(left.q, right.q, (left.w + right.w == 0f) ? 0.5f : right.w / (left.w + right.w));
                var w = left.w + right.w;
                return (q, w);
            }
            private Quaternion GetMixedQuaternion(params (Quaternion q, float w)[] args)
            {
                if (args.Length == 0) { return  Quaternion.identity; }
                if (args.Length == 1) { return args[0].q; }

                var q = Quaternion.identity;
                var w = 0f;
                for (int i=0; i<args.Length - 1; i++)
                {
                    (q, w) = GetMixedQuaternion(args[i], args[i + 1]);
                    args[i + 1].q = q;
                    args[i + 1].w = w;
                }
                return args[args.Length-1].q;
            }
        }

        #region public variable
        public List<TrackerType> Trackers => m_trackers;
        public List<MixedTrackerGroup> MixedTrackerGroups => m_mixedTrackerGroups;
        public TransformType PositionTransformType
        {
            set => m_mixPosition = value;
            get => m_mixPosition;
        }
        public TransformType RotationTransformType
        {
            set => m_mixRotation = value;
            get => m_mixRotation;
        }
        #endregion

        #region private variable
        /// <summary>
        /// TrackerTypes to mix.
        /// </summary>
        [SerializeField]
        private List<TrackerType> m_trackers = new(64);
        private List<TrackerType> m_previousTrackers = new(64);
        [SerializeField]
        private List<MixedTrackerGroup> m_mixedTrackerGroups = new(64);
        protected List<MonoBehaviour> m_previousReferences = new(64);
        [SerializeField] TransformType m_mixPosition;
        [SerializeField] TransformType m_mixRotation;
        #endregion

        #region public method
        public override void AddReference(ICameraRig reference)
        {
            base.AddReference(reference);
            InitializeMixedTrackerGroups();
        }
        public override void RemoveReference(ICameraRig reference)
        {
            base.RemoveReference(reference);
            InitializeMixedTrackerGroups();
        }
        public void InitializeMixedTrackerGroups(bool removeNull = true, bool mergeCurrentValue = true)
        {
            if (removeNull)
            {
                // –³Œø‚ÈŽQÆ‚ðÁ‚·
                m_references.RemoveAll(x => x == null);
            }

            var result = new List<MixedTrackerGroup>();
            var oldGroup = new List<MixedTrackerGroup>(m_mixedTrackerGroups);

            //Debug.Log("Init");

            // create MixedTrackerGroup for each TrackerType
            var references = References; // call getter
            for (int i = 0; i < m_trackers.Count; i++)
            {
                var group = new MixedTrackerGroup(m_trackers[i], references);

                // If the old MixedTrackerGroup has the same TrackerType, merge them.
                for (int j = 0; j < m_mixedTrackerGroups.Count; j++)
                {
                    if (group.TrackerType == m_mixedTrackerGroups[j].TrackerType && mergeCurrentValue)
                    {
                        group = MixedTrackerGroup.Merge(oldGroup[j], group);
                    }
                }

                result.Add(group);
            }

            m_mixedTrackerGroups = new List<MixedTrackerGroup>(result);
        }
        public bool TryGetWeight(ICameraRig cameraRig, TrackerType type, out float weight)
        {
            try
            {
                weight = MixedTrackerGroups.Where(x => x.TrackerType == type).First()
                    .WeightsForCameraRigs.Where(x => x.CameraRig == cameraRig).First()
                    .Weight;
                return true;
            }
            catch
            {
                weight = 0f;
                return false;
            }
        }
        public void SetWeight(ICameraRig cameraRig, TrackerType type, float weight, float minWeight = 0f, float maxWeight = 1f)
        {
            for (int i=0; i<MixedTrackerGroups.Count; i++)
            {
                if (MixedTrackerGroups[i].TrackerType == type)
                {
                    weight = Mathf.Clamp(weight, minWeight, maxWeight);
                    MixedTrackerGroups[i].SetWeightOf(cameraRig, weight, out bool hasModified);
                    m_hasModified |= hasModified;
                }
            }
        }
        public void AddWeight(ICameraRig cameraRig, TrackerType type, float delta, float minWeight = 0f, float maxWeight = 1f)
        {
            if (TryGetWeight(cameraRig, type, out float weight))
            {
                float newWeight = Mathf.Clamp(weight + delta, minWeight, maxWeight);
                SetWeight(cameraRig, type, newWeight, minWeight, maxWeight);
            }
        }
        #endregion

        #region protected method
        protected override void RemapOnUpdate()
        {
            // normalize the weights and mix them.
            for (int i = 0; i<m_mixedTrackerGroups.Count; i++)
            {
                var type = m_mixedTrackerGroups[i].TrackerType;

                if (m_target.TryGetTransform(type, out var t))
                {
                    switch (m_mixPosition)
                    {
                        case TransformType.Global:
                            t.position = m_mixedTrackerGroups[i].GetMixedPosition(isLocal: false);
                            break;
                        case TransformType.Local:
                            t.localPosition = m_mixedTrackerGroups[i].GetMixedPosition(isLocal: true);
                            break;
                        case TransformType.None:
                        default:
                            break;
                    }
                    switch (m_mixRotation)
                    {
                        case TransformType.Global:
                            t.rotation = m_mixedTrackerGroups[i].GetMixedRotation(isLocal: false);
                            break;
                        case TransformType.Local:
                            t.localRotation = m_mixedTrackerGroups[i].GetMixedRotation(isLocal: true);
                            break;
                        case TransformType.None:
                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        #region event
        protected override void OnValidate()
        {
            base.OnValidate();
            InitializeMixedTrackerGroups(removeNull: false);
        }
        protected override void Awake()
        {
            base.Awake();
            InitializeMixedTrackerGroups();
        }
        #endregion

        #region GUI
        int m_windowId = Utilities.GetWindowId();
        Rect m_windowRect = new Rect(0, 0, 1, 1);
        private readonly int m_windowTrackerTypeWidth = 80;
        private readonly int m_windowSlideBarWidth = 100;
        private readonly int m_windowSpaceWidth = 10;
        private readonly int m_windowSpaceHeight = 20;
        [SerializeField] bool m_isDrawingGUI = true;
        public bool IsDrawingGUI
        {
            get => m_isDrawingGUI;
            set => m_isDrawingGUI = value;
        }
        public void DrawGUI(int windowId)
        {
            // draw the name of camera rigs
            for (int i=0; i<m_references.Count; i++)
            {
                GUI.Label(new Rect(
                    x: m_windowTrackerTypeWidth + m_windowSpaceWidth + (m_windowSlideBarWidth + m_windowSpaceWidth) * i,
                    y: m_windowSpaceHeight,
                    width: m_windowSlideBarWidth,
                    height: m_windowSpaceHeight
                    ), m_references[i].name);
            }

            // draw the slide bars
            var references = References;
            for (int i=0; i<m_mixedTrackerGroups.Count; i++)
            {
                // draw the tracker type of the group
                GUI.Label(new Rect(
                    x: 10,
                    y: m_windowSpaceHeight * (2 + i),
                    width: m_windowTrackerTypeWidth,
                    height: m_windowSpaceHeight
                    ), m_mixedTrackerGroups[i].TrackerType.ToString());

                // draw slide bars
                for (int j=0; j<m_references.Count; j++)
                {
                    if (TryGetWeight(references[j], MixedTrackerGroups[i].TrackerType, out float weight))
                    {
                        float nextWeight = GUI.HorizontalSlider(new Rect(
                            x: m_windowTrackerTypeWidth + m_windowSpaceWidth + (m_windowSlideBarWidth + m_windowSpaceWidth) * j,
                            y: m_windowSpaceHeight * (2 + i) + 5,
                            width: m_windowSlideBarWidth,
                            height: m_windowSpaceHeight
                            ), weight, 0f, 1f);
                        SetWeight(references[j], MixedTrackerGroups[i].TrackerType, nextWeight);
                    }
                }
            }

            // add buttons to override all the weight to one camera rig.
            for (int i = 0; i < m_references.Count; i++)
            {
                if (GUI.Button(new Rect(
                    x: m_windowTrackerTypeWidth + m_windowSpaceWidth + (m_windowSlideBarWidth + m_windowSpaceWidth) * i,
                    y: m_windowSpaceHeight * (2 + m_mixedTrackerGroups.Count),
                    width: m_windowSlideBarWidth,
                    height: m_windowSpaceHeight
                    ), "Override"))
                {
                    for (int j = 0; j < m_mixedTrackerGroups.Count; j++)
                    {
                        var mainCameraRig = references[i];
                        var TrackerType = m_mixedTrackerGroups[j].TrackerType;

                        for (int k = 0; k < m_references.Count; k++)
                        {
                            if (TryGetWeight(references[k], TrackerType, out float weight))
                            {
                                SetWeight(references[k], TrackerType, references[k] == mainCameraRig ? 1f : 0f);
                            }
                        }
                    }
                }
            }

            GUI.DragWindow();
        }
        void OnGUI()
        {
            if (!m_isValid) { return; }
            if (!m_isDrawingGUI) { return; }

            // resize the window
            m_windowRect.width = m_windowTrackerTypeWidth + References.Count * (m_windowSlideBarWidth + m_windowSpaceWidth) + 20;
            m_windowRect.height = m_windowSpaceHeight * (4 + m_trackers.Count);

            m_windowRect = GUI.Window(m_windowId, m_windowRect, (id) =>
            {
                DrawGUI(id);
            }, $"{name} CameraRigMixer");
        }
        #endregion
    }
}
