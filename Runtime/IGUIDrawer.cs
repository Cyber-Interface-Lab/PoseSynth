using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// クラスの状態を確認/変更するIMGUIを描画する性質を持つクラスのインターフェースです。
    /// Interface to draw IMGUI for configuration.
    /// </summary>
    public interface IGUIDrawer
    {
        /// <summary>
        /// IMGUIを描画するかどうかのフラグです。
        /// Flag to draw IMGUI.
        /// </summary>
        bool IsDrawingGUI { get; set; }
        /// <summary>
        /// IMGUIを描画します。
        /// Draw IMGUI.
        /// </summary>
        /// <param name="windowId"></param>
        void DrawGUI(int windowId);
    }
}