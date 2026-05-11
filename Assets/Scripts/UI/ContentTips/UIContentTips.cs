using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
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

        public Dictionary<ImageType,List<string>> predictLabel;

        private void Start()
        {
            backButton.onClick.AddListener(BackUI);
            predictButton.onClick.AddListener(OpenPredict);
        }


        public void SetData(AACDisperseIconToText inData)
        {
            ClearDic();
            
            content.text = inData.Text;
            List<string> predictIconLabel = inData.PredictIconLabel.IconLabel.ToList();

            for (int i = 0; i < predictIconLabel.Count; i++)
            {
                DataManager.GetIconByName(predictIconLabel[i], out var icon);
                if (icon == null)
                {
                    Debug.Log("null");
                    continue;
                }
                if(predictLabel.ContainsKey(icon.Category))
                {
                    predictLabel[icon.Category].Add(icon.IconLabel);
                }
                else
                {
                    predictLabel[icon.Category] = new List<string> { icon.IconLabel };
                }
            }
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

            foreach (var kv in predictLabel)
            {
                if (aacSelectPanel.predictPanel.predictIconPanel.ContainsKey(kv.Key))
                {
                    aacSelectPanel.predictPanel.predictIconPanel[kv.Key].imagePanel.SetData(kv.Value);
                }
                else
                {
                    aacSelectPanel.predictPanel.predictIconPanel[ImageType.Other].imagePanel.SetData(kv.Value);
                }
            }

            OnClose();
        }

        private void ClearDic()
        {
            if (predictLabel != null)
            {
                if (predictLabel.Count > 0)
                {
                    predictLabel.Clear();
                }
            }
            else
            {
                predictLabel = new Dictionary<ImageType, List<string>>();
            }
        }

        private void OnClose()
        {
            this.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            backButton.onClick.RemoveListener(BackUI);
            predictButton.onClick.RemoveListener(OpenPredict);
        }

        private void OnApplicationQuit()
        {
            backButton.onClick.RemoveListener(BackUI);
            predictButton.onClick.RemoveListener(OpenPredict);
        }
    }
}