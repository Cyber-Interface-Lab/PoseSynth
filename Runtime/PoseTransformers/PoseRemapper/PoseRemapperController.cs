using CyberInterfaceLab.PoseSynth.Network.UserInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Controller of <see cref="PoseRemapper"/>.
    /// Add buttons of <see cref="Pose"/> to set <see cref="PoseRemapper.RefPose"/>.
    /// </summary>
    /// <seealso cref="PoseMapperController"/>
    public class PoseRemapperController : MonoBehaviour
    {
        #region public variable
        #endregion

        #region private variable
        [SerializeField] PoseRemapper[] m_remappers;
        /// <summary>
        /// Candidate poses for its <see cref="PoseRemapper"/>s to refer.
        /// </summary>
        [SerializeField] private List<Pose> m_posesToRefer;
        #endregion

        #region public method
        public void SetRemapperRefPoses(Pose refPose)
        {
            // set ref pose
            foreach (var remapper in m_remappers)
            {
                remapper.Reference = refPose;
            }
        }
        public void ResetRemapperRefPoses()
        {
            // reset ref pose
            foreach (var remapper in m_remappers)
            {
                remapper.Reference = null;
            }
        }
        #endregion

        #region private method
        #endregion

        #region event
        private int m_windowId = Utilities.GetWindowId();
        private Rect m_windowRect = new Rect(10, 10, 200, 150);
        Vector2 m_scrollPos;
        [SerializeField] bool m_isDrawingGUI = true;
        private void OnGUI()
        {
            if (!m_isDrawingGUI) return;

            m_windowRect = GUI.Window(m_windowId, m_windowRect, (id) =>
            {
                // add buttons to select reference pose
                // in a scroll view
                m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
                foreach (var pose in m_posesToRefer)
                {
                    if (GUILayout.Button(pose.name))
                    {
                        SetRemapperRefPoses(pose);
                    }
                }
                GUILayout.EndScrollView();

                // add a button to reset reference pose
                if (GUILayout.Button("Reset"))
                {
                    ResetRemapperRefPoses();
                }

                GUI.DragWindow();
            }, $"PoseRemappers ({name})");
        }
        #endregion
    }
}
