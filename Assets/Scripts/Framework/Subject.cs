using System.Collections.Generic;
using UnityEngine;

namespace Framework {
    public abstract class Subject<T> : MonoBehaviour {
        private Dictionary<int, IObserver<T>> _observers;

        public void AddObserver(IObserver<T> observer) {
            if (!_observers.TryAdd(observer.GetHashCode(), observer)) {
                Debug.LogWarning("Tried to add observer but observer was already in the dictionary.");
            }
        }

        public void RemoveObserver(IObserver<T> observer) {
            int hashCode = observer.GetHashCode();
            if (_observers.ContainsKey(hashCode)) {
                _observers.Remove(hashCode);
            }
            else {
                Debug.LogWarning("Tried to remove a observer that was not there.");
            }
        }

        protected void NotifyObservers(T arg) {
            foreach (IObserver<T> observer in _observers.Values) {
                observer.OnNotify(arg);
            }
        }
    }
}