using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    System.Diagnostics.Process.Start($"\"{Application.persistentDataPath}\"");
                    break;
                default:
                    break;
            }
        }
    }
}
