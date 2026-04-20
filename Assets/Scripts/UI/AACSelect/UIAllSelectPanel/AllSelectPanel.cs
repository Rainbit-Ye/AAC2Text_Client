using System.Collections.Generic;
using UI.AACSelect.AllSelectPanel;
using UnityEngine;

namespace UI.AACSelect.UIAllSelectPanel
{
    public class AllSelectPanel : MonoBehaviour
    {
        public AACSelectPanel owner;
        public List<CategoryPanel> categoryPanels;

        private void Start()
        {
            AddCategoryPanel();

        }
        
        private void AddCategoryPanel()
        {
            var childes = transform.GetComponentsInChildren<CategoryPanel>();
            foreach (var child in childes)
            {
                child.owner = this;
                categoryPanels.Add(child);
            }
        }
    }
}