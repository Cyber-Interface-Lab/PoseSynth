using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// ある<see cref="ICameraRig"/>の情報を元に、別の<see cref="ICameraRig"/>のトラッカの位置・姿勢を変換するクラスです。
    /// 具体的な変換の内容は、派生クラスで実装します。
    /// This class is used to transform the position and orientation of the trackers of a <see cref="ICameraRig"/> based on the information of another <see cref="ICameraRig"/>.
    /// The specific transformation is implemented in derived classes.
    /// </summary>
    [RequireComponent(typeof(ICameraRig))]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public abstract class CameraRigRemapper : MonoBehaviour, ICameraRigTransformer
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
        public ICameraRig Target
        {
            get => m_target;
            set
            {
                m_target = value;
            }
        }
        /// <summary>
        /// <see cref="Target"/>の変換のために入力として使用する<see cref="ICameraRig"/>です。
        /// <see cref="ICameraRig"/> used as input for the transformation of <see cref="Target"/>.
        /// </summary>
        public virtual ICameraRig Reference
        {
            get => m_reference;
            set
            {
                SetReferenceWithoutNotice(value);
            }
        }
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

        #region protected variable
        protected ICameraRig m_target;
        protected ICameraRig m_reference;
        [SerializeField] protected bool m_isValid = true;
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
        public void SetReferenceWithoutNotice(ICameraRig cameraRig)
        {
            m_reference = cameraRig;
        }
        #endregion

        #region protected method
        /// <summary>
        /// <see cref="IsValid"/>がtrueの場合に、毎ループ呼び出される変換用の関数です。
        /// Called every loop when <see cref="IsValid"/> is true.
        /// </summary>
        protected abstract void RemapOnUpdate();
        #endregion

        #region event
        protected virtual void OnValidate()
        {
            m_target = GetComponent<ICameraRig>();
        }
        protected virtual void Awake()
        {
            m_target = GetComponent<ICameraRig>();
        }
        protected virtual void Update()
        {
            m_hasModified = false;

            if (IsValid && m_reference != null)
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
