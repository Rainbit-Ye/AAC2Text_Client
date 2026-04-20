using System;
using System.Collections.Generic;
using DataTable;
using ImageCache;
using Manager;
using UI.AACSelect.AllSelectPanel;
using UI.AACSelect.UIAllSelectPanel;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ImageType
{
    None,
    Function,
    Entity,
    Action,
    Other,
}
namespace UI.AACSelect.AllSelectPanel
{
    public class ImagePanel : MonoBehaviour
    {
        public CategoryPanel owner;
        public List<AACImagePrefabs> selectedImages;

        public void SetData()
        {
            DataManager.GetIconListByType(owner.panelType,out var iconList);

            iconList.Sort((a, b) => Random.Range(-1, 2));

            for (int i = 0; i < 4; i++)
            {
                var icon = iconList[i];
                selectedImages[i].SetData(icon.IconLabel);
                selectedImages[i].owner = this;
            }

        }
    }
}