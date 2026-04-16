using System;
using Unity.VisualScripting;

namespace Manager
{
    public class SystemManager : Singleton<SystemManager>
    {
        public void AnalysisMessage(CSID messageID)
        {
            switch (messageID)
            {
                case CSID.AacDispersesIconSend:
                    break;
                case CSID.AacDispersesIconToText:
                    // 可能会有什么逻辑但我没想到
                    break;
            }
        }
    }
}