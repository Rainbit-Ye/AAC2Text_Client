using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ColorSelect
{
    Red = 0,
    Green = 1,
    Blue = 2
}
public class ChangeColor : MonoBehaviour
{
    public ColorSelect color;
    private Material _material;

    public Material Material
    {
        get
        {
            if (_material == null)
            {
                _material = transform.GetComponent<MeshRenderer>().sharedMaterial;
            }
            return _material;
        }
        set
        {
            _material = value;
        }
    }

    
    public void ChangeMaterialColor(ColorSelect color)
    {
        if (color == ColorSelect.Red)
        {
            Material.color = Color.red;
        }
        else if (color == ColorSelect.Green)
        {
            Material.color = Color.green;
        }
        else if(color == ColorSelect.Blue)
        {
            Material.color = Color.blue;
        }
        else
        {
            Material.color = Color.white;
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("OnPointerClick");
        ChangeMaterialColor(color);
    }
}

