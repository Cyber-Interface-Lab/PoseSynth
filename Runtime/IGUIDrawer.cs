using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Interface who can draw IMGUI for configuration.
    /// </summary>
    public interface IGUIDrawer
    {
        bool IsDrawingGUI { get; set; }
        void DrawGUI(int windowId);
    }
}