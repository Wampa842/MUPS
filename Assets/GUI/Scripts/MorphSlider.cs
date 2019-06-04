using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MUPS.UI
{
    public class MorphSlider : MonoBehaviour
    {
        public Slider Slider;
        public Text Label;
        public VertexMorph Morph;
        public SceneModel Owner;

        public void UpdateOwnerMorphs(BaseEventData eventData)
        {
            Morph.Weight = Slider.value;
            Owner.ApplyVertexMorphs(0);
        }
    }
}
