using System;
using TMPro;
using UI.AACSelect;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.ContentTips
{
    public class UIContentTips : UIWindow
    {
        public TMP_Text content;
        public Button backButton;
        public Button predictButton;

        private void Start()
        {
            backButton.onClick.AddListener(BackUI);
            predictButton.onClick.AddListener(OpenPredict);
        }


        public void SetData(string inContent)
        {
            content.text = inContent;
        }
        
        private void BackUI()
        {
            Debug.Log("关闭");
            OnClose();
        }

        private void OpenPredict()
        {
            AACSelectPanel aacSelectPanel = UIManager.Ins.Get<AACSelectPanel>();
            aacSelectPanel.SwitchPanelType(false);
            OnClose();
        }
        
        private void OnClose()
        {
            this.gameObject.SetActive(false);
            backButton.onClick.RemoveListener(BackUI);
            predictButton.onClick.RemoveListener(OpenPredict);
        }

    }
}