using System;
using UnityEngine;
using MUPS.SaveData;

namespace MUPS
{
    class PmxBoneBehaviour : MonoBehaviour
    {
        public enum TailType { None, Bone, Vector }
        [Flags]
        public enum BoneFlags { Rotation, Translation, Visible }

        public static float Radius = 0.1f;
        public static Color NormalColor = new Color(0, 0, 1);
        public static Color SelectedColor = new Color(1, 0, 0);
        public static Color ModifiedColor = new Color(0, 1, 0);

        public string Name = "Bone";
        public BoneFlags Type = BoneFlags.Rotation | BoneFlags.Visible;
        public bool Modified = false;

        public SphereCollider Collider = null;
        public Transform SpriteHolder = null;
        public Transform TailHolder = null;
        public TailType Tail = TailType.None;

        public Transform TailBone = null;
        public Vector3 TailPosition = new Vector3();

        public static void ResetColors()
        {
            foreach (PmxBoneBehaviour bone in Resources.FindObjectsOfTypeAll<PmxBoneBehaviour>())
            {
                bone.Color = bone.Modified ? ModifiedColor : NormalColor;
            }
        }

        public void SetColors()
        {
            ResetColors();
            Color = SelectedColor;
        }

        public Color Color
        {
            get
            {
                return SpriteHolder.GetComponent<SpriteRenderer>().color;
            }
            set
            {
                SpriteHolder.GetComponent<SpriteRenderer>().color = value;
                TailHolder.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(value.r, value.g, value.b, 0.5f);
            }
        }

        #region Interactivity
        private bool _interactive = true;
        public bool Interactive
        {
            get
            {
                return _interactive;
            }
            set
            {
                _interactive = value;
                Collider.enabled = value;
                SpriteHolder.GetComponent<Renderer>().enabled = value;
                TailHolder.GetComponent<Renderer>().enabled = value;
            }
        }
        #endregion

        void Awake()
        {
            Renderer r = TailHolder.GetComponent<Renderer>();
            r.material = new Material(r.material);
        }

        void Start()
        {
            TailHolder.GetComponent<Renderer>().enabled = Tail != TailType.None;
            ResetColors();
        }

        void Update()
        {
            float dist = Camera.main.transform.InverseTransformPoint(transform.position).z;
            float r = SaveData.Settings.Current.View.BoneSize * dist;
            Collider.radius = r / 2.0f;
            SpriteHolder.LookAt(Camera.main.transform.position);
            SpriteHolder.localScale = new Vector3(r, r, r);

            switch (Tail)
            {
                case TailType.Bone:
                    TailHolder.LookAt(TailBone);
                    TailHolder.localScale = new Vector3(Settings.Current.View.BoneTailSize * dist, Settings.Current.View.BoneTailSize * dist, Vector3.Distance(transform.position, TailBone.position));
                    break;
                case TailType.Vector:
                    TailHolder.LookAt(transform.TransformPoint(TailPosition));
                    TailHolder.localScale = new Vector3(Settings.Current.View.BoneTailSize * dist, Settings.Current.View.BoneTailSize * dist, TailPosition.magnitude);
                    break;
                default:
                    break;
            }
        }
    }
}
