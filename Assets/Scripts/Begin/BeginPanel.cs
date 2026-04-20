using System;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Begin
{
    public class BeginPanel : MonoBehaviour
    {
        public Button startButton;

        private void Start()
        {
            startButton.onClick.AddListener(() =>
            {
                EventManager.Ins.OpenSelectUI();
                this.gameObject.SetActive(false);
            });
        }
    }
}