//========================================================//
// Taken from: https://github.com/TGEnigma/NvTriStrip.Net //
//========================================================//

using System;
using System.Collections.Generic;

namespace NvTriStripDotNet
{
    internal class FaceInfo : IEquatable<FaceInfo>
    {
        /// <summary>
        /// Vertex index 0.
        /// </summary>
        public int V0;

        /// <summary>
        /// Vertex index 1.
        /// </summary>
        public int V1;

        /// <summary>
        /// Vertex index 2.
        /// </summary>
        public int V2;

        /// <summary>
        /// Real strip Id.
        /// </summary>
        public int StripId;

        /// <summary>
        /// Strip Id in an experiment.
        /// </summary>
        public int TestStripId;

        /// <summary>
        /// In what experiment was it given an experiment Id?
        /// </summary>
        public int ExperimentId;

        /// <summary>
        /// If true, will be deleted when the strip it's in is deleted.
        /// </summary>
        public bool IsFake;

        public FaceInfo( int v0, int v1, int v2, bool isFake = false )
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
            StripId = -1;
            TestStripId = -1;
            ExperimentId = -1;
            IsFake = isFake;
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as FaceInfo );
        }

        public bool Equals( FaceInfo other )
        {
            return other != null &&
                     V0 == other.V0 &&
                     V1 == other.V1 &&
                     V2 == other.V2 &&
                     StripId == other.StripId &&
                     TestStripId == other.TestStripId &&
                     ExperimentId == other.ExperimentId &&
                     IsFake == other.IsFake;
        }

        public override int GetHashCode()
        {
            var hashCode = -2028054185;
            hashCode = hashCode * -1521134295 + V0.GetHashCode();
            hashCode = hashCode * -1521134295 + V1.GetHashCode();
            hashCode = hashCode * -1521134295 + V2.GetHashCode();
            hashCode = hashCode * -1521134295 + StripId.GetHashCode();
            hashCode = hashCode * -1521134295 + TestStripId.GetHashCode();
            hashCode = hashCode * -1521134295 + ExperimentId.GetHashCode();
            hashCode = hashCode * -1521134295 + IsFake.GetHashCode();
            return hashCode;
        }

        public static bool operator ==( FaceInfo info1, FaceInfo info2 )
        {
            return EqualityComparer<FaceInfo>.Default.Equals( info1, info2 );
        }

        public static bool operator !=( FaceInfo info1, FaceInfo info2 )
        {
            return !( info1 == info2 );
        }
    }
}