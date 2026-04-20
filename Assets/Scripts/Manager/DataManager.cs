using System.Collections.Generic;
using System.IO;
using DataTable;
using Newtonsoft.Json;
using UnityEngine;
using Utilities;

namespace Manager
{
    public class DataManager
    {
        private static Dictionary<string,IconLabelDataTable> ImageNameList = new Dictionary<string,IconLabelDataTable>();
        private static Dictionary<ImageType,List<IconLabelDataTable>> IconTypeList = new Dictionary<ImageType,List<IconLabelDataTable>>();
        public static void InitDataTable()
        {
            InitImageList();
        }
        public static void InitImageList()
        {
            try
            {
                string filePath = Config.DataTablePath + "IconLabelDataTable.txt";
                Debug.Log($"尝试读取文件: {filePath}");
                
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var dataList = JsonConvert.DeserializeObject<List<IconLabelDataTable>>(json);
                    ImageNameList.Clear();
                    if (dataList != null &&  dataList.Count > 0)
                    {
                        foreach (var define in dataList)
                        {
                            string labelName = define.IconLabel;
                            ImageType category = define.Category;

                            if(IconTypeList.ContainsKey(category))
                            {
                                IconTypeList[category].Add(define);
                            }
                            else
                            {
                                IconTypeList[category] = new List<IconLabelDataTable>();
                                IconTypeList[category].Add(define);
                            }
                            
                            // 将当前对象添加到对应的内层字典中
                            ImageNameList[labelName] = define;
                        }
                    }
                    Debug.Log($"成功加载图片名称列表，数量: {ImageNameList.Count}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"InitImageList 异常: {e}");
            }
        }
        
        
        
        public static void GetImageNameList(out Dictionary<string,IconLabelDataTable> imageNameList)
        {
            imageNameList = ImageNameList;
        }

        public static void GetIconListByType(ImageType type, out List<IconLabelDataTable> iconList)
        {
            if (IconTypeList.TryGetValue(type, out var list))
            {
                iconList = list;
            }
            else
            {
                iconList = new List<IconLabelDataTable>();
            }
        }
    }
}