using System;
using UI.AACSelect.AllSelectPanel;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AACSelect.UIPredictPanel
{
    public class PredictIconPanel:CategoryPanelBase
    {
        public PredictPanel owner;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            categoryName.text = panelType.ToString();
            changeImage.onClick.AddListener(ChangeImage);
            imagePanel.owner = this;
        }
        

        private void ChangeImage()
        {
            
        }
    }
}