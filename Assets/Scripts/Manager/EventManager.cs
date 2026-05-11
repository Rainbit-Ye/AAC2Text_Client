using System;
using System.Collections.Generic;
using UI;
using UI.AACSelect;
using Unity.VisualScripting;
using UnityEngine;

namespace Manager
{
    public class EventManager : Singleton<EventManager>
    {
        
        public void OpenSelectUI()
        {
            UIManager.Ins.Show<AACSelectPanel>();
        }
        
    }
}