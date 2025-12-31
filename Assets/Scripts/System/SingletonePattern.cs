using UnityEngine;

public class SingletonePattern<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    protected bool IsMainInstance { get; private set; }
    
    public static T Instance
    {
        get
        {
            //인스턴스가 null이면 먼저 씬에서 찾아봄
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                // 그래도 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            IsMainInstance = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance != this)
            {
                IsMainInstance = false;
                Destroy(gameObject);
            }
        }
    }
}
