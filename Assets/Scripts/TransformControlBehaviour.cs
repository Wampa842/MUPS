using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MUPS.UI
{
    class TransformControlBehaviour : MonoBehaviour, IDragHandler
    {
        public void Transform(int type)
        {
            Debug.Log("t");
        }
    }
}
