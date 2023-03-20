using UnityEngine;

/// <summary>
/// 这个泛型单例模式实现中，我们将类型约束为继承自 MonoBehaviour，并定义了一个 Instance 属性，该属性会检查是否已经存在该类型的实例，如果没有，则会创建一个新的实例并返回。同时，我们也重写了 Awake 方法，以确保只有一个实例存在。
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(singletonObject);
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


