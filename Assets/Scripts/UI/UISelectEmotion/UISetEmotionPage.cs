using System;
using System.Collections;
using System.Collections.Generic;
using UI.UISelectEmotion;
using UnityEngine;
using UnityEngine.UI;

public class UISetEmotionPage : MonoBehaviour
{
    public GameObject sliderObject;
    public UISelectEmotion owner;
    public Transform parentTransform;
    public Button selectButton;

    private List<UISetEmotionValue> _values = new List<UISetEmotionValue>();
    private void Start()
    {
        InitSelectSlider();
    }

    public void InitSelectSlider()
    {
        foreach(var emotion in owner.selectedEmotions)
        {
            GameObject obj = GameObject.Instantiate(sliderObject, parentTransform);
            UISetEmotionValue slider = obj.GetComponent<UISetEmotionValue>();
            slider.Initialize(owner, emotion.Key);
            _values.Add(slider);
        }
        selectButton.onClick.AddListener(SaveButton);
    }
    
    private void SaveButton()
    {
        foreach (var value in _values)
        {
            owner.selectedEmotions[value.emotion] = (int)value.slider.value;
            Debug.Log($"{value.emotion}:{owner.selectedEmotions[value.emotion]}");
        }
        owner.gameObject.SetActive(false);
    }
}
