using System;
using System.Collections;
using System.Collections.Generic;
using ImageCache;
using UI.AACSelect.AllSelectPanel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// AAC图片预设，将选择好的图片存到字典中
public class AACImagePrefabs : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ImagePanel owner;
    public Image aacImage;
    public Image selectImage;
    public Button button;
    
    public GameObject bubble;

    public bool canSelected = true;
    
    private string _imageName;
    public string ImageName => _imageName;

    private void Start()
    {
        button.onClick.AddListener(AddAacImageToDictionary);
    }

    private void AddAacImageToDictionary()
    {
        if (canSelected)
        {
            owner.owner.owner.owner.iconSlot.SetData(_imageName);
        }
    }

    public void SetData(string imageName,bool selected = true)
    {
        canSelected = selected;
        _imageName = imageName;
        Texture2D tex = ImageCacheFunctionLibrary.GetImage(imageName);
        Sprite sprite = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(0.5f, 0.5f));
        aacImage.sprite = sprite;
    }

public void OnPointerEnter(PointerEventData eventData)
    {
        if (canSelected)
        {
            selectImage.gameObject.SetActive(true);
            //bubble.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canSelected)
        {
            selectImage.gameObject.SetActive(false);
            //bubble.gameObject.SetActive(false);
        }
    }
}
