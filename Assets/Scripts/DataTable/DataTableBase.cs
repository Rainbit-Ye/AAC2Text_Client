using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 数据表基类,提供通用的数据表管理功能
/// </summary>
/// <typeparam name="T">数据项类型</typeparam>
public abstract class DataTableBase<T> where T : class, new()
{
    protected Dictionary<int, T> _dataItems;
    protected int _nextId = 1;

    protected List<T> _items = new List<T>();

    public DataTableBase()
    {
        _dataItems = new Dictionary<int, T>();
        _items = new List<T>();
    }

    /// <summary>
    /// 获取指定ID的数据项
    /// </summary>
    public T GetData(int id)
    {
        if (_dataItems.TryGetValue(id, out T data))
        {
            return data;
        }

        return null;
    }

    /// <summary>
    /// 添加数据项
    /// </summary>
    public int AddItem(T item)
    {
        int id = _nextId++;
        _dataItems[id] = item;
        _items.Add(item);
        return id;
    }

    /// <summary>
    /// 移除数据项
    /// </summary>
    public bool RemoveItem(int id)
    {
        if (_dataItems.TryGetValue(id, out T item))
        {
            _dataItems.Remove(id);
            _items.Remove(item);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 更新数据项
    /// </summary>
    public bool UpdateItem(int id, T newItem)
    {
        if (_dataItems.ContainsKey(id))
        {
            int index = _items.IndexOf(_dataItems[id]);
            if (index >= 0)
            {
                _items[index] = newItem;
            }

            _dataItems[id] = newItem;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取所有数据项
    /// </summary>
    public List<T> GetAllItems()
    {
        return new List<T>(_items);
    }

    /// <summary>
    /// 清空所有数据
    /// </summary>
    public void Clear()
    {
        _dataItems.Clear();
        _items.Clear();
        _nextId = 1;
    }

    /// <summary>
    /// 保存为JSON文件
    /// </summary>
    public void SaveToJson(string filePath)
    {
        try
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonConvert.SerializeObject(_items, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Debug.Log($"DataTable saved to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save DataTable: {e.Message}");
        }
    }

    /// <summary>
    /// 从JSON文件加载
    /// </summary>
    public void LoadFromJson(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                Debug.Log($"Loading JSON from {filePath}, content length: {json.Length}");

                _items = JsonConvert.DeserializeObject<List<T>>(json);

                if (_items == null)
                {
                    Debug.LogWarning("Failed to deserialize JSON, result is null. Creating empty list.");
                    _items = new List<T>();
                }

                // 确保每个列表属性都被初始化
                foreach (var item in _items)
                {
                    var properties = item.GetType().GetProperties();
                    foreach (var prop in properties)
                    {
                        if (prop.PropertyType.IsGenericType &&
                            prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var list = prop.GetValue(item) as System.Collections.IList;
                            if (list == null)
                            {
                                list = (System.Collections.IList)Activator.CreateInstance(prop.PropertyType);
                                prop.SetValue(item, list);
                            }
                        }
                    }
                }

                // 重建字典和ID
                _dataItems.Clear();
                _nextId = 1;
                for (int i = 0; i < _items.Count; i++)
                {
                    _dataItems[_nextId] = _items[i];
                    _nextId++;
                }

                Debug.Log($"DataTable loaded from: {filePath}, {_items.Count} items");
            }
            else
            {
                Debug.LogWarning($"File not found: {filePath}");
                AddItem(new T());
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to load DataTable from {filePath}: {e.Message}");
            Debug.LogWarning($"Stack trace: {e.StackTrace}");
            // 加载失败时，添加一个空项
            AddItem(new T());
        }
    }

    /// <summary>
    /// 获取数据表名称(由子类实现)
    /// </summary>
    public abstract string GetTableName();

    /// <summary>
    /// 获取默认保存路径
    /// </summary>
    public virtual string GetDefaultSavePath()
    {
        // 保存到 Data/ 目录（与 DataLevelInfoFunctionLibrary 保持一致的相对路径）
        return Path.Combine("Data/", $"{GetTableName()}.txt");
    }
}