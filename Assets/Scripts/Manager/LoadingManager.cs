using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageCache;
using UnityEngine;

namespace Manager
{
    public class LoadingManager : Singleton<LoadingManager>
    {
        public void StartLoad()
        {
            DataManager.InitDataTable();
            DataManager.GetImageNameList(out var imageNameList);
            _ = PreLoadImageFromServerBatch(imageNameList.Keys.ToList(), 200);
        }

        private async Task PreLoadImageFromServerBatch(List<string> imageNameList, int batchSize)
        {
            for (int i = 0; i < imageNameList.Count; i += batchSize)
            {
                int count = Mathf.Min(batchSize, imageNameList.Count - i);
                var batch = imageNameList.GetRange(i, count);
                await PreLoadImageFromServer(batch);
            }
        }

        private async Task PreLoadImageFromServer(List<string> imageName)
        {
            try
            {
                Debug.Log($"加载图片：{imageName.Count}");
                LoadIconReq request = new LoadIconReq();
                request.Csid = CSID.LoadIconReq;
                request.IconLabel.AddRange(imageName);
                var response = await GrpcClientManager.Ins.Client.LoadIconAsync(request);
                OnLoadIconResponse(response);

            }
            catch (System.Exception e)
            {
                Debug.LogError($"加载失败：{e}");
            }
        }


        private void OnLoadIconResponse(LoadIconRes res)
        {
            for (int i = 0; i < res.ImageData.Count; i++)
            {
                byte[] imageBytes = res.ImageData[i].ToByteArray();
                    
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imageBytes);
                ImageCacheFunctionLibrary.SaveImage(res.IconLabel[i], tex);
            }
        }

    }
}