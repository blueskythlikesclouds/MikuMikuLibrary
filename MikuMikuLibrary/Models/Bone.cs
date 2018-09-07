using System.Numerics;

namespace MikuMikuLibrary.Models
{
    public class Bone
    {
        public string Name { get; set; }
        public int ParentID { get; set; }
        public int ID { get; set; }
        public Matrix4x4 Matrix { get; set; }

        public Bone()
        {
            ParentID = -1;
        }
    }

    public struct BoneWeight
    {
        public float Weight1, Weight2, Weight3, Weight4;
        public int Index1, Index2, Index3, Index4;

        public static readonly BoneWeight Empty = new BoneWeight
        {
            Index1 = -1, Index2 = -1, Index3 = -1, Index4 = -1,
        };

        public void AddWeight( int index, float weight )
        {
            if ( Index1 < 0 )
            {
                Index1 = index;
                Weight1 = weight;
            }
            else if ( Index2 < 0 )
            {
                Index2 = index;
                Weight2 = weight;
            }
            else if ( Index3 < 0 )
            {
                Index3 = index;
                Weight3 = weight;
            }
            else if ( Index4 < 0 )
            {
                Index4 = index;
                Weight4 = weight;
            }
        }

        public override string ToString()
        {
            return $"<({Index1}, {Weight1}), ({Index2}, {Weight2}), ({Index3}, {Weight3}), ({Index4}, {Weight4})>";
        }
    }
}
