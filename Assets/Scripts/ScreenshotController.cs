using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MUPS.SaveData;
using UnityEngine;
using UnityEngine.UI;
using Crosstales.FB;

namespace MUPS
{
    class ScreenshotController : MonoBehaviour
    {
        public Camera RenderCamera = null;
        public InputField WidthField = null;
        public InputField HeightField = null;

        private bool _screenshot = false;
        private string _path;

        public void QueueScreenshot()
        {
            _path = FileBrowser.SaveFile("Render image", Path.GetDirectoryName(Settings.Current.ApplicationSettings.LastRenderPath), "", new ExtensionFilter("Supported images", "png", "jpg", "jpeg"));
            if (!string.IsNullOrEmpty(_path))
            {
                Settings.Current.ApplicationSettings.RenderWidth = int.Parse(WidthField.text);
                Settings.Current.ApplicationSettings.RenderHeight = int.Parse(HeightField.text);
                _screenshot = true;
            }
        }

        public void LateUpdate()
        {
            if (_screenshot)
            {
                RenderCamera.enabled = true;

                RenderTexture target = new RenderTexture(new RenderTextureDescriptor(Settings.Current.ApplicationSettings.RenderWidth, Settings.Current.ApplicationSettings.RenderHeight));
                RenderCamera.targetTexture = target;
                Texture2D image = new Texture2D(target.width, target.height);
                RenderCamera.Render();
                RenderTexture.active = target;
                image.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
                RenderCamera.targetTexture = null;
                RenderTexture.active = null;

                string ext = Path.GetExtension(_path).ToLower();
                switch (ext)
                {
                    case ".png":
                        File.WriteAllBytes(_path, image.EncodeToPNG());
                        break;
                    case ".jpg":
                    case ".jpeg":
                        File.WriteAllBytes(_path, image.EncodeToJPG());
                        break;
                }

                Destroy(target);
                Destroy(image);

                RenderCamera.enabled = false;
                _screenshot = false;
            }
        }

        public void Start()
        {
            WidthField.text = Settings.Current.ApplicationSettings.RenderWidth.ToString();
            HeightField.text = Settings.Current.ApplicationSettings.RenderHeight.ToString();
        }
    }
}
