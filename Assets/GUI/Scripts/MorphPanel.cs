using UnityEngine;

namespace MUPS.UI
{
    class MorphPanel : MonoBehaviour
    {
        public GameObject Panel;
        public void TogglePanel()
        {
            Panel.SetActive(!Panel.activeSelf);
        }
    }
}
