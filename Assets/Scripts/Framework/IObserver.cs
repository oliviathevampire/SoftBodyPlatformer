namespace Framework {
    public interface IObserver<in T> {
        void OnNotify(T arg);
    }

    public interface IObserver {
        void OnNotify();
    }
}