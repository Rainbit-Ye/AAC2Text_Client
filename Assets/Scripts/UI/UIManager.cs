using System;
using System.Collections.Generic;
using UI.AACSelect;
using UI.ContentTips;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class UIManager : SingletonPersistentMono<UIManager>
    {
        class UIElement
        {
            public string Resources;
            public bool Cache;
            public GameObject prefabs;
        }
        
        private Dictionary<Type, UIElement> _UIElements = new Dictionary<Type, UIElement>();

        public UIManager()
        {
            _UIElements.Add(typeof(AACSelectPanel), new UIElement() { Resources = "UI/UIAACSelectPanel", Cache = true });
            _UIElements.Add(typeof(UIContentTips), new UIElement() { Resources = "UI/UIContentTips", Cache = true });
        }
        ~UIManager(){}

        /// <summary>
        /// 生成对应UI组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Show<T>()
        {
            Type type = typeof(T);
            if (_UIElements.ContainsKey(type))
            {
                UIElement element = _UIElements[type];
                if (element.prefabs != null)
                {
                    element.prefabs.SetActive(true);
                }
                else
                {
                    Object prefab = Resources.Load(element.Resources);
                    if (prefab == null)
                    {
                        return default(T);
                    }
                    element.prefabs = (GameObject)Instantiate(prefab);
                }
                return element.prefabs.GetComponent<T>();
            }
            return default(T);
        }

        public void Close(Type type)
        {
            if (_UIElements.ContainsKey(type))
            {
                UIElement element = _UIElements[type];
                if (element.Cache)
                {
                    element.prefabs.SetActive(false);
                }
                else
                {
                    Destroy(element.prefabs);
                    element.prefabs = null;
                }
            }
        }
        
        public T Get<T>() where T : Component
        {
            Type type = typeof(T);
            if (_UIElements.ContainsKey(type))
            {
                UIElement info = _UIElements[type];
                if (info.prefabs != null && info.prefabs.activeInHierarchy)
                {
                    return info.prefabs.GetComponent<T>();
                }
            }

            return null;
        }

        /// <summary>
        /// 获取当前已存在的UI实例（即使未激活也返回）
        /// </summary>
        /// <typeparam name="T">UI类型</typeparam>
        /// <param name="includeInactive">是否包含未激活的实例</param>
        /// <returns>UI实例，如果不存在返回null</returns>
        public T Get<T>(bool includeInactive) where T : Component
        {
            Type type = typeof(T);
            if (_UIElements.ContainsKey(type))
            {
                UIElement info = _UIElements[type];
                if (info.prefabs != null && (includeInactive || info.prefabs.activeInHierarchy))
                {
                    return info.prefabs.GetComponent<T>();
                }
            }

            return null;
        }
    }
}
