namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Commandパターンを実装するためのインタフェースです。
    /// Interface to implement Command pattern.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// クラスによって予め決められた処理を実行します。
        /// Execute the process that is predetermined by the class.
        /// </summary>
        void Execute();
    }
}
