namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// <see cref="ServerCameraRig"/> for <see cref="SimpleLocalCameraRig"/>.
    /// </summary>
    public class SimpleServerCameraRig : ServerCameraRig
    {
        public override CameraRigType Type => CameraRigType.Simple;
    }
}