using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MUPS;

namespace MUPS.UI
{
    public class BoneButton : MonoBehaviour
    {
        public SceneObject Owner;
        public PmxBoneBehaviour Bone;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => SceneController.Instance.SelectBone(Bone));
        }
    }
}
