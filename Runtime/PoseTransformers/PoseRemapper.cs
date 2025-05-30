using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// ある<see cref="Pose"/>の情報を元に、別の<see cref="Pose"/>を変換するクラスです。
    /// 具体的な変換の内容は、派生クラスで実装します。
    /// This class is used to transform a <see cref="Pose"/> based on the information of another <see cref="Pose"/>.
    /// The specific transformation is implemented in derived classes.
    /// </summary>
    //[RequireComponent(typeof(Pose))]
    
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    public abstract class PoseRemapper : MonoBehaviour, IPoseTransformer
    {
        #region public variable
        [SerializeField]
        protected bool m_isValid = true;
        /// <inheritdoc/>
        public bool IsValid
        {
            get
            {
                if (!m_target)
                {
                    m_isValid = false;
                }
                return m_isValid;
            }
            set
            {
                if (!m_target)
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
            set
            {
                m_target = value;
            }
        }
        /// <summary>
        /// <see cref="Target"/>の変換のために入力として使用する<see cref="Pose"/>です。
        /// <see cref="Pose"/> used as input for the transformation of <see cref="Target"/>.
        /// </summary>
        public Pose Reference
        {
            get => m_reference;
            set
            {
                //m_refPose = value;
                SetReferenceWithoutNotice(value);
                Notify();
            }
        }
        #endregion
        #region protected variable
        /// <summary>
        /// The result pose.
        /// </summary>
        [SerializeField]
        protected Pose m_target;
        /// <summary>
        /// The reference pose.
        /// </summary>
        [SerializeField]
        protected Pose m_reference;
        #endregion

        #region observable
        /// <summary>
        /// Call it when the inner values have changed.
        /// </summary>
        /// <remarks>
        /// This method is implemented in derived classes with <see cref="IObservable{T}"/> where T is the derived class.
        /// To call this, turn on <see cref="m_hasModified"/> in <see cref="RemapOnUpdate"/>.
        /// </remarks>
        public abstract void Notify();
        protected bool m_hasModified = false;
        #endregion
        #region public method
        /// <summary>
        /// <see cref="Reference"/>を設定しますが、この時<see cref="Notify"/>の呼び出しを行いません。
        /// Set <see cref="Reference"/> without calling <see cref="Notify"/>.
        /// </summary>
        /// <remarks>
        /// この関数は、ネットワーク間で内部状態の同期を行う際、無限ループを避けるために使用します。
        /// This function is used to avoid infinite loops when synchronizing internal states across the network.
        /// </remarks>
        /// <param name="pose"></param>
        public void SetReferenceWithoutNotice(Pose pose)
        {
            m_reference = pose;
        }
        #endregion
        #region protected method
        /// <summary>
        /// <see cref="IsValid"/>がtrueの場合に、毎ループ呼び出される変換用の関数です。
        /// Called every loop when <see cref="IsValid"/> is true.
        /// </summary>
        protected abstract void RemapOnUpdate();
        #endregion

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

            if (IsValid)
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
    }
}