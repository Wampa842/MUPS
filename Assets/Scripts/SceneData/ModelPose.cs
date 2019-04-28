using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUPS.Scene
{
    /// <summary>
    /// Reference and pose data associated with a model.
    /// </summary>
    // Note to self: THIS IS NOT SERIALIZED TO FILE. Use references, for fuck's sake.
    public class ModelPose
    {
        public PmxModelBehaviour Model { get; set; }
        public List<BonePose> Skeleton { get; set; }

        public ModelPose()
        {
            Model = null;
            Skeleton = new List<BonePose>();
        }

        public ModelPose(PmxModelBehaviour model) : this()
        {
            Model = model;
            foreach(PmxBoneBehaviour bone in model.SkeletonRoot.GetComponentsInChildren<PmxBoneBehaviour>())
            {
                Skeleton.Add(new BonePose(bone));
            }
        }

        public void Apply()
        {
            if(Model == null)
            {
                Log.Debug("Model is null");
                return;
            }

            foreach(BonePose pose in Skeleton)
            {
                pose.Apply();
            }

            Log.Trace($"Loaded pose for {Model.DisplayName}");
        }
    }
}
