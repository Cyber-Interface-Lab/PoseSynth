namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// ISynthesizer who can edit pose with delay.
    /// It has a queue of ICommand.
    /// </summary>
    [System.Obsolete]
    public interface IDelayableSynthesizer
    {
        int DelayFixedFrame { get; set; }
        /// <summary>
        /// Initialize the command queue.
        /// </summary>
        void Initialize();
    }
}
