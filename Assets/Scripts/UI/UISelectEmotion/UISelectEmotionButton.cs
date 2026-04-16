using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.UISelectEmotion
{
    public enum EmotionType
    {
        None = 0,
        Happy,
        Anxious,
        Angry,
        Afraid,
        Sad,
        Relaxed,
        Bored,
        Calm,
        Content,
        Excited
    }
    public class UISelectEmotionButton : MonoBehaviour
    {
        public UISelectEmotion owner;
        public Button button;
        public TMP_Text text;

        
        public EmotionType emotion;
        public Image emotionButtonImg;
        public Sprite selectedSprite;
        public Sprite unselectedSprite;
        private bool _selected = false;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                emotionButtonImg.sprite = _selected ? selectedSprite : unselectedSprite;
            }
        }
        private void Start()
        {
            button.onClick.AddListener(SelectEmotion);
            text.text = emotion.ToString();
        }

        private void SelectEmotion()
        {
            if (!Selected)
            {
                owner.selectedEmotions[emotion] = 0;
                Selected = true;
                
            }
            else
            {
                owner.selectedEmotions.Remove(emotion);
                Selected = false;
            }
        }


    }
}
