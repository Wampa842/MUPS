using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MUPS
{
    /// <summary>
    /// Represents a model loaded from a PMX file.
    /// </summary>
    public class SceneModel : SceneObject
    {
        public enum Panel { None = 0, Eyebrow = 1, Eye = 2, Mouth = 3, Other = 4 }

        public string DescriptionEnglish { get; set; }
        public string DescriptionJapanese { get; set; }

        public VertexMorph[] VertexMorphs { get; set; }
        private Mesh _mesh;
        private Vector3[] _baseMesh;

        // Morph sliders that belong to this model.
        public UI.MorphSlider[] MorphSliders { get; set; }

        // Gets the sum of the model's vertex morph offsets
        private Vector3[] GetVertexMorphDelta(out int first, out int last)
        {
            Vector3[] sum = new Vector3[VertexMorphs[0].Offsets.Length];
            first = last = -1;
            bool foundFirst = false;
            foreach (VertexMorph morph in VertexMorphs)
            {
                for (int i = 0; i < morph.Offsets.Length; ++i)
                {
                    Vector3 offset = morph.Offsets[i];

                    if (!foundFirst && offset != Vector3.zero)
                    {
                        foundFirst = true;
                        first = i;
                    }
                    if (morph.Weight != 0)
                    {
                        sum[i] += morph.Offsets[i] * morph.Weight;
                    }
                }
            }

            last = Array.FindLastIndex(sum, a => a != Vector3.zero);

            return sum;
        }

        // Applies the vertex morphs to the mesh
        public void ApplyVertexMorphs(float val)
        {
            // 56 milliseconds... Unity really wasn't designed for vertex morphs.
            var sw = System.Diagnostics.Stopwatch.StartNew();

            Vector3[] vertices = (Vector3[])_baseMesh.Clone();
            Vector3[] delta = GetVertexMorphDelta(out int first, out int last);

            for (int i = 0; i <= last; ++i)
            {
                if (delta[i] != Vector3.zero)
                {
                    vertices[i] += delta[i];
                }
            }

            _mesh.vertices = vertices;

            Log.Trace("Vertices copied", sw.ElapsedMilliseconds);
            sw.Stop();
        }

        public void HideMorphList()
        {
            if (MorphSliders == null || MorphSliders.Length <= 0)
                return;

            foreach (UI.MorphSlider slider in MorphSliders)
            {
                slider.gameObject.SetActive(false);
            }
        }

        public static void HideAllMorphLists()
        {
            foreach (SceneModel model in SceneController.Instance.SceneModels.OfType<SceneModel>())
            {
                model.HideMorphList();
            }
        }

        public void ShowMorphList()
        {
            SceneModel.HideAllMorphLists();
            foreach (UI.MorphSlider slider in MorphSliders)
            {
                slider.gameObject.SetActive(true);
            }
        }

        protected override void Start()
        {
            base.Start();

            OnSelected.AddListener(ShowMorphList);
            _mesh = GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            _baseMesh = (Vector3[])_mesh.vertices.Clone();
        }

        protected override void Awake()
        {
            base.Awake();

            
        }

        public static new SceneModel Create(string name, bool withBone = false)
        {
            GameObject root = new GameObject("Model " + Resources.FindObjectsOfTypeAll<SceneModel>().Length);
            SceneModel comp = root.AddComponent<SceneModel>();
            comp.DisplayName = root.name;
            comp.SkeletonRoot = new GameObject("Skeleton").transform;
            comp.SkeletonRoot.SetParent(root.transform);
            comp.MeshRoot = new GameObject("Mesh").transform;
            comp.MeshRoot.SetParent(root.transform);
            if (withBone)
            {
                GameObject bone = GameObject.Instantiate(SceneController.Instance.BonePrefab);
                bone.transform.SetParent(comp.SkeletonRoot);
                PmxBoneBehaviour bc = bone.GetComponent<PmxBoneBehaviour>();
                bone.name = bc.name = "root";
                bc.Interactive = true;
                bc.Flags = PmxBoneBehaviour.BoneFlags.Rotation | PmxBoneBehaviour.BoneFlags.Translation | PmxBoneBehaviour.BoneFlags.Visible;
                bc.Tail = PmxBoneBehaviour.TailType.Vector;
                bc.TailPosition = new Vector3(0, 0, 1);
                comp.LastSelectedBone = bc;
            }

            root.transform.SetParent(SceneController.Instance.transform);
            return comp;
        }
    }
}
