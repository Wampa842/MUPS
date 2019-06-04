using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PmxSharp;

namespace MUPS
{
    public class VertexMorph
    {
        public enum MorphPanel { None, Eye, Eyebrow, Mouth, Other }

        public string Name { get; set; }
        public PmxMorphPanel Panel { get; set; }
        public float Weight { get; set; }
        public Vector3[] Offsets { get; set; }

        public VertexMorph(PmxMorph morph, Mesh baseMesh, float scale = 1.0f)
        {
            Weight = 0;
            Name = morph.Name;
            Panel = morph.Panel;
            Offsets = new Vector3[baseMesh.vertexCount];
            Log.Info(Name, Panel.ToString(), Offsets.Length, morph.Offsets.Length);
            for (int i = 0; i < morph.Offsets.Length; ++i)
            {
                PmxVertexOffset offset = (PmxVertexOffset)morph.Offsets[i];
                Offsets[offset.Index] = offset.Translation * scale;
            }
        }
    }
}
