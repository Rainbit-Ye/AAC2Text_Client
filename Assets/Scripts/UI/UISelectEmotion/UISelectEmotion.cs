using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UISelectEmotion
{
    public class UISelectEmotion : MonoBehaviour
    {
        public Dictionary<EmotionType,int> selectedEmotions = new Dictionary<EmotionType, int>();
        public UISelectEmotionButton[] buttons;
        
        public Button yesButton;
        public GameObject selectEmotionPage;
        public UISetEmotionPage setValuePage;
        private void Start()
        {
            Init();
            yesButton.onClick.AddListener(ChangePage);
        }

        private void Init()
        {
            foreach (var button in buttons)
            {
                button.owner = this;
            }
            setValuePage.owner = this;
        }

        private void ChangePage()
        {
            selectEmotionPage.SetActive(false);
            setValuePage.gameObject.SetActive(true);
        }
        
        
    }
}
