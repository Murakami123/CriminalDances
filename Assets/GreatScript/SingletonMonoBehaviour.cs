using UnityEngine;

namespace GreatScript
{
    /// ------------------------------
    /// SingleTonのジェネリッククラス
    ///
    /// ＊必ず継承をして使ってください
    /// ------------------------------
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        Debug.LogError(typeof(T) + "is nothing");
                    }
                }
                return instance;
            }
        }
        void Awake()
        {
            if (instance != null && instance != this)
            {
                if (this.gameObject)
                    Destroy(gameObject);
                return;
            }
        }
        public void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }
    }
}
