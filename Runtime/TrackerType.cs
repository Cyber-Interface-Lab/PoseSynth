using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Type of tracker.
    /// 
    /// e.g. Head, HandLeft, HandRight, ...
    /// </summary>
    public enum TrackerType
    {
        None = 0,

        // Head
        Head = 10,
        HeadIKTarget,

        // Hand Left
        HandLeft = 20,
        ControllerLeft,
        HandLeftIKTarget,

        // Hand Right
        HandRight = 30,
        ControllerRight,
        HandRightIKTarget,

        // Full Body Tracking
        Pelvis = 100,
        Chest,
        LegLeft,
        LegRight,
        // and so on

        // Finger Left (Oculus Integration)
        HandWristRootLeft = 200,
        HandForearmStubLeft,
        Thumb0Left,
        Thumb1Left,
        Thumb2Left,
        Thumb3Left,
        Index1Left,
        Index2Left,
        Index3Left,
        Middle1Left,
        Middle2Left,
        Middle3Left,
        Ring1Left,
        Ring2Left,
        Ring3Left,
        Pinky0Left,
        Pinky1Left,
        Pinky2Left,
        Pinky3Left,

        // Finger Right (Oculus Integration)
        HandWristRootRight = 300,
        HandForearmStubRight,
        Thumb0Right,
        Thumb1Right,
        Thumb2Right,
        Thumb3Right,
        Index1Right,
        Index2Right,
        Index3Right,
        Middle1Right,
        Middle2Right,
        Middle3Right,
        Ring1Right,
        Ring2Right,
        Ring3Right,
        Pinky0Right,
        Pinky1Right,
        Pinky2Right,
        Pinky3Right,

        // and so on
    }

}