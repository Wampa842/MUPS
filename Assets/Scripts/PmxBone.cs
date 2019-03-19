using System;
using UnityEngine;
using MUPS.SaveData;

namespace MUPS
{
    class PmxBone : MonoBehaviour
    {
        public enum TailType { None, Bone, Vector }
        public enum BoneType { Disabled, Rotate, Move, Twist, Invisible }

        public static float Radius = 0.1f;
        public static Color NormalColor = new Color(0, 0, 1);
        public static Color SelectedColor = new Color(1, 0, 0);
        public static Color ModifiedColor = new Color(0, 1, 0);

        public string Name = "Bone";
        public BoneType Type = BoneType.Rotate;

        public SphereCollider Collider = null;
        public Transform SpriteHolder = null;
        public Transform TailHolder = null;
        public TailType Tail = TailType.None;

        public Transform TailBone = null;
        public Vector3 TailPosition = new Vector3();

        public void SetColors()
        {
            foreach (PmxBone bone in Resources.FindObjectsOfTypeAll<PmxBone>())
            {
                bone.Color = PmxBone.NormalColor;
            }
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
                TailHolder.GetComponent<MeshRenderer>().material.color = new Color(value.r, value.g, value.b, 0.5f);
            }
        }

        void Awake()
        {

        }

        void Start()
        {
            TailHolder.GetComponent<Renderer>().enabled = Tail != TailType.None;
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
