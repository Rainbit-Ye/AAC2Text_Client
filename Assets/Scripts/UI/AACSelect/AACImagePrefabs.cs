using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// AAC图片预设，将选择好的图片存到字典中
public class AACImagePrefabs : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image aacImage;
    
    public Button button;
    public Image selectImage;
    public TMP_Text picDesc;
    public GameObject bubble;

    public bool isSelected = true;

    private void Start()
    {
        button.onClick.AddListener(AddAACImageToDictionary);
    }

    private void AddAACImageToDictionary()
    {
        if (isSelected)
        {
        }
    }

public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected)
        {
            selectImage.gameObject.SetActive(true);
            bubble.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected)
        {
            selectImage.gameObject.SetActive(false);
            bubble.gameObject.SetActive(false);
        }
    }
}
