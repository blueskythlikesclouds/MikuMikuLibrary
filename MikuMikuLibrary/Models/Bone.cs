using System;
using System.Numerics;

namespace MikuMikuLibrary.Models
{
    public class Bone : IEquatable<Bone>
    {
        public string Name { get; set; }
        public int ParentID { get; set; }
        public int ID { get; set; }
        public Matrix4x4 Matrix { get; set; }

        public Bone()
        {
            ParentID = -1;
        }

        public bool Equals( Bone other )
        {
            return Name == other.Name && ParentID == other.ParentID && ID == other.ID && Matrix == other.Matrix;
        }
    }

    public struct BoneWeight
    {
        public float Weight1, Weight2, Weight3, Weight4;
        public int Index1, Index2, Index3, Index4;
    }
}
