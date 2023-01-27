namespace Framework {
    public interface IObserver<in T> {
        void OnNotify(T arg);
    }
}