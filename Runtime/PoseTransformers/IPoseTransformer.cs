namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Interface who can transform a <see cref="Pose"/>.
    /// See <see cref="PoseMapper"/>, <see cref="PoseRemapper"/>, and <see cref="PoseMixer"/>.
    public interface IPoseTransformer
    {
        /// <summary>
        /// Can update <see cref="Pose"/> if true.
        /// </summary>
        bool IsValid { get; set; }
        /// <summary>
        /// Target Pose to be transformed.
        /// </summary>
        Pose Pose { get; set; }
    }
}