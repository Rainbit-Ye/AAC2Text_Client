using System;
using UI;
using UI.ContentTips;
using Unity.VisualScripting;
using UnityEngine;

namespace Manager
{
    public class SystemManager : Singleton<SystemManager>
    {
        public void AnalysisMessage<T>(CSID messageID,T response)
        {
            switch (messageID)
            {
                case CSID.AacDispersesIconToText:
                    AACDisperseIconToText aacDisperseIconToText = response as AACDisperseIconToText;
                    
                    UIContentTips tips = UIManager.Ins.Show<UIContentTips>();
                    tips.SetData(aacDisperseIconToText);

                    AACPredictIconLabel aacPredictIconLabel = aacDisperseIconToText.PredictIconLabel;
                    Debug.Log($"预测结果: {aacPredictIconLabel.IconLabel}");
                    
                    break;
                case CSID.ConnectSuccessful:
                    // 可能会有什么逻辑但我没想到
                    break;
            }
        }
    }
}