namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Interface that can transform a <see cref="Target"/>.
    /// See <see cref="PoseMapper"/>, <see cref="PoseRemapper"/>, and <see cref="PoseMixer"/>.
    public interface IPoseTransformer
    {
        /// <summary>
        /// Can update <see cref="Target"/> if true.
        /// </summary>
        bool IsValid { get; set; }
        /// <summary>
        /// Target Pose to be transformed.
        /// </summary>
        Pose Target { get; set; }
    }
}