using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CyberInterfaceLab.PoseSynth
{
    public class PoseBoneRedirector : PoseRemapper, IObservable<PoseBoneRedirector>
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

        public string Label;
        public int Index;
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
        protected Queue<ICommand> m_commands = new(64);

        protected Pose.JointGroup Group
        {
            get
            {
                if (Pose.JointGroup.TrySearchFromLabel(m_target.JointGroups, Label, out var group))
                {
                    return group;
                }

                Debug.LogError($"BoneRedirector.get_Group: Pose {m_target} does not have BoneGroup {Label}!");
                return new();
            }
        }
        protected Pose.JointGroup RefGroup
        {
            get
            {
                if (Pose.JointGroup.TrySearchFromLabel(m_reference.JointGroups, Label, out var group))
                {
                    return group;
                }

                Debug.LogError($"BoneRedirector.get_Group: RefPose {m_reference} does not have BoneGroup {Label}!");
                return new();
            }
        }
        protected Pose.Joint Joint => Group.Contents[Index];
        protected Pose.Joint RefJoint => RefGroup.Contents[Index];
        protected Transform JointTransform => Joint.transform;
        protected Transform RefJointTransform => RefJoint.transform;


        #region observable
        HashSet<IObserver<PoseBoneRedirector>> m_observers = new(8);
        public void AddObserver(IObserver<PoseBoneRedirector> observer) => m_observers.Add(observer);
        public void RemoveObserver(IObserver<PoseBoneRedirector> observer) => m_observers.Remove(observer);
        public override void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        public virtual void Initialize()
        {
            m_commands = new Queue<ICommand>(m_delayFixedFrame + 1);
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
                    return RefJointTransform.localPosition;
                case TransformType.Global:
                    return RefJointTransform.position;
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
                    return RefJointTransform.localRotation;
                case TransformType.Global:
                    return RefJointTransform.rotation;
            }
        }
        protected override void RemapOnUpdate()
        {
            // Enqueue a new command.
            m_commands.Enqueue(new ApplyCommand(JointTransform, GetRefPosition(), GetRefRotation(), PositionType, RotationType));

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