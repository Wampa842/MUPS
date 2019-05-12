using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace PmxSharp
{
    #region Directives
    public abstract class MaterialDirective
    {
        public string RawString { get; private set; }
        public virtual void Execute(Material subject) { }
        public virtual void ExecuteEarly(Material subject) { }
        public virtual void ExecuteLate(Material subject) { }
        public virtual void Execute(MeshRenderer subject) { }
        public virtual void ExecuteEarly(MeshRenderer subject) { }
        public virtual void ExecuteLate(MeshRenderer subject) { }
        public virtual void Execute(SkinnedMeshRenderer subject) { }
        public virtual void ExecuteEarly(SkinnedMeshRenderer subject) { }
        public virtual void ExecuteLate(SkinnedMeshRenderer subject) { }

        protected MaterialDirective(string raw)
        {
            RawString = raw;
        }

        public static bool HasStartTag(string s)
        {
            return s.ToLowerInvariant().Contains("[start]");
        }

        public static bool Validate(string s)
        {
            string[] lines = s.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            bool startUnpaired = false;
            foreach (string line in lines)
            {
                if (line.Trim().ToLowerInvariant() == "[start]")
                    startUnpaired = true;

                if (startUnpaired && line.Trim().ToLowerInvariant() == "[end]")
                    startUnpaired = false;
            }

            return !startUnpaired;
        }

        public static string TrimString(string s)
        {
            return s.Trim().TrimStart('[').TrimEnd(']');
        }
    }

    public class SetDirective : MaterialDirective
    {
        private enum PropertyType { Float, Int, Color, Texture, Offset, Keyword, Unsupported }
        private PropertyType ParsePropertyType(string s)
        {
            switch (s.ToLowerInvariant())
            {
                case "float":
                    return PropertyType.Float;
                case "int":
                case "integer":
                    return PropertyType.Int;
                case "color":
                case "colour":
                    return PropertyType.Color;
                case "texture":
                    return PropertyType.Texture;
                case "string":
                    return PropertyType.Offset;
                case "keyword":
                    return PropertyType.Keyword;
                default:
                    return PropertyType.Unsupported;
            }
        }

        private PropertyType _type;
        private object _value;
        private string _property;

        public override void Execute(Material subject)
        {
            switch (_type)
            {
                case PropertyType.Float:
                    subject.SetFloat(_property, (float)_value);
                    break;
                case PropertyType.Int:
                    subject.SetInt(_property, (int)_value);
                    break;
                case PropertyType.Color:
                    subject.SetColor(_property, (Color)_value);
                    break;
                case PropertyType.Texture:
                    subject.SetTexture(_property, (Texture)_value);
                    break;
                case PropertyType.Offset:
                    subject.SetTextureOffset(_property, (Vector2)_value);
                    break;
                default:
                    break;
            }
        }

        public SetDirective(string raw) : base(raw)
        {
            string[] parsed = raw.Split(new char[] { ' ' }, 4);
            _type = ParsePropertyType(parsed[1]);
            _property = parsed[2];

            switch (_type)
            {
                case PropertyType.Float:
                    if (float.TryParse(parsed[3], out float f))
                    {
                        _value = f;
                    }
                    break;
                case PropertyType.Int:
                    if (int.TryParse(parsed[3], out int i))
                    {
                        _value = i;
                    }
                    break;
                case PropertyType.Color:
                    string[] cs = parsed[3].Split(' ');
                    float r, g, b, a;
                    if (cs.Length == 3)
                    {
                        r = float.Parse(cs[0]);
                        g = float.Parse(cs[1]);
                        b = float.Parse(cs[2]);
                        a = 1;
                    }
                    else if (cs.Length == 4)
                    {
                        r = float.Parse(cs[0]);
                        g = float.Parse(cs[1]);
                        b = float.Parse(cs[2]);
                        a = float.Parse(cs[3]);
                    }
                    else
                    {
                        throw new MaterialDirectiveException("Cannot parse color string (format must be RGB or RGBA separated by spaces).", raw);
                    }
                    _value = new Color(r, g, b, a);
                    break;
                case PropertyType.Texture:
                    throw new NotImplementedException("set texture is not yet implemented.");
                case PropertyType.Offset:
                    throw new NotImplementedException("set string is not yet implemented.");
                case PropertyType.Keyword:
                    throw new NotImplementedException("set keyword is not yet implemented.");
                default:
                    break;
            }
        }
    }

    public class CopyDirective : MaterialDirective
    {
        private static readonly HashSet<string> _keywords = new HashSet<string> { "diffuse", "ambient", "emissive", "specular", "smoothness", "exponent", "roughness", "edge", "edgesize", "diffusepath", "diffusetex", "spherepath", "spheretex", "sphereblend", "toonindex", "toonpath", "toontex" };
        PmxMaterial _mat;
        private string _keyword;
        private string _dst;

        public CopyDirective(string raw, PmxMaterial material) : base(raw)
        {
            _mat = material;
            string[] parsed = raw.Split(' ');
            _keyword = parsed[1];
            if (_keywords.Contains(_keyword))
            {
                throw new MaterialDirectiveException(string.Format("{0} is not a valid keyword.", _keyword), raw);
            }
            _dst = parsed[2];
        }

        public override void Execute(Material subject)
        {
            switch (_keyword)
            {
                case "diffuse":
                    subject.SetColor(_dst, _mat.Diffuse);
                    break;
                case "ambient":
                case "emissive":
                    subject.SetColor(_dst, _mat.Ambient);
                    break;
                case "specular":
                    subject.SetColor(_dst, _mat.Specular);
                    break;
                case "smoothness":
                case "exponent":
                    subject.SetFloat(_dst, _mat.SpecularExponent);
                    break;
                case "roughness":
                    subject.SetFloat(_dst, 1 - _mat.SpecularExponent);
                    break;
                case "edge":
                    subject.SetColor(_dst, _mat.EdgeColor);
                    break;
                case "edgesize":
                    subject.SetFloat(_dst, _mat.EdgeSize);
                    break;
                case "diffusetex":
                    break;
                case "spherepath":
                    break;
                case "spheretex":
                    break;
                case "sphereblend":
                    break;
                case "toonindex":
                    subject.SetInt(_dst, _mat.ToonReference);
                    break;
                case "toonpath":
                    break;
                case "toontex":
                    break;
            }
        }
    }

    public class KeywordDirective : MaterialDirective
    {
        private string _keyword;
        private bool _active;

        public KeywordDirective(string raw) : base(raw)
        {
            string[] parsed = raw.Split(' ');
            _active = parsed[1].ToLowerInvariant() == "enable" || parsed[1].ToLowerInvariant() == "on";
            _keyword = parsed[2];
        }

        public override void Execute(Material subject)
        {
            if (_active)
                subject.EnableKeyword(_keyword);
            else
                subject.DisableKeyword(_keyword);
        }
    }

    public class QueueDirective : MaterialDirective
    {
        int _queue;
        public QueueDirective(string raw) : base(raw)
        {
            if (!int.TryParse(raw.Split(' ')[1], out _queue))
            {
                throw new FormatException(string.Format("Failed to parse number in {0}", raw));
            }
        }

        public override void Execute(Material subject)
        {
            subject.renderQueue = _queue;
        }
    }

    public class ShadowDirective : MaterialDirective
    {
        public enum ShadowOperation { Cast, Receive }
        private ShadowOperation _op;
        private ShadowCastingMode _mode;

        public ShadowDirective(string raw, PmxMaterial material) : base(raw)
        {
            string[] parsed = raw.Split(' ');

            switch (parsed[1].ToLowerInvariant())
            {
                case "cast":
                    _op = ShadowOperation.Cast;
                    switch (parsed[2].ToLowerInvariant())
                    {
                        case "off":
                            _mode = ShadowCastingMode.Off;
                            break;
                        case "on":
                            _mode = ShadowCastingMode.On;
                            break;
                        case "double":
                            _mode = ShadowCastingMode.TwoSided;
                            break;
                        case "shadowonly":
                            _mode = ShadowCastingMode.ShadowsOnly;
                            break;
                        case "auto":
                            if (material.HasFlag(PmxMaterialFlags.CastShadow))
                            {
                                if (material.HasFlag(PmxMaterialFlags.TwoSided))
                                {
                                    _mode = ShadowCastingMode.TwoSided;
                                }
                                else
                                {
                                    _mode = ShadowCastingMode.On;
                                }
                            }
                            else
                            {
                                _mode = ShadowCastingMode.Off;
                            }
                            break;
                        case "doubleauto":
                            _mode = material.HasFlag(PmxMaterialFlags.CastShadow) ? ShadowCastingMode.TwoSided : ShadowCastingMode.Off;
                            break;
                        case "singleauto":
                            _mode = material.HasFlag(PmxMaterialFlags.CastShadow) ? ShadowCastingMode.On : ShadowCastingMode.Off;
                            break;
                    }
                    break;
                case "receive":
                    _mode = parsed[2].ToLowerInvariant() == "on" ? ShadowCastingMode.On : ShadowCastingMode.Off;
                    break;
            }
        }

        public override void ExecuteLate(MeshRenderer subject)
        {
            if (_op == ShadowOperation.Cast)
            {
                subject.shadowCastingMode = _mode;
            }
            else
            {
                subject.receiveShadows = _mode == ShadowCastingMode.On;
            }
        }
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

        /// <summary>
        /// Returns true if any of the specified flag bits are set.
        /// </summary>
        public bool HasFlag(PmxMaterialFlags flag)
        {
            return (Flags & flag) != 0;
        }
        #endregion
        #region Surfaces

        public int VertexCount { get; set; }
        public int FirstVertex { get; set; }
        public int LastVertex { get { return FirstVertex + VertexCount - 1; } }
        public int TriangleCount { get { return VertexCount / 3; } }
        public int FirstTriangle { get { return FirstVertex / 3; } }
        public int LastTriangle { get { return FirstTriangle + TriangleCount - 1; } }
        public PmxSurface[] GetSurfaces(PmxSurface[] coll)
        {
            return coll.Skip(FirstTriangle).Take(TriangleCount).ToArray();
        }

        #endregion
        #region Directives

        public List<MaterialDirective> Directives { get; private set; }

        public void ReadDirectives(string text)
        {
            Directives = new List<MaterialDirective>();

            foreach (string untrimmed in Note.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string line = MaterialDirective.TrimString(untrimmed);
                string name = line.Split(' ')[0].ToLowerInvariant();
                switch (name)
                {
                    case "set":
                        Directives.Add(new SetDirective(line));
                        break;
                    case "copy":
                        Directives.Add(new CopyDirective(line, this));
                        break;
                    case "keyword":
                        Directives.Add(new KeywordDirective(line));
                        break;
                    case "queue":
                        Directives.Add(new QueueDirective(line));
                        break;
                    case "shadow":
                        Directives.Add(new ShadowDirective(line, this));
                        break;
                }
            }
        }

        public void ReadDirectives()
        {
            if (MaterialDirective.HasStartTag(Note))
            {
                if (MaterialDirective.Validate(Note))
                {
                    throw new FormatException(string.Format("The note of material {0} contains an unmatched [start] tag.", Name));
                }
                else
                {
                    ReadDirectives(Note);
                }
            }
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Material {0}\n", Name)
                .AppendFormat("Color {0}\n", Diffuse)
                .AppendFormat("Vertices {0}\n", VertexCount);
            return sb.ToString();
        }
    }
}
