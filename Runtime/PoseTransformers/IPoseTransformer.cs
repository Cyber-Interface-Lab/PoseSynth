namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// <see cref="Pose"/>を変換するインターフェースです。
    /// Interface to transform a <see cref="Pose"/>.
    public interface IPoseTransformer
    {
        /// <summary>
        /// 有効な場合、<see cref="Target"/>を更新できます。
        /// Can update <see cref="Target"/> if true.
        /// </summary>
        bool IsValid { get; set; }
        /// <summary>
        /// 変換先の<see cref="Pose"/>です。
        /// Target <see cref="Pose"/> to be transformed.
        /// </summary>
        Pose Target { get; set; }
    }
}