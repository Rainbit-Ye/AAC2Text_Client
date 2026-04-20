using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AACSelect
{
    public class AACIconSlot : MonoBehaviour
    {
        public AACSelectPanel owner;
        public Button sendButton;
        
        public GameObject imagePrefabs;
        public Transform imageParent;
        public List<AACImagePrefabs> pool;

        private List<string> _iconLabels = new List<string>();
        private void Start()
        {
            sendButton.onClick.AddListener(SendAAC);
        }

        private void SendAAC()
        {
            Debug.Log("SendAAC");
            _ = GrpcClientManager.Ins.SendDispersesIcon(_iconLabels);
            var images = transform.GetComponentsInChildren<AACImagePrefabs>(true);
            foreach (var imagePrefab in images)
            {
                PushImagePrefabsToPool(imagePrefab);
            }
        }

        public void SetData(string iconLabel)
        {
            GetImagePrefabsForPool(out var imagePrefab);
            _iconLabels.Add(iconLabel);
            imagePrefab.SetData(iconLabel);
        }

        private void GetImagePrefabsForPool(out AACImagePrefabs imagePrefab)
        {
            if (pool.Count > 0)
            {
                imagePrefab = pool[0];
                pool.RemoveAt(0);
            }
            else
            {
                GameObject obj = Instantiate(imagePrefabs);
                imagePrefab = obj.GetComponent<AACImagePrefabs>();
                imagePrefab.transform.SetParent(imageParent);
            }
            imagePrefab.gameObject.SetActive(true);
        }
        
        private void PushImagePrefabsToPool(AACImagePrefabs imagePrefab)
        {
            pool.Add(imagePrefab);
            imagePrefab.gameObject.SetActive(false);
        }
    }
}