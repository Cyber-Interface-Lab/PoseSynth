# PoseSynth
PoseSynth is a toolkit that streamlines the implementation of avatar joint structures and motion systems.<br>
When using it, please import `PoseSynth.unitypackage` and VR system-specific packages into your Unity project from Releases.<br>

## Features
### Virtual Co-embodiment Control
In conventional methods, the target positions of each end effector (hands, feet, etc.) operated by two people are first integrated into one, and then inverse kinematics (IK) is applied to the entire avatar.
PoseSynth reverses this order by solving IK individually for each operator's skeleton first, then fusing the two obtained full-body poses in real-time.
This allows for operation without breaking the positions of arms and legs when the number of tracking points is increased.

## Developers
- Amane Yamaguchi
- Kenta Hashiura

## Preparation
- Unity 2022.3.6f1

The following packages are required:
- [Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290?locale=ja-JP&srsltid=AfmBOorUUYiQTAoEkpdjBnK3XMrzI5K-kmmlXV9W8mSsx6QrRtlcSQRm)<br>
  - For humanoid avatar motion generation
- [NetCode for GameObjects](https://unity.com/ja/products/netcode)<br>
  - For network communication

## How to Install
PoseSynth is divided into a basic package `PoseSynth_x_x_x.unitypackage` and additional packages for each VR system `ExtensionFor{VR_System}_x_x_x.unitypackage`.<br>
We recommend using the latest version `x_x_x`.<br>
Please import the same version for both the basic package and additional packages.<br>
1. Import `PoseSynth_x_x_x.unitypackage` into a Unity project that has the required packages imported
2. Import the additional package for your VR system
    - Oculus XR Plugin: `ExtensionForOVR_x_x_x.unitypackage`
※Additional packages for other VR systems are currently under development.<br>

## Documents
While the English wiki is currently under development, we have set up a temporary wiki using deepwiki for now. Please refer to this documentation in the meantime: https://deepwiki.com/Cyber-Interface-Lab/PoseSynth/7-setup-and-installation.

## For Researchers
If you publish the results of your research using PoseSynth, please cite the software and related papers.<br>

If you encounter any troubles during use, please feel free to contact us at yamaama@cyber.t.u-tokyo.ac.jp.