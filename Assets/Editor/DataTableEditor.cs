using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Unity编辑器窗口,用于可视化编辑数据表
/// 类似于UE的DataTable编辑器
/// </summary>
public class DataTableEditor : EditorWindow
{
    private class EditorData
    {
        public Type DataTableType;
        public object DataTableInstance;
        public List<object> Items;
        public Vector2 ScrollPosition;
        public int SelectedIndex = -1;
        public bool ShowSearchBar = true;
        public string SearchText = "";
        public Dictionary<object, bool> ExpandedItems = new Dictionary<object, bool>();
        public object CurrentEditingItem; // 当前正在编辑的项
        public PropertyInfo CurrentEditingProperty; // 当前正在编辑的属性
    }

    private EditorData _currentData;
    private List<Type> _dataTableTypes;
    private int _selectedTypeIndex = 0;
    private bool _needRefresh = true;

    [MenuItem("Tools/DataTable Editor")]
    public static void ShowWindow()
    {
        GetWindow<DataTableEditor>("DataTable Editor").Show();
    }

    private void OnEnable()
    {
        RefreshDataTableTypes();
    }

    private void OnGUI()
    {
        if (_needRefresh)
        {
            RefreshDataTableTypes();
            _needRefresh = false;
        }

        EditorGUILayout.Space(10);
        DrawHeader();
        EditorGUILayout.Space(10);
        DrawContent();
    }

    private void DrawHeader()
    {
        EditorGUILayout.LabelField("DataTable Editor", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This is a tool for editing DataTable classes.", MessageType.Info);
    }

    private void DrawContent()
    {
        // DataTable类型选择
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Select DataTable:", GUILayout.Width(120));
        string[] typeNames = _dataTableTypes.Select(t => t.Name).ToArray();
        int newIndex = EditorGUILayout.Popup(_selectedTypeIndex, typeNames);
        if (newIndex != _selectedTypeIndex)
        {
            _selectedTypeIndex = newIndex;
            LoadDataTable();
        }

        EditorGUILayout.EndHorizontal();

        if (_dataTableTypes.Count == 0)
        {
            EditorGUILayout.HelpBox("No DataTable classes found. Create a class that inherits from DataTable<T>",
                MessageType.Warning);
            return;
        }

        // 操作按钮
        bool shouldClear = false;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load", GUILayout.Width(80)))
        {
            LoadDataTable();
        }

        if (GUILayout.Button("Save", GUILayout.Width(80)))
        {
            SaveDataTable();
        }

        if (GUILayout.Button("Add Item", GUILayout.Width(80)))
        {
            AddNewItem();
        }

        if (GUILayout.Button("Clear All", GUILayout.Width(80)))
        {
            shouldClear = true;
        }

        if (GUILayout.Button("Import CSV", GUILayout.Width(100)))
        {
            ImportFromCSV();
        }

        if (GUILayout.Button("Export CSV", GUILayout.Width(100)))
        {
            ExportToCSV();
        }

        EditorGUILayout.EndHorizontal();

        // 在布局块外处理删除操作
        if (shouldClear)
        {
            if (EditorUtility.DisplayDialog("Confirm Clear", "Are you sure you want to clear all items?", "Yes", "No"))
            {
                ClearDataTable();
            }
        }

        EditorGUILayout.Space(5);

        // 绘制数据表格
        if (_currentData != null && _currentData.Items != null)
        {
            DrawDataTable();
        }
    }

    private void DrawDataTable()
    {
        if (_currentData.Items.Count == 0)
        {
            EditorGUILayout.HelpBox("No items in this DataTable. Click 'Add Item' to add one.", MessageType.Info);
            return;
        }

        // 搜索框
        _currentData.SearchText = EditorGUILayout.TextField("Search:", _currentData.SearchText);

        EditorGUILayout.Space(5);

        Type itemType = GetItemType(_currentData.DataTableType);
        PropertyInfo[] properties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        // 过滤掉DataTableBase的属性
        PropertyInfo[] filteredProperties = properties
            .Where(p => !IsDataTableBaseProperty(p))
            .ToArray();

        // 数据行
        _currentData.ScrollPosition = EditorGUILayout.BeginScrollView(_currentData.ScrollPosition);

        List<int> itemsToDelete = new List<int>();

        for (int i = 0; i < _currentData.Items.Count; i++)
        {
            object item = _currentData.Items[i];

            // 搜索过滤
            if (!string.IsNullOrEmpty(_currentData.SearchText))
            {
                string itemText = JsonConvert.SerializeObject(item);
                if (!itemText.ToLower().Contains(_currentData.SearchText.ToLower()))
                {
                    continue;
                }
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                // 顶部行：显示索引和非列表属性
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField($"#{i + 1}", EditorStyles.boldLabel, GUILayout.Width(50));

                    // 字段编辑 - 只显示非列表属性
                    foreach (var prop in filteredProperties)
                    {
                        if (!IsListType(prop.PropertyType))
                        {
                            DrawPropertyFieldInTable(item, prop);
                        }
                    }

                    GUILayout.FlexibleSpace();

                    // 操作按钮
                    if (GUILayout.Button("Edit", GUILayout.Width(50)))
                    {
                        _currentData.SelectedIndex = i;
                    }

                    if (GUILayout.Button("X", GUILayout.Width(30)))
                    {
                        itemsToDelete.Add(i);
                    }
                }
                EditorGUILayout.EndHorizontal();

                // 列表属性单独显示
                foreach (var prop in filteredProperties)
                {
                    if (IsListType(prop.PropertyType))
                    {
                        EditorGUILayout.Space(5);
                        EditorGUI.indentLevel++;

                        // 生成折叠key
                        string foldoutKey = $"{prop.Name}_{i}";

                        // 获取或创建展开状态
                        bool isExpanded;
                        if (!_currentData.ExpandedItems.TryGetValue(foldoutKey, out isExpanded))
                        {
                            isExpanded = true; // 默认展开
                            _currentData.ExpandedItems[foldoutKey] = isExpanded;
                        }

                        // 绘制折叠标题
                        string icon = isExpanded ? "[-] " : "[+] ";
                        string foldoutText = $"{icon}{prop.Name}";
                        isExpanded = EditorGUILayout.Foldout(isExpanded, foldoutText, true, EditorStyles.boldLabel);
                        _currentData.ExpandedItems[foldoutKey] = isExpanded;

                        if (isExpanded)
                        {
                            GUILayout.Space(10);
                            Type listElementType = prop.PropertyType.GetGenericArguments()[0];
                            DrawListPropertyFieldCompact(item, prop, listElementType);
                        }

                        EditorGUI.indentLevel--;
                    }
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(2);
        }

        EditorGUILayout.EndScrollView();

        // 在循环外处理删除操作
        if (itemsToDelete.Count > 0)
        {
            foreach (int index in itemsToDelete)
            {
                if (EditorUtility.DisplayDialog("Confirm Delete", $"Delete item {index + 1}?", "Yes", "No"))
                {
                    RemoveItem(index);
                    break;
                }
            }
        }
    }

    private void DrawPropertyFieldInTable(object item, PropertyInfo prop)
    {
        object value = prop.GetValue(item);
        object newValue = value;

        // 显示属性名称和编辑控件
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        EditorGUILayout.LabelField(prop.Name, EditorStyles.boldLabel);

        // 检查是否是List<T>类型
        if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type listElementType = prop.PropertyType.GetGenericArguments()[0];
            DrawListPropertyFieldCompact(item, prop, listElementType);
        }
        else if (prop.PropertyType == typeof(string))
        {
            newValue = EditorGUILayout.TextField((string)(value ?? ""));
        }
        else if (prop.PropertyType == typeof(int))
        {
            newValue = EditorGUILayout.IntField((int)(value ?? 0));
        }
        else if (prop.PropertyType == typeof(float))
        {
            newValue = EditorGUILayout.FloatField((float)(value ?? 0f));
        }
        else if (prop.PropertyType == typeof(bool))
        {
            newValue = EditorGUILayout.Toggle((bool)(value ?? false));
        }
        else if (prop.PropertyType.IsEnum)
        {
            newValue = EditorGUILayout.EnumPopup((Enum)(value ?? Activator.CreateInstance(prop.PropertyType)));
        }
        else
        {
            EditorGUILayout.LabelField(value?.ToString() ?? "null");
        }

        EditorGUILayout.EndVertical();

        if (newValue != value && newValue != null)
        {
            prop.SetValue(item, newValue);
        }
    }

    private void DrawListPropertyFieldCompact(object item, PropertyInfo prop, Type elementType)
    {
        System.Collections.IList list = prop.GetValue(item) as System.Collections.IList;
        if (list == null)
        {
            list = (System.Collections.IList)Activator.CreateInstance(prop.PropertyType);
            prop.SetValue(item, list);
        }

        // 垂直显示和编辑每个元素
        if (list.Count == 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("[Empty]", GUILayout.Width(100));

            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                object newElement = Activator.CreateInstance(elementType);
                list.Add(newElement);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            List<int> indicesToRemove = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[{i}]", GUILayout.Width(50));

                if (elementType.IsEnum)
                {
                    // 枚举类型 - 使用下拉菜单编辑，向右移动
                    GUILayout.Space(10);
                    Enum enumValue = (Enum)list[i];
                    Enum newEnumValue = EditorGUILayout.EnumPopup(enumValue, GUILayout.Width(200));

                    if (!Equals(enumValue, newEnumValue))
                    {
                        list[i] = newEnumValue;
                    }
                }
                else if (elementType == typeof(string))
                {
                    // 字符串类型，向右移动
                    GUILayout.Space(10);
                    string stringValue = (string)list[i];
                    string newValue = EditorGUILayout.TextField(stringValue, GUILayout.Width(300));

                    if (newValue != stringValue)
                    {
                        list[i] = newValue;
                    }
                }
                else if (elementType == typeof(int))
                {
                    // 整数类型，向右移动
                    GUILayout.Space(10);
                    int intValue = (int)list[i];
                    int newValue = EditorGUILayout.IntField(intValue, GUILayout.Width(100));

                    if (newValue != intValue)
                    {
                        list[i] = newValue;
                    }
                }

                GUILayout.FlexibleSpace();

                // 删除按钮 - 标记要删除的索引
                if (GUILayout.Button("×", GUILayout.Width(25)))
                {
                    indicesToRemove.Add(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            // 移除标记的元素（从后往前删除以避免索引问题）
            for (int j = indicesToRemove.Count - 1; j >= 0; j--)
            {
                list.RemoveAt(indicesToRemove[j]);
            }

            // 底部的添加按钮，也向右移动
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(60);
            if (GUILayout.Button("+ Add", GUILayout.Width(80)))
            {
                object newElement = elementType.IsEnum
                    ? Activator.CreateInstance(elementType)
                    : Activator.CreateInstance(elementType);
                list.Add(newElement);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawPropertyField(object item, PropertyInfo prop)
    {
        object value = prop.GetValue(item);
        object newValue = value;

        // 检查是否是List<T>类型
        if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type listElementType = prop.PropertyType.GetGenericArguments()[0];
            DrawListPropertyField(item, prop, listElementType);
        }
        else if (prop.PropertyType == typeof(string))
        {
            newValue = EditorGUILayout.TextField((string)(value ?? ""), GUILayout.Width(150));
        }
        else if (prop.PropertyType == typeof(int))
        {
            newValue = EditorGUILayout.IntField((int)(value ?? 0), GUILayout.Width(100));
        }
        else if (prop.PropertyType == typeof(float))
        {
            newValue = EditorGUILayout.FloatField((float)(value ?? 0f), GUILayout.Width(100));
        }
        else if (prop.PropertyType == typeof(bool))
        {
            newValue = EditorGUILayout.Toggle((bool)(value ?? false), GUILayout.Width(60));
        }
        else if (prop.PropertyType.IsEnum)
        {
            newValue = EditorGUILayout.EnumPopup((Enum)(value ?? Activator.CreateInstance(prop.PropertyType)),
                GUILayout.Width(120));
        }
        else
        {
            EditorGUILayout.LabelField(value?.ToString() ?? "null", GUILayout.Width(150));
        }

        if (newValue != value && newValue != null)
        {
            prop.SetValue(item, newValue);
        }
    }

    private void DrawListPropertyField(object item, PropertyInfo prop, Type elementType)
    {
        System.Collections.IList list = prop.GetValue(item) as System.Collections.IList;
        if (list == null)
        {
            list = (System.Collections.IList)Activator.CreateInstance(prop.PropertyType);
            prop.SetValue(item, list);
        }

        // 生成一个唯一的key来存储展开状态
        string foldoutKey = $"{prop.Name}_{item.GetHashCode()}";

        // 获取或创建展开状态字典
        if (_currentData.ExpandedItems == null)
        {
            _currentData.ExpandedItems = new Dictionary<object, bool>();
        }

        // 检查是否展开
        bool isExpanded;
        if (!_currentData.ExpandedItems.TryGetValue(foldoutKey, out isExpanded))
        {
            isExpanded = list.Count > 0;
            _currentData.ExpandedItems[foldoutKey] = isExpanded;
        }

        // 绘制折叠按钮
        isExpanded = EditorGUILayout.Foldout(isExpanded, $"{prop.Name} [{list.Count}]", true);
        _currentData.ExpandedItems[foldoutKey] = isExpanded;

        if (isExpanded)
        {
            EditorGUI.indentLevel++;

            // 显示列表中的每个元素 - 垂直排列
            List<int> indicesToRemove = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[{i}]", GUILayout.Width(30));

                if (elementType.IsEnum)
                {
                    // 枚举类型 - 使用枚举下拉菜单
                    Enum enumValue = (Enum)list[i];
                    Enum newEnumValue = EditorGUILayout.EnumPopup(enumValue);

                    if (!Equals(enumValue, newEnumValue))
                    {
                        list[i] = newEnumValue;
                    }
                }
                else if (elementType == typeof(string))
                {
                    // 字符串类型
                    string stringValue = (string)list[i];
                    string newValue = EditorGUILayout.TextField(stringValue);

                    if (newValue != stringValue)
                    {
                        list[i] = newValue;
                    }
                }
                else if (elementType == typeof(int))
                {
                    // 整数类型
                    int intValue = (int)list[i];
                    int newValue = EditorGUILayout.IntField(intValue);

                    if (newValue != intValue)
                    {
                        list[i] = newValue;
                    }
                }
                else if (elementType == typeof(float))
                {
                    // 浮点类型
                    float floatValue = (float)list[i];
                    float newValue = EditorGUILayout.FloatField(floatValue);

                    if (!Mathf.Approximately(newValue, floatValue))
                    {
                        list[i] = newValue;
                    }
                }
                else
                {
                    // 其他类型 - 显示文本
                    EditorGUILayout.LabelField(list[i]?.ToString() ?? "null");
                }

                GUILayout.FlexibleSpace();

                // 删除按钮
                if (GUILayout.Button("×", GUILayout.Width(25)))
                {
                    indicesToRemove.Add(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            // 移除标记的元素
            for (int i = indicesToRemove.Count - 1; i >= 0; i--)
            {
                list.RemoveAt(indicesToRemove[i]);
            }

            // 添加新元素按钮
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(30);
            if (GUILayout.Button("+ Add", GUILayout.Width(60)))
            {
                object newElement = elementType.IsEnum
                    ? Activator.CreateInstance(elementType)
                    : Activator.CreateInstance(elementType);
                list.Add(newElement);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }
    }

    private void RefreshDataTableTypes()
    {
        _dataTableTypes = new List<Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract)
                    .Where(t => t.BaseType != null &&
                                t.BaseType.IsGenericType &&
                                t.BaseType.GetGenericTypeDefinition() == typeof(DataTableBase<>))
                    .ToList();
                _dataTableTypes.AddRange(types);
            }
            catch
            {
                continue;
            }
        }
    }

    private void LoadDataTable()
    {
        if (_selectedTypeIndex >= _dataTableTypes.Count) return;

        Type tableType = _dataTableTypes[_selectedTypeIndex];
        _currentData = new EditorData
        {
            DataTableType = tableType,
            DataTableInstance = Activator.CreateInstance(tableType),
            Items = new List<object>(),
            ScrollPosition = Vector2.zero,
            SelectedIndex = -1
        };

        // 从默认路径加载
        MethodInfo loadMethod = tableType.GetMethod("LoadFromJson");
        if (loadMethod != null)
        {
            MethodInfo getSavePathMethod = tableType.GetMethod("GetDefaultSavePath");
            string path = getSavePathMethod?.Invoke(_currentData.DataTableInstance, null) as string;

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    loadMethod.Invoke(_currentData.DataTableInstance, new object[] { path });
                    _currentData.Items = GetItemsFromTable(_currentData.DataTableInstance);
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("Warning", $"Failed to load data file: {ex.Message}\n\nA new empty item has been created.", "OK");
                    // 加载失败时，默认添加一个空项
                    AddNewItem();
                }
            }
            else
            {
                // 如果没有数据文件，默认添加一个空项
                AddNewItem();
            }
        }
    }

    private void SaveDataTable()
    {
        if (_currentData == null) return;

        MethodInfo getSavePathMethod = _currentData.DataTableType.GetMethod("GetDefaultSavePath");
        string path = getSavePathMethod?.Invoke(_currentData.DataTableInstance, null) as string;

        if (!string.IsNullOrEmpty(path))
        {
            // 将编辑的数据同步回DataTable
            SetItemsToTable(_currentData.DataTableInstance, _currentData.Items);

            MethodInfo saveMethod = _currentData.DataTableType.GetMethod("SaveToJson");
            saveMethod?.Invoke(_currentData.DataTableInstance, new object[] { path });

            EditorUtility.DisplayDialog("Success", $"DataTable saved to: {path}", "OK");
        }
    }

    private void AddNewItem()
    {
        if (_currentData == null) return;

        Type itemType = GetItemType(_currentData.DataTableType);
        object newItem = Activator.CreateInstance(itemType);
        _currentData.Items.Add(newItem);

        MethodInfo addMethod = _currentData.DataTableType.GetMethod("AddItem");
        addMethod?.Invoke(_currentData.DataTableInstance, new object[] { newItem });
    }

    private void RemoveItem(int index)
    {
        if (_currentData == null || index < 0 || index >= _currentData.Items.Count) return;

        object item = _currentData.Items[index];
        _currentData.Items.RemoveAt(index);

        MethodInfo removeMethod = _currentData.DataTableType.GetMethod("RemoveItem");
        // 这里需要获取ID,暂时简化处理
    }

    private void ClearDataTable()
    {
        if (_currentData == null) return;

        _currentData.Items.Clear();
        MethodInfo clearMethod = _currentData.DataTableType.GetMethod("Clear");
        clearMethod?.Invoke(_currentData.DataTableInstance, null);
    }

    private List<object> GetItemsFromTable(object dataTable)
    {
        // 直接获取私有字段
        FieldInfo itemsField = dataTable.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
        if (itemsField != null)
        {
            object items = itemsField.GetValue(dataTable);
            // 转换为 List<object>
            if (items != null)
            {
                Type itemsType = items.GetType();
                if (itemsType.IsGenericType && itemsType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type itemType = itemsType.GetGenericArguments()[0];
                    // 转换每个元素为 object
                    MethodInfo castMethod = typeof(Enumerable).GetMethod("Cast")?.MakeGenericMethod(itemType);
                    MethodInfo toListMethod = typeof(Enumerable).GetMethod("ToList")?.MakeGenericMethod(typeof(object));

                    if (castMethod != null && toListMethod != null)
                    {
                        object castedItems = castMethod.Invoke(null, new object[] { items });
                        object list = toListMethod.Invoke(null, new object[] { castedItems });
                        return list as List<object>;
                    }
                }
            }
        }

        return new List<object>();
    }

    private void SetItemsToTable(object dataTable, List<object> items)
    {
        // 直接使用字段而不是属性，因为Items属性没有setter
        FieldInfo itemsField = dataTable.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
        if (itemsField != null)
        {
            Type itemType = GetItemType(dataTable.GetType());
            MethodInfo castMethod = typeof(Enumerable).GetMethod("Cast")?.MakeGenericMethod(itemType);
            MethodInfo toListMethod = typeof(Enumerable).GetMethod("ToList")?.MakeGenericMethod(itemType);

            if (castMethod != null && toListMethod != null)
            {
                object castedItems = castMethod.Invoke(null, new object[] { items });
                object list = toListMethod.Invoke(null, new object[] { castedItems });

                itemsField.SetValue(dataTable, list);
            }
        }
    }

    private Type GetItemType(Type tableType)
    {
        if (tableType.IsGenericType && tableType.GetGenericTypeDefinition() == typeof(DataTableBase<>))
        {
            return tableType.GetGenericArguments()[0];
        }

        // 处理非泛型类的情况(如ExampleDataTable继承DataTableBase<ExampleDataItem>)
        if (tableType.BaseType != null &&
            tableType.BaseType.IsGenericType &&
            tableType.BaseType.GetGenericTypeDefinition() == typeof(DataTableBase<>))
        {
            return tableType.BaseType.GetGenericArguments()[0];
        }

        return null;
    }

    private void ImportFromCSV()
    {
        if (_currentData == null)
        {
            EditorUtility.DisplayDialog("Error", "Please load a DataTable first.", "OK");
            return;
        }

        // 打开文件选择对话框
        string filePath = EditorUtility.OpenFilePanel("Import CSV", "", "csv");
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        try
        {
            // 读取CSV文件 - 优先使用GB2312(Excel默认编码)
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string fileContent = null;

            // 优先尝试GB2312 (Excel中文版默认编码)
            try
            {
                fileContent = System.Text.Encoding.GetEncoding("GB2312").GetString(fileBytes);
            }
            catch
            {
                // 如果GB2312失败,尝试UTF-8
                fileContent = System.Text.Encoding.UTF8.GetString(fileBytes);
                // 检查是否包含BOM
                if (fileBytes.Length >= 3 && fileBytes[0] == 0xEF && fileBytes[1] == 0xBB && fileBytes[2] == 0xBF)
                {
                    fileContent = fileContent.Substring(1); // 移除BOM
                }
            }

            string[] lines = fileContent.Split('\n');

            if (lines.Length < 2)
            {
                EditorUtility.DisplayDialog("Error", "CSV file must have at least a header row and one data row.", "OK");
                return;
            }

            Type itemType = GetItemType(_currentData.DataTableType);
            PropertyInfo[] properties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 过滤掉DataTableBase<>中声明的属性（DataItems, Items等）
            PropertyInfo[] filteredProperties = properties
                .Where(p => !IsDataTableBaseProperty(p))
                .ToArray();

            if (filteredProperties.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No importable properties found in the data item type.", "OK");
                return;
            }

            // 解析表头
            string[] headers = ParseCSVLine(lines[0]);

            // 验证CSV列是否与属性匹配
            List<PropertyInfo> matchedProperties = new List<PropertyInfo>();
            foreach (string header in headers)
            {
                string trimmedHeader = header.Trim();
                PropertyInfo prop = filteredProperties.FirstOrDefault(p => p.Name.Equals(trimmedHeader, StringComparison.OrdinalIgnoreCase));
                if (prop != null)
                {
                    matchedProperties.Add(prop);
                }
            }

            if (matchedProperties.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No matching columns found in CSV.", "OK");
                return;
            }

            // 询问用户是否清除现有数据
            bool clearExisting = EditorUtility.DisplayDialog(
                "Import Options",
                $"Found {matchedProperties.Count} matching columns. Clear existing data before import?",
                "Clear & Import",
                "Append");

            if (clearExisting)
            {
                _currentData.Items.Clear();
                MethodInfo clearMethod = _currentData.DataTableType.GetMethod("Clear");
                clearMethod?.Invoke(_currentData.DataTableInstance, null);
            }

            // 解析数据行
            int successCount = 0;
            int failCount = 0;

            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    string[] values = ParseCSVLine(lines[i]);
                    object newItem = Activator.CreateInstance(itemType);

                    for (int j = 0; j < matchedProperties.Count && j < values.Length; j++)
                    {
                        PropertyInfo prop = matchedProperties[j];
                        string value = values[j]?.Trim();

                        // 如果值是空的,跳过(保持默认值)
                        if (string.IsNullOrEmpty(value))
                        {
                            // 对于列表类型,确保有一个空列表
                            if (prop.PropertyType.IsGenericType &&
                                prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                var list = prop.GetValue(newItem) as System.Collections.IList;
                                if (list == null)
                                {
                                    list = (System.Collections.IList)Activator.CreateInstance(prop.PropertyType);
                                    prop.SetValue(newItem, list);
                                }
                            }
                            continue;
                        }

                        object convertedValue = ConvertValue(prop.PropertyType, value);
                        if (convertedValue != null)
                        {
                            prop.SetValue(newItem, convertedValue);
                        }
                    }

                    _currentData.Items.Add(newItem);
                    successCount++;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to import row {i + 1}: {ex.Message}");
                    failCount++;
                }
            }

            // 同步到DataTable实例
            SetItemsToTable(_currentData.DataTableInstance, _currentData.Items);

            EditorUtility.DisplayDialog(
                "Import Complete",
                $"Successfully imported {successCount} items." + (failCount > 0 ? $"\nFailed: {failCount} items" : ""),
                "OK");
        }
        catch (Exception ex)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to import CSV: {ex.Message}", "OK");
            Debug.LogError($"CSV Import Error: {ex}");
        }
    }

