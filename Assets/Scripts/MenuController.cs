﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using UnityEngine;

namespace MUPS.UI
{
    class MenuController : MonoBehaviour
    {
        public enum DirectoryType { Config }
        public void OpenDirectory(int type)
        {
            switch (type)
            {
                case 1:
                    Process.Start('"' + Application.persistentDataPath + '"');
                    break;
                case 2:
                    Process.Start('"' + Path.GetDirectoryName(Scene.SceneController.Instance.SelectedModel.FilePath) + '"');
                    break;
                default:
                    break;
            }
        }
    }
}
