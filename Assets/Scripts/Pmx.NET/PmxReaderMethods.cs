using System;
using System.Text;
using System.IO;
using UnityEngine;

namespace PmxSharp
{
    public static class PmxReaderMethods
    {
        #region Geometry
        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }
        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
        public static Vector4 ReadVector4(this BinaryReader reader)
        {
            return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
        public static Color ReadColor3(this BinaryReader reader)
        {
            Vector3 vec = reader.ReadVector3();
            return new Color(vec.x, vec.y, vec.z, 1.0f);
        }
        public static Color ReadColor3(this BinaryReader reader, float alpha)
        {
            Vector3 vec = reader.ReadVector3();
            return new Color(vec.x, vec.y, vec.z, alpha);
        }
        public static Color ReadColor4(this BinaryReader reader)
        {
            Vector4 vec = reader.ReadVector4();
            return new Color(vec.x, vec.y, vec.z, vec.w);
        }
        #endregion
        #region Text
        public static string ReadPmxString(this BinaryReader reader, Encoding encoding)
        {
            if (!(encoding == Encoding.UTF8 || encoding == Encoding.Unicode))
                throw new ArgumentException(string.Format("Encoding must be UTF-8 or Unicode (little endian), got {0}", encoding.EncodingName));

            int length = reader.ReadInt32();
            return encoding.GetString(reader.ReadBytes(length));
        }
        #endregion
        #region Index
        public static int ReadIndex(this BinaryReader reader, PmxIndexType type)
        {
            // Vertex types are byte, ushort, int.
            if(type == PmxIndexType.Vertex)
            {
                switch (PmxIndex.IndexSize(type))
                {
                    case 1: return reader.ReadByte();
                    case 2: return reader.ReadUInt16();
                    case 4: return reader.ReadInt32();
                    default: throw new NotSupportedException("Only integers of up to 4 bytes are supported.");
                }
            }
            // Other types are sbyte, short, int.
            else
            {
                switch (PmxIndex.IndexSize(type))
                {
                    case 1: return reader.ReadSByte();
                    case 2: return reader.ReadInt16();
                    case 4: return reader.ReadInt32();
                    default: throw new NotSupportedException("Only integers of up to 4 bytes are supported.");
                }
            }
        }

        public static int ReadIndex(this BinaryReader reader, int length, bool signed = true)
        {
            switch(length)
            {
                case 1:
                    return signed ? (int)reader.ReadSByte() : (int)reader.ReadByte();
                case 2:
                    return signed ? (int)reader.ReadInt16() : (int)reader.ReadUInt16();
                case 4:
                    return reader.ReadInt32();
            }

            throw new ArgumentException(string.Format("Expected a length of 1, 2, or 4. Got {0}", length));
        }
        #endregion
    }
}