    private string[] ParseCSVLine(string line)
    {
        // 处理带引号的CSV字段
        List<string> fields = new List<string>();
        bool inQuotes = false;
        StringBuilder currentField = new StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                // 检查是否是转义的双引号 ("")
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        fields.Add(currentField.ToString());
        return fields.ToArray();
    }

    private object ConvertValue(Type targetType, string value)
    {
        try
        {
            // 检查是否是List<T>类型
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementType = targetType.GetGenericArguments()[0];
                System.Collections.IList list = (System.Collections.IList)Activator.CreateInstance(targetType);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return list;
                }

                // 使用|分隔符分割多个值
                string[] values = value.Split('|');

                foreach (string itemValue in values)
                {
                    string trimmedValue = itemValue.Trim();
                    if (string.IsNullOrEmpty(trimmedValue))
                        continue;

                    object convertedElement = ConvertSingleValue(elementType, trimmedValue);
                    if (convertedElement != null)
                    {
                        list.Add(convertedElement);
                    }
                }

                return list;
            }

            // 处理非列表类型
            return ConvertSingleValue(targetType, value);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to convert value '{value}' to type {targetType.Name}: {ex.Message}");
            return null;
        }
    }

    private object ConvertSingleValue(Type targetType, string value)
    {
        try
        {
            if (targetType == typeof(string))
            {
                return value;
            }
            else if (targetType == typeof(int))
            {
                if (int.TryParse(value, out int intValue))
                    return intValue;
            }
            else if (targetType == typeof(float))
            {
                if (float.TryParse(value, out float floatValue))
                    return floatValue;
            }
            else if (targetType == typeof(bool))
            {
                if (bool.TryParse(value, out bool boolValue))
                    return boolValue;
                else if (value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                         value.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;
                else if (value.Equals("0", StringComparison.OrdinalIgnoreCase) ||
                         value.Equals("false", StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            else if (targetType.IsEnum)
            {
                try
                {
                    return Enum.Parse(targetType, value, true);
                }
                catch
                {
                    // 尝试按数值转换
                    if (int.TryParse(value, out int enumValue))
                    {
                        return Enum.ToObject(targetType, enumValue);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to convert value '{value}' to type {targetType.Name}: {ex.Message}");
        }

        return null;
    }

    private void ExportToCSV()
    {
        if (_currentData == null || _currentData.Items.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No data to export. Please load a DataTable with items first.", "OK");
            return;
        }

        try
        {
            Type itemType = GetItemType(_currentData.DataTableType);
            PropertyInfo[] properties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 过滤掉DataTableBase<>中声明的属性（DataItems, Items等）
            PropertyInfo[] filteredProperties = properties
                .Where(p => !IsDataTableBaseProperty(p))
                .ToArray();

            if (filteredProperties.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No properties found in the data item type.", "OK");
                return;
            }

            // 创建导出目录
            string exportDir = "./CSV";
            if (!Directory.Exists(exportDir))
            {
                Directory.CreateDirectory(exportDir);
            }

            // 生成文件名
            string fileName = $"{_currentData.DataTableType.Name}.csv";
            string filePath = Path.Combine(exportDir, fileName);

            // 如果文件已存在，询问是否覆盖
            if (File.Exists(filePath))
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "File Exists",
                    $"File '{fileName}' already exists. Do you want to overwrite it?",
                    "Overwrite",
                    "Cancel");

                if (!overwrite)
                {
                    return;
                }
            }

            // 构建CSV内容
            StringBuilder csvContent = new StringBuilder();

            // 写入表头
            List<string> headers = new List<string>();
            foreach (var prop in filteredProperties)
            {
                headers.Add(EscapeCSVField(prop.Name));
            }
            csvContent.AppendLine(string.Join(",", headers));

            // 写入数据行
            foreach (var item in _currentData.Items)
            {
                List<string> values = new List<string>();
                foreach (var prop in filteredProperties)
                {
                    object value = prop.GetValue(item);
                    string stringValue = FormatValueForCSV(value);
                    values.Add(EscapeCSVField(stringValue));
                }
                csvContent.AppendLine(string.Join(",", values));
            }

            // 写入文件 - 使用GB2312编码(中文Windows系统默认编码)
            File.WriteAllText(filePath, csvContent.ToString(), System.Text.Encoding.GetEncoding("GB2312"));

            // 询问是否在资源管理器中打开文件
            bool openFolder = EditorUtility.DisplayDialog(
                "Export Successful",
                $"Successfully exported {_currentData.Items.Count} items to:\n{filePath}",
                "Open Folder",
                "OK");

            if (openFolder)
            {
                EditorUtility.RevealInFinder(filePath);
            }

            // 刷新Unity资源
            AssetDatabase.Refresh();

            Debug.Log($"DataTable exported to CSV: {filePath}");
        }
        catch (Exception ex)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to export CSV: {ex.Message}", "OK");
            Debug.LogError($"CSV Export Error: {ex}");
        }
    }

    private bool IsDataTableBaseProperty(PropertyInfo prop)
    {
        // 检查属性是否在DataTableBase<>中声明
        Type declaringType = prop.DeclaringType;

        if (declaringType == null)
        {
            return false;
        }

        // 如果声明类型是泛型的且是DataTableBase<>（如 DataTable<MaterialDefine>）
        if (declaringType.IsGenericType &&
            declaringType.GetGenericTypeDefinition() == typeof(DataTableBase<>))
        {
            return true;
        }

        return false;
    }

    private string FormatValueForCSV(object value)
    {
        if (value == null)
        {
            return "";
        }

        // 检查是否是List<T>类型
        if (value is System.Collections.IList list && value.GetType().IsGenericType &&
            value.GetType().GetGenericTypeDefinition() == typeof(List<>))
        {
            // 将列表元素用|分隔符连接
            var values = new System.Collections.Generic.List<string>();
            foreach (var item in list)
            {
                values.Add(FormatValueForCSV(item));
            }
            return string.Join("|", values);
        }

        if (value is Enum enumValue)
        {
            // 枚举导出为名称
            return enumValue.ToString();
        }
        else if (value is bool boolValue)
        {
            // 布尔值导出为 true/false
            return boolValue.ToString().ToLower();
        }
        else
        {
            return value.ToString();
        }
    }

    private bool IsListType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }

    private string EscapeCSVField(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return "";
        }

        // 如果字段包含逗号、换行符或双引号，需要用双引号包裹并转义双引号
        if (field.Contains(",") || field.Contains("\n") || field.Contains("\"") || field.Contains("\r"))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}