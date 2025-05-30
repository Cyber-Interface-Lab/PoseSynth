using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// 複数の<see cref="Pose"/>を参照して、1つの<see cref="Pose"/>を変換する<see cref="IPoseTransformer"/>です。
    /// This class is used to transform a <see cref="Pose"/> based on the information of multiple <see cref="Pose"/>s.
    /// </summary>
    public abstract class PoseRemapperMultipleReferences : MonoBehaviour, IPoseTransformer
    {
        #region public variable
        /// <inheritdoc/>
        public bool IsValid
        {
            get
            {
                if (m_target == null)
                {
                    m_isValid = false;
                }
                return m_isValid;
            }
            set
            {
                if (m_target == null)
                {
                    m_isValid = false;
                    return;
                }
                m_isValid = value;
            }
        }
        /// <inheritdoc/>
        public Pose Target
        {
            get => m_target;
            set => m_target = value;
        }
        /// <summary>
        /// <see cref="Target"/>の変換のために入力として使用する<see cref="Pose"/>のリストです。
        /// List of <see cref="Pose"/>s used as input for the transformation of <see cref="Target"/>.
        /// </summary>
        public List<Pose> References
        {
            get => m_references;
            set => m_references = value;
        }
        #endregion

        #region private variable
        [SerializeField] protected Pose m_target;
        [SerializeField] protected List<Pose> m_references = new(64);
        [SerializeField] protected bool m_isValid = true;
        #endregion

        #region IObservable
        /// <summary>
        /// Call it when the inner values have changed.
        /// </summary>
        /// <remarks>
        /// This method is implemented in derived classes with <see cref="IObservable{T}"/> where T is the derived class.
        /// To call this, turn on <see cref="m_hasModified"/> in <see cref="RemapOnUpdate"/>.
        /// </remarks>
        public abstract void Notify();
        /// <summary>
        /// Flag to check whether the object has been modified.
        /// If true, <see cref="Notify"/> will be called in <see cref="LateUpdate"/> to synchronize all the parameters to clients.
        /// </summary>
        protected bool m_hasModified = false;
        #endregion

        #region public method
        /// <summary>
        /// 参照する<see cref="Pose"/>を追加します。
        /// Add a <see cref="Pose"/> to be referenced.
        /// </summary>
        /// <param name="pose"></param>
        public virtual void AddPose(Pose pose)
        {
            if (pose == null)
            {
                Debug.LogError("Pose is null");
                return;
            }
            if (!m_references.Contains(pose))
                m_references.Add(pose);
        }
        /// <summary>
        /// 参照する<see cref="Pose"/>を削除します。
        /// Remove a <see cref="Pose"/> from the references.
        /// </summary>
        /// <param name="pose"></param>
        public virtual void RemovePose(Pose pose)
        {
            if (pose == null)
            {
                Debug.LogError("Pose is null");
                return;
            }
            if (m_references.Contains(pose))
                m_references.Remove(pose);
        }
        #endregion

        #region private method
        /// <summary>
        /// <see cref="IsValid"/>がtrueの場合に、毎ループ呼び出される変換用の関数です。
        /// Called every loop when <see cref="IsValid"/> is true.
        /// </summary>
        protected abstract void RemapOnUpdate();
        #endregion

        #region event
        protected virtual void OnValidate()
        {
            m_target = GetComponent<Pose>();
            if (m_target == null)
            {
                m_target = GetComponentInParent<Pose>(includeInactive: true);
            }
        }
        protected virtual void Awake()
        {
            if (m_target == null)
            {
                m_target = GetComponent<Pose>();
                if (m_target == null)
                {
                    m_target = GetComponentInParent<Pose>(includeInactive: true);
                }
            }
        }
        protected virtual void Update()
        {
            m_hasModified = false;
            if (m_isValid && m_references.Count > 0)
            {
                RemapOnUpdate();
            }
        }
        void LateUpdate()
        {
            if (m_hasModified)
            {
                Notify();
            }
        }
        #endregion
    }
}
