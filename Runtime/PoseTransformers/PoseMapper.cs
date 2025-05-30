using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// <see cref="ICameraRig"/>の情報を<see cref="Pose"/>へ変換するクラスです。
    /// 具体的な変換は派生クラスで実装します。
    /// This class transforms the information of <see cref="ICameraRig"/> to <see cref="Pose"/>.
    /// The specific transformation is implemented in derived classes.
    /// </summary>
    //[RequireComponent(typeof(Pose))]
    public abstract class PoseMapper : MonoBehaviour, IPoseTransformer
    {
        #region public variable
        /// <inheritdoc/>
        public virtual bool IsValid
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
            set => m_target = value;
        }
        /// <summary>
        /// <see cref="Target"/>の変換のために入力として使用する<see cref="ICameraRig"/>です。
        /// /// <see cref="ICameraRig"/> used as input for the transformation of <see cref="Target"/>.
        /// </summary>
        public virtual ICameraRig Reference
        {
            get => m_reference;
            set
            {
                SetCameraRigWithoutNotice(value);
                Notify();
            }
        }
        #endregion

        #region protected variable
        [SerializeField]
        protected Pose m_target;
        [SerializeField]
        [Tooltip("Referenceとして使用するICameraRigを指定してください。Attach Object with ICameraRig to be Reference.")]
        private Component m_referenceObject;
        protected ICameraRig m_reference;
        [SerializeField]
        protected bool m_isValid = true;
        #endregion

        #region observable
        /// <summary>
        /// Call it when the inner values have changed.
        /// </summary>
        /// <remarks>
        /// This method is implemented in derived classes with <see cref="IObservable{T}"/> where T is the derived class.
        /// To call this, turn on <see cref="m_hasModified"/> in <see cref="MapOnUpdate"/>.
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
        public virtual void SetCameraRigWithoutNotice(ICameraRig value)
        {
            m_referenceObject = value as Component;
            m_reference = value;
        }
        #endregion

        #region protected method
        /// <summary>
        /// <see cref="IsValid"/>がtrueの場合に、毎ループ呼び出される変換用の関数です。
        /// Called every loop when <see cref="IsValid"/> is true.
        /// </summary>
        protected abstract void MapOnUpdate();
        #endregion



        protected virtual void Awake()
        {
            if (m_referenceObject == null)
            {
                Reference = null;
            }
            else if (m_referenceObject.TryGetComponent<ICameraRig>(out var c))
            {
                Reference = c;
            }
        }
        protected virtual void OnValidate()
        {
            m_target = GetComponent<Pose>();
            if (m_target == null)
            {
                m_target = GetComponentInParent<Pose>(includeInactive: true);
            }

            // get m_cameraRig from Component _cameraRig
            if (m_referenceObject == null)
            {
                m_reference = null;
            }
            else if (m_referenceObject.TryGetComponent<ICameraRig>(out var c))
            {
                m_reference = c;
            }
            else
            {
                Debug.LogError("PoseMapper: m_referenceObject must be ICameraRig!");
                m_referenceObject = null;
                m_reference = null;
            }
        }
        protected virtual void Update()
        {
            m_hasModified = false;

            if (IsValid)
            {
                MapOnUpdate();
            }
        }
    }
}