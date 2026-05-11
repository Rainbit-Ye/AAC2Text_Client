using System;
using Manager;
using TMPro;
using UI.AACSelect.AllSelectPanel;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AACSelect.UIAllSelectPanel
{
    public class CategoryPanel : CategoryPanelBase
    {
        public AllSelectPanel owner;
        public Button expandAllImage;

        

        public void Start()
        {
            if (LoadingManager.IsLoading)
            {
                Init();
            }
            else
            {
                LoadingManager.Ins.OnLoadingComplete += Init; 
            }

        }

        private void OnDestroy()
        {
            LoadingManager.Ins.OnLoadingComplete -= Init;
        }

        private void OnApplicationQuit()
        {
            LoadingManager.Ins.OnLoadingComplete -= Init;
        }
        

        public void Init()
        {
            categoryName.text = panelType.ToString();
            expandAllImage.onClick.AddListener(ExpandAllImage);
            changeImage.onClick.AddListener(ChangeImage);
            imagePanel.owner = this;
            imagePanel.SetData();
        }

        private void ChangeImage()
        {
            Debug.Log("ChangeImage");
            imagePanel.SetData();
        }

        private void ExpandAllImage()
        {
            Debug.Log("ExpandAllImage");
        }
        
        
    }
}