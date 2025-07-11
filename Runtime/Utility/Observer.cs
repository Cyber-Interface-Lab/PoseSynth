namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// <see cref="IObserver{T}"/>から観測される性質を付与するインターフェースです。
    /// クラス内部の値が変化した際に<see cref="Notify(T)"/>を呼び出すことで、<see cref="IObserver{T}"/>に通知します。
    /// Observable iinterface that can be observed by <see cref="IObserver{T}"/>.
    /// Call <see cref="Notify(T)"/> when the class (T)'s value chaned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObservable<T>
    {
        /// <summary>
        /// <see cref="Notify"/>する際の対象となる<see cref="IObserver{T}"/>を追加します。
        /// </summary>
        /// <param name="observer"></param>
        void AddObserver(IObserver<T> observer);
        /// <summary>
        /// <see cref="Notify"/>する際の対象となる<see cref="IObserver{T}"/>から削除します。
        /// </summary>
        /// <param name="observer"></param>
        void RemoveObserver(IObserver<T> observer);
        /// <summary>
        /// <see cref="IObserver{T}"/>に自身の状態を通知します。
        /// </summary>
        void Notify();
    }
    /// <summary>
    /// <see cref="IObservable{T}"/>を観測できる性質を付与するインターフェースです。
    /// Observer interface that observes <see cref="IObservable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObserver<T>
    {
        /// <summary>
        /// <see cref="IObservable{T}.Notify"/>によって呼び出され、<see cref="T"/>の状態を通知されます。
        /// Be notified by <see cref="IObservable{T}.Notify"/> and get the state of <see cref="T"/>.
        /// </summary>
        /// <param name="observable"></param>
        void OnNotified(T observable);
    }
}
