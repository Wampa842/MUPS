using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PmxSharp
{
    #region Directives
    public abstract class MaterialDirective
    {
        public string Name { get; }
        public string Help { get; }

        public virtual void Execute(Material subject) { }
        public virtual void ExecuteEarly(Material subject) { }
        public virtual void ExecuteLate(Material subject) { }
        public virtual void Execute(MeshRenderer subject) { }
        public virtual void ExecuteEarly(MeshRenderer subject) { }
        public virtual void ExecuteLate(MeshRenderer subject) { }
    }
    #endregion

    [Flags]
    public enum PmxMaterialFlags { TwoSided = 1, GroundShadow = 2, CastShadow = 4, ReceiveShadow = 8, Edge = 16, VertexColor = 32, DisplayPoints = 64, DisplayEdges = 128 }
    public enum PmxEnvironmentTextureType { Disabled = 0, Multiplicative = 1, Additive = 2, SubTexture = 3 }
    public enum PmxToonType { Texture, Index }

    public class PmxMaterial : PmxItem
    {
        #region Standard properties
        public PmxMaterialFlags Flags { get; set; }
        public Color Diffuse { get; set; }
        public Color Specular { get; set; }
        public float SpecularExponent { get; set; }
        public Color Ambient { get; set; }
        public Color EdgeColor { get; set; }
        public float EdgeSize { get; set; }
        public int TextureIndex { get; set; }
        public int EnvironmentIndex { get; set; }
        public PmxEnvironmentTextureType EnvironmentType { get; set; }
        public PmxToonType ToonType { get; set; }
        public int ToonReference { get; set; }
        public string Note { get; set; }
        public int SurfaceCount { get; set; }
        #endregion
        #region Surfaces

        #endregion
        #region Directives

        public List<MaterialDirective> Directives { get; private set; }

        public void ReadDirectives(string text)
        {
            Directives = new List<MaterialDirective>();
        }

        public void ReadDirectives()
        {
            ReadDirectives(Note);
        }

        #endregion
    }
}
