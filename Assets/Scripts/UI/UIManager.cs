using System;
using System.Collections.Generic;
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
                    Object prefab = Resources.Load(element.Resources, type);
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
    }
}
