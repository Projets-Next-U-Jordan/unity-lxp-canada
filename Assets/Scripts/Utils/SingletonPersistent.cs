using UnityEngine;

namespace Utils
{
    public class SingletonPersistent<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Try to find the existing instance in the scene
                    _instance = FindObjectOfType<T>();

                    // If no instance exists, create a new GameObject
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                        //DontDestroyOnLoad(obj);
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                // Set this instance as the singleton instance if it's the first one
                _instance = this as T;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Destroy any duplicate instances that are created
                Destroy(gameObject);
            }
        }
    }
}