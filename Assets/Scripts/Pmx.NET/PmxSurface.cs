using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PmxSharp
{
    public class PmxSurface
    {
        private int[] _vertices;
        public int this[int index]
        {
            get
            {
                return _vertices[index];
            }
            set
            {
                _vertices[index] = value;
            }
        }
        public int Vertex0
        {
            get
            {
                return _vertices[0];
            }
            set
            {
                _vertices[0] = value;
            }
        }
        public int Vertex1
        {
            get
            {
                return _vertices[1];
            }
            set
            {
                _vertices[1] = value;
            }
        }
        public int Vertex2
        {
            get
            {
                return _vertices[2];
            }
            set
            {
                _vertices[2] = value;
            }
        }

        public PmxSurface()
        {
            _vertices = new int[3];
        }

        public PmxSurface(int vertex0, int vertex1, int vertex2, bool flipped = false)
        {
            if (flipped)
                _vertices = new int[3] { vertex2, vertex1, vertex0 };
            else
                _vertices = new int[3] { vertex0, vertex1, vertex2 };
        }

        public static int[] GetIndices(IEnumerable<PmxSurface> collection)
        {
            List<int> list = new List<int>();
            foreach(PmxSurface s in collection)
            {
                list.Add(s.Vertex0);
                list.Add(s.Vertex1);
                list.Add(s.Vertex2);
            }
            return list.ToArray();
        }
    }
}
