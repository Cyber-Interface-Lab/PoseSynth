using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public class PoseDelayer : PoseRemapper, IDelayableSynthesizer
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
                m_pose.BoneLocalRotations = m_localRotations;
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

        public virtual void Initialize()
        {
            m_commands = new Queue<ICommand>(m_delayFixedFrame);
        }
        protected override void RemapOnUpdate()
        {
            // Enqueue a new command.
            m_commands.Enqueue(new ApplyPoseCommand(m_pose, m_refPose.BoneLocalRotations));
            
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
        protected void Awake()
        {
            Initialize();
        }
    }
}
