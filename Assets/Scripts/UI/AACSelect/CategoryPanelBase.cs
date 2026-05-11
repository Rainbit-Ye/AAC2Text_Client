using TMPro;
using UI.AACSelect.AllSelectPanel;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AACSelect
{
    public class CategoryPanelBase : MonoBehaviour
    {
        public ImagePanel imagePanel;
        public Button changeImage;
        
        public ImageType panelType;
        public TMP_Text categoryName;
    }
}