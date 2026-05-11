using System.Collections.Generic;
using UI.AACSelect.AllSelectPanel;
using UnityEngine;

namespace UI.AACSelect.UIPredictPanel
{
    public class PredictPanel : MonoBehaviour
    {
        public AACSelectPanel owner;
        public Dictionary<ImageType,PredictIconPanel> predictIconPanel;

        private void Start()
        {
            if (predictIconPanel == null)
            {
                AddCategoryPanel();
            }
        }

        public void Init()
        {
            predictIconPanel = new Dictionary<ImageType, PredictIconPanel>();
            AddCategoryPanel();
        }
        private void AddCategoryPanel()
        {
            var childes = transform.GetComponentsInChildren<PredictIconPanel>();
            foreach (var child in childes)
            {
                child.owner = this;
                predictIconPanel[child.panelType] = child;
            }
        }
        
    }
}