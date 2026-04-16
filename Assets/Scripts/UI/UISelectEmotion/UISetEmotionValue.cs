using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.UISelectEmotion;
using UnityEngine;
using UnityEngine.UI;

public class UISetEmotionValue : MonoBehaviour
{
    public UISelectEmotion owner;
    public Slider slider;
    public TMP_Text valueText;
    public TMP_Text emotionText;

    public EmotionType emotion;
    public void Initialize(UISelectEmotion owner,EmotionType emotionType)
    {
        this.owner = owner;
        emotion = emotionType;
        this.emotionText.text = emotionType.ToString();
        slider.value = slider.maxValue;
        valueText.text = slider.value.ToString();
    }

    //todo 差一个修改字典内容的button
    public void SetSliderValue()
    {
        valueText.text = slider.value.ToString();
        
    }


}
