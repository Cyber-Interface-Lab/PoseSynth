namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Observable iinterface.
    /// This can be observed by <see cref="IObserver{T}"/>.
    /// Call <see cref="Notify(T)"/> when the class (T)'s value chaned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObservable<T>
    {
        void AddObserver(IObserver<T> observer);
        void RemoveObserver(IObserver<T> observer);
        void Notify();
    }
    /// <summary>
    /// Observer interface.
    /// It observes <see cref="IObservable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObserver<T>
    {
        /// <summary>
        /// Called at <see cref="IObservable{T}.Notify(T)"/>.
        /// </summary>
        /// <param name="observer"></param>
        void OnNotified(T observer);
    }
}
