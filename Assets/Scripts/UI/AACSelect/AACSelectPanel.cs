using System;
using UI.AACSelect;
using UI.AACSelect.UIAllSelectPanel;
using UnityEngine;



public class AACSelectPanel : MonoBehaviour
{
    public AACIconSlot iconSlot;
    public AllSelectPanel allSelectPanel;

    private void Start()
    {
        iconSlot.owner = this;
        allSelectPanel.owner = this;
    }
}
