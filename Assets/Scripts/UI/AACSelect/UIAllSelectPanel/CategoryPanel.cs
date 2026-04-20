using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AACSelect.AllSelectPanel
{
    public class CategoryPanel : MonoBehaviour
    {
        public UIAllSelectPanel.AllSelectPanel owner;
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
        }

        private void ExpandAllImage()
        {
            Debug.Log("ExpandAllImage");
        }
        
        
    }
}