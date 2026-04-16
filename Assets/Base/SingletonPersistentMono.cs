using UnityEngine;

/// <summary>
/// 实现单例 Mono
/// 会自动挂载在场景里并保证场景切换时不会被销毁
/// 获取时才会自动挂在场景里
/// </summary>
/// <typeparam name="T">需继承MonoBehaviour</typeparam>
public class SingletonPersistentMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Ins
    {
        get { return _instance; }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            // 如果已经存在实例，销毁当前对象
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}