using UnityEngine;
using System;
/// <summary>
/// 实现单例 类 继承此类即可
/// </summary>
/// <typeparam name="T">需可实例化的对象</typeparam>
/// <summary>
/// 线程安全的泛型单例基类（带自动销毁重复实例功能）
/// 支持MonoBehaviour和非MonoBehaviour类型
/// </summary>
public abstract class Singleton<T> where T : class, new()
{
    private static readonly object _lock = new object();
    private static T _instance;

    // 增加初始化标志位
    private static bool _isInitialized = false;

    public static T Ins
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new T();
                    _isInitialized = true;
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// 安全销毁单例（重置状态）
    /// </summary>
    public static void SafeDestroy()
    {
        lock (_lock)
        {
            if (_instance is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if (_instance is MonoBehaviour mono)
            {
                UnityEngine.Object.Destroy(mono.gameObject);
            }

            _instance = null;
            _isInitialized = false;
        }
    }

    // 防止外部实例化
    protected Singleton()
    {
        if (_isInitialized && _instance != null)
        {
            throw new Exception($"禁止重复创建{typeof(T)}单例！");
        }
    }
}