using Manager;
using TMPro;
using UI.AACSelect.AllSelectPanel;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AACSelect.UIAllSelectPanel
{
    public class CategoryPanel : MonoBehaviour
    {
        public AllSelectPanel owner;
        public ImagePanel imagePanel;
        
        public TMP_Text categoryName;
        public Button expandAllImage;
        public Button changeImage;
        
        public ImageType panelType;
        public void Start()
        {
            LoadingManager.Ins.OnLoadingComplete += Init;
        }

        private void OnApplicationQuit()
        {
            LoadingManager.Ins.OnLoadingComplete -= Init;
        }
        

        public void Init()
        {
            categoryName.text = panelType.ToString();
            expandAllImage.onClick.AddListener(ExpandAllImage);
            changeImage.onClick.AddListener(ChangeImage);
            imagePanel.owner = this;
            imagePanel.SetData();
        }

        private void ChangeImage()
        {
            Debug.Log("ChangeImage");
            imagePanel.SetData();
        }

        private void ExpandAllImage()
        {
            Debug.Log("ExpandAllImage");
        }
        
        
    }
}