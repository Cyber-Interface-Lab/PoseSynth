using UnityEngine;
using RootMotion.FinalIK;

namespace CyberInterfaceLab.PoseSynth.IK
{
    public class VRIKMapper : IKMapper
    {
        [SerializeField]
        private VRIK m_ik;

        public TrackerType HeadTarget = TrackerType.HeadIKTarget;
        public TrackerType PelvisTarget = TrackerType.Pelvis;
        public TrackerType ChestGoal = TrackerType.Chest;
        public TrackerType HandLeftTarget = TrackerType.HandLeftIKTarget;
        public TrackerType HandRightTarget = TrackerType.HandRightIKTarget;
        public TrackerType LegLeftTarget = TrackerType.LegLeft;
        public TrackerType LegRightTarget = TrackerType.LegRight;

        protected override void SetIKTargets(ICameraRig cameraRig)
        {
            Transform t;

            // Spine
            if (cameraRig.TryGetTransform(TrackerType.HeadIKTarget, out t))
            {
                m_ik.solver.spine.headTarget = t;
            }
            if (cameraRig.TryGetTransform(TrackerType.Pelvis, out t))
            {
                m_ik.solver.spine.pelvisTarget = t;
            }
            if (cameraRig.TryGetTransform(TrackerType.Chest, out t))
            {
                m_ik.solver.spine.chestGoal = t;
            }

            // Hand Left
            if (cameraRig.TryGetTransform(TrackerType.HandLeftIKTarget, out t))
            {
                m_ik.solver.leftArm.target = t;
            }

            // Hand Right
            if (cameraRig.TryGetTransform(TrackerType.HandRightIKTarget, out t))
            {
                m_ik.solver.rightArm.target = t;
            }

            // Leg Left
            if (cameraRig.TryGetTransform(TrackerType.LegLeft, out t))
            {
                m_ik.solver.leftLeg.target = t;
            }

            // Leg Right
            if (cameraRig.TryGetTransform (TrackerType.LegRight, out t))
            {
                m_ik.solver.rightLeg.target = t;
            }
        }
        protected override void ResetIKTargets()
        {
            m_ik.solver.spine.headTarget = null;
            m_ik.solver.spine.pelvisTarget = null;
            m_ik.solver.spine.chestGoal = null;
            m_ik.solver.leftArm.target = null;
            m_ik.solver.rightArm.target = null;
            m_ik.solver.leftLeg.target = null;
            m_ik.solver.rightLeg.target = null;
        }
        public override bool IsValid
        {
            get
            {
                if (!m_pose)
                {
                    m_isValid = false;
                    return false;
                }
                return m_isValid;
            }
            set
            {
                if (!m_pose)
                {
                    m_ik.enabled = false;
                    m_isValid = false;
                    return;
                }
                m_ik.enabled = value;
                m_isValid = value;
            }
        }
        protected override void MapOnUpdate()
        {
            m_ik.UpdateSolverExternal();
        }
    }
}