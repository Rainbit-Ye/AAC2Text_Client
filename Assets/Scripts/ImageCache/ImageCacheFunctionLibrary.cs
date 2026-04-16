using System.Collections.Generic;
using UnityEngine;

namespace ImageCache
{
    public class ImageCacheFunctionLibrary
    {
        private static Dictionary<string,Texture2D> _imageDict = new Dictionary<string, Texture2D>();
        
        public static void SaveImage(string key, Texture2D tex)
        {
            _imageDict[key] = tex;
        }
        
        public static Texture2D GetImage(string key)
        {
            _imageDict.TryGetValue(key, out var tex);
            return tex;
        }
        
        public static bool HasImage(string key) => _imageDict.ContainsKey(key);
    }
}