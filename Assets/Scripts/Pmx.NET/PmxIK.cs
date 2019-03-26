using System;
using System.Collections.Generic;
using UnityEngine;

namespace PmxSharp
{
    public struct PmxIKLink
    {
        public int Bone { get; set; }
        public bool Limited { get; set; }
        public Vector3 LimitMin { get; set; }
        public Vector3 LimitMax { get; set; }
    }

    public struct PmxIK
    {
        public int Target { get; set; }
        public int Loop { get; set; }
        public float Limit { get; set; }
        public PmxIKLink[] Links { get; set; }
    }
}
