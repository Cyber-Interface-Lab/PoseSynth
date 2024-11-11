using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.VR
{
    /// <summary>
    /// Auto scaler of Clipped Plane Surface.
    /// </summary>
#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    public class OVRGUIScaler : MonoBehaviour
    {
        public const float Thickness = 0.01f;

        #region public variable
        #endregion

        #region private variable
        [SerializeField] private ClippedPlaneSurface m_surface;
        [SerializeField] private RectTransform m_rectTransform;
        #endregion

        #region public method
        #endregion

        #region private method
        #endregion

        #region event
        private void OnValidate()
        {
            m_surface = GetComponent<ClippedPlaneSurface>();
        }
        void Awake()
        {
            m_surface = GetComponent<ClippedPlaneSurface>();
        }
        void Update()
        {
            Vector3 worldPosition = m_rectTransform.position;

            Vector2 canvasSize = m_rectTransform.rect.size;

            float scaleX = canvasSize.x / m_rectTransform.lossyScale.x;
            float scaleY = canvasSize.y / m_rectTransform.lossyScale.y;

            transform.localScale = new Vector3(scaleX, scaleY, Thickness);

            transform.position = worldPosition;

            
        }
        #endregion
    }
}
