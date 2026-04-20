using UI.AACSelect.UIPredictPanel;

namespace UI.AACSelect
{
    public class AACSelectPanel : UIWindow
    {
        public AACIconSlot iconSlot;
        public UIAllSelectPanel.AllSelectPanel allSelectPanel;
        public PredictPanel predictPanel;
        private void Start()
        {
            iconSlot.owner = this;
            allSelectPanel.owner = this;
            predictPanel.owner = this;
        }

        /// <summary>
        ///  true：打开全选 false：打开预测
        /// </summary>
        /// <param name="v"></param>
        public void SwitchPanelType(bool v)
        {
            allSelectPanel.gameObject.SetActive(v);
            predictPanel.gameObject.SetActive(!v);
        }
    }
}
