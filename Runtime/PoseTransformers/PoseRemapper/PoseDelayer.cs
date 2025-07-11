using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public class PoseDelayer : PoseRemapper, IObservable<PoseDelayer>
    {
        internal class ApplyPoseCommand : ICommand
        {
            private Pose m_pose;
            private List<Quaternion> m_localRotations;

            /// <summary>
            /// ctr
            /// </summary>
            /// <param name="localRotations"></param>
            public ApplyPoseCommand(Pose pose, List<Quaternion> localRotations)
            {
                m_pose = pose;
                m_localRotations = new List<Quaternion>(localRotations);
            }
            public void Execute()
            {
                m_pose.LocalRotations = m_localRotations;
            }
        }

        [SerializeField]
        protected int m_delayFixedFrame = 30;
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

        #region observable
        HashSet<IObserver<PoseDelayer>> m_observers = new(8);
        public void AddObserver(IObserver<PoseDelayer> observer) => m_observers.Add(observer);
        public void RemoveObserver(IObserver<PoseDelayer> observer) => m_observers.Remove(observer);
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
            m_commands = new Queue<ICommand>(m_delayFixedFrame);
        }
        protected override void RemapOnUpdate()
        {
            // Enqueue a new command.
            m_commands.Enqueue(new ApplyPoseCommand(m_target, m_reference.LocalRotations));
            
            // If # of commands is not enough, do nothing.
            if (m_commands.Count <= m_delayFixedFrame)
            {
                return;
            }

            // Dequeue a command and apply it.
            ICommand command = m_commands.Dequeue();
            command.Execute();
        }
        protected override void OnValidate()
        {
            base.OnValidate();

            // Initialize the command queue
            Initialize();
        }
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }
    }
}
