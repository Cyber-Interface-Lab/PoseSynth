using CyberInterfaceLab.PoseSynth.Network.UserInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Controller of <see cref="PoseRemapper"/>.
    /// Add a button to set <see cref="PoseRemapper.RefPose"/>.
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
        /// <summary>
        /// Set/reset the ref pose of <see cref="m_remappers"/> to <see cref="m_posesToRefer"/>[index].
        /// </summary>
        private List<PooledButton> m_buttons = new(64);

        [SerializeField] private bool m_addButtonToGUI = true;
        #endregion

        #region public method
        public void SetRemapperRefPoses(Pose refPose)
        {
            // set active
            gameObject.SetActive(refPose is object);

            // set ref pose
            foreach (var remapper in m_remappers)
            {
                remapper.RefPose = refPose;
            }
        }
        public void ResetRemapperRefPoses() => SetRemapperRefPoses(null);
        #endregion

        #region private method
        private void SetRefPose(int index)
        {
            if (m_posesToRefer.Count <= index || index < 0) { return; }
            SetRemapperRefPoses(m_posesToRefer[index]);

            // remove this listener
            m_buttons[index].Button.onClick.RemoveAllListeners();
            // add another function
            m_buttons[index].Button.onClick.AddListener(() => ResetRefPose(index));
            m_buttons[index].Text.text = $"Reset Remappers";
        }
        private void ResetRefPose(int index)
        {
            ResetRemapperRefPoses();

            // remove this listener
            m_buttons[index].Button.onClick.RemoveAllListeners();
            // add another function
            m_buttons[index].Button.onClick.AddListener(() => SetRefPose(index));
            m_buttons[index].Text.text = $"Set Remappers => {m_posesToRefer[index].name}";
        }
        #endregion

        #region event
        void Start()
        {
            if (m_addButtonToGUI)
            {
                var networkGUI = NetworkGUIStateMachine.Instance;
                if (networkGUI != null)
                {
                    for (int i=0; i<m_posesToRefer.Count; i++)
                    {
                        // add button
                        //m_buttons[i] = networkGUI.AddButton();
                        var button = networkGUI.AddButton();
                        button.Button.onClick.AddListener(() => SetRefPose(i));
                        button.Text.text = $"Set Remappers => {m_posesToRefer[i].name}";
                        m_buttons.Add(button);
                    }
                }
            }
        }
        private void Update()
        {
            // enable/disable the button if the ref pose is active/deactive
            for (int i=0; i< m_posesToRefer.Count; ++i)
            {
                m_buttons[i].gameObject.SetActive(m_posesToRefer[i].gameObject.activeSelf);
            }
        }
        private void OnDestroy()
        {
            for (int i=0; i< m_posesToRefer.Count; i++)
            {
                if (m_buttons[i] != null)
                {
                    m_buttons[i]?.Deactivate();
                }
            }
        }
        #endregion
    }
}
