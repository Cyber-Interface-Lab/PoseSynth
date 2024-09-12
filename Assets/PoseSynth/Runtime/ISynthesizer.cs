namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Interface who can synthesize Poses.
    /// e.g. PoseMixer, PoseRemapper.
    /// </summary>
    /// <seealso cref="PoseMixer" cref="PoseRemapper"/>
    public interface ISynthesizer
    {
        /// <summary>
        /// Update Pose in every FixedUpdate if true.
        /// </summary>
        bool IsValid { get; set; }
        /// <summary>
        /// Target Pose that this synthesizer is synthesizing.
        /// </summary>
        Pose Pose { get; set; }
    }
}