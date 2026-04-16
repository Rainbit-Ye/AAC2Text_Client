using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public enum WindowResult
    {
        None = 0,
        Yes,
        No
    }

    public delegate void CloseHandler(UIWindow sender, WindowResult windowResult);
    public event CloseHandler OnClose;
    
    public virtual Type Type { get { return GetType(); } }

    private void Close(WindowResult result = WindowResult.None)
    {
        UIManager.Ins.Close(Type);
        if (OnClose != null)
        {
            OnClose(this, result);
        }
        OnClose = null;
    }

    public virtual void OnCloseClick()
    {
        Close(WindowResult.No);
    }

    public virtual void OnYesClick()
    {
        Close(WindowResult.Yes);
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
    }
}
