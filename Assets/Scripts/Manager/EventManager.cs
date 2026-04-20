using UI;
using UI.AACSelect;
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