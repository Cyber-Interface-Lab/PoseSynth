using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CyberInterfaceLab.PoseSynth
{

    public class PoseRootBoneRedirector : PoseRemapper
    {
        internal class ApplyCommand : ICommand
        {
            // apply {m_position} and {m_rotation} to {m_bone} in {m_type} way.
            private Transform m_bone;
            private Vector3 m_position;
            private Quaternion m_rotation;
            private TransformType m_positionType;
            private TransformType m_rotationType;

            /// <summary>
            /// ctr
            /// </summary>
            /// <param name="localRotations"></param>
            public ApplyCommand(Transform bone, Vector3 pos, Quaternion rot, TransformType positionType, TransformType rotationType)
            {
                m_bone = bone;
                m_position = pos;
                m_rotation = rot;
                m_positionType = positionType;
                m_rotationType = rotationType;
            }
            public void Execute()
            {
                ApplyPosition();
                ApplyRotation();
            }
            private void ApplyPosition()
            {
                switch (m_positionType)
                {
                    case TransformType.None:
                    default:
                        break;
                    case TransformType.Local:
                        m_bone.localPosition = m_position;
                        break;
                    case TransformType.Global:
                        m_bone.position = m_position;
                        break;
                }
            }
            private void ApplyRotation()
            {
                switch (m_rotationType)
                {
                    case TransformType.None:
                    default:
                        break;
                    case TransformType.Local:
                        m_bone.localRotation = m_rotation;
                        break;
                    case TransformType.Global:
                        m_bone.rotation = m_rotation;
                        break;
                }
            }
        }
        public enum TransformType
        {
            None,
            Global,
            Local,
        }
        public TransformType PositionType;
        public TransformType RotationType;

        [SerializeField]
        protected int m_delayFixedFrame = 0;
        public int DelayFixedFrame
        {
            get => m_delayFixedFrame;
            set
            {
                m_delayFixedFrame = value;
                Initialize();
            }
        }
        protected Queue<ICommand> m_commands;

        HashSet<IObserver<PoseRootBoneRedirector>> m_observers = new(64);

        #region observable
        public void AddObserver(IObserver<PoseRootBoneRedirector> observer) => m_observers.Add(observer);
        public void RemoveObserver(IObserver<PoseRootBoneRedirector> observer) => m_observers.Remove(observer);
        public override void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        protected Transform RootBone => m_reference.Root.transform;
        protected Transform RefRootBone => m_reference.Root.transform;
        public virtual void Initialize()
        {
            m_commands = new Queue<ICommand>(m_delayFixedFrame);
        }
        protected Vector3 GetRefPosition()
        {
            if (m_reference == null) { return Vector3.zero; }
            var type = PositionType;
            switch (type)
            {
                case TransformType.None:
                default:
                    return Vector3.zero;
                case TransformType.Local:
                    return RefRootBone.localPosition;
                case TransformType.Global:
                    return RefRootBone.position;
            }
        }
        protected Quaternion GetRefRotation()
        {
            if (m_reference == null) { return Quaternion.identity; }
            var type = RotationType;
            switch (type)
            {
                case TransformType.None:
                default:
                    return Quaternion.identity;
                case TransformType.Local:
                    return RefRootBone.localRotation;
                case TransformType.Global:
                    return RefRootBone.rotation;
            }
        }
        protected override void RemapOnUpdate()
        {
            // Enqueue a new command.
            m_commands.Enqueue(new ApplyCommand(RootBone, GetRefPosition(), GetRefRotation(), PositionType, RotationType));

            // if # of commands is not enough, do nothing.
            if (m_commands.Count <= m_delayFixedFrame) { return; }

            // Dequeue a command and apply it.
            ICommand command = m_commands.Dequeue();
            command.Execute();
        }
        protected override void OnValidate()
        {
            base.OnValidate();

            Initialize();
        }
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }
    }
}