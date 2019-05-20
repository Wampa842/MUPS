using UnityEngine;

namespace MUPS
{
    public class SceneLight : SceneObject
    {
        public Light Light;

        public static SceneLight Create()
        {
            GameObject root = new GameObject("Light " + Resources.FindObjectsOfTypeAll<SceneLight>().Length);
            SceneLight comp = root.AddComponent<SceneLight>();
            comp.DisplayName = root.name;
            comp.SkeletonRoot = new GameObject("Skeleton").transform;
            comp.SkeletonRoot.SetParent(root.transform);
            comp.MeshRoot = new GameObject("Mesh").transform;
            comp.MeshRoot.SetParent(root.transform);
            GameObject bone = GameObject.Instantiate(SceneController.Instance.BonePrefab);
            bone.transform.SetParent(comp.SkeletonRoot);
            PmxBoneBehaviour bc = bone.GetComponent<PmxBoneBehaviour>();
            bone.name = bc.name = "root";
            bc.Interactive = true;
            bc.Flags = PmxBoneBehaviour.BoneFlags.Rotation | PmxBoneBehaviour.BoneFlags.Translation | PmxBoneBehaviour.BoneFlags.Visible;
            bc.Tail = PmxBoneBehaviour.TailType.Vector;
            bc.TailPosition = new Vector3(0, 0, 1);
            comp.LastSelectedBone = bc;

            comp.Light = bone.AddComponent<Light>();
            LightShadows mode;
            switch (SkyLightController.Instance.LightShadowSelect.value)
            {
                case 0:
                    mode = LightShadows.Soft;
                    break;
                case 1:
                    mode = LightShadows.Hard;
                    break;
                default:
                    mode = LightShadows.None;
                    break;
            }
            comp.Light.shadows = mode;

            root.transform.SetParent(SceneController.Instance.transform);
            return comp;
        }
    }
}
