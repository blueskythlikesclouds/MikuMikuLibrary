//========================================================//
// Taken from: https://github.com/TGEnigma/NvTriStrip.Net //
//========================================================//

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NvTriStripDotNet
{
    /// <summary>
    /// This is a summary of a strip that has been built.
    /// </summary>
    internal class StripInfo
    {
        public StripStartInfo StartInfo;
        public List<FaceInfo> Faces;
        public int StripId;
        public int ExperimentId;
        public bool WasVisited;
        public int DegenerateCount;

        /// <summary>
        /// A little information about the creation of the triangle strips.
        /// </summary>
        public StripInfo( StripStartInfo startInfo, int stripId, int experimentId = -1 )
        {
            StartInfo = startInfo;
            StripId = stripId;
            ExperimentId = experimentId;
            WasVisited = false;
            DegenerateCount = 0;
            Faces = new List<FaceInfo>();
        }

        /// <summary>
        /// This is an experiment if the experiment id is >= 0.
        /// </summary>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool IsExperiment()
        {
            return ExperimentId >= 0;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool IsInStrip( FaceInfo faceInfo )
        {
            if ( faceInfo == null )
                return false;

            return ExperimentId >= 0 ? faceInfo.TestStripId == StripId : faceInfo.StripId == StripId;
        }

        /// <summary>
        /// Returns true if the input face and the current strip share an edge.
        /// </summary>
        /// <returns></returns>
        public bool SharesEdge( FaceInfo faceInfo, List<EdgeInfo> edgeInfos )
        {
            //check v0.v1 edge
            var currEdge = Stripifier.FindEdgeInfo( edgeInfos, faceInfo.V0, faceInfo.V1 );

            if ( IsInStrip( currEdge.Face0 ) || IsInStrip( currEdge.Face1 ) )
                return true;

            //check v1.v2 edge
            currEdge = Stripifier.FindEdgeInfo( edgeInfos, faceInfo.V1, faceInfo.V2 );

            if ( IsInStrip( currEdge.Face0 ) || IsInStrip( currEdge.Face1 ) )
                return true;

            //check v2.v0 edge
            currEdge = Stripifier.FindEdgeInfo( edgeInfos, faceInfo.V2, faceInfo.V0 );

            if ( IsInStrip( currEdge.Face0 ) || IsInStrip( currEdge.Face1 ) )
                return true;

            return false;
        }

        /// <summary>
        /// Combines the two input face vectors and puts the result into m_faces.
        /// </summary>
        public void Combine( List<FaceInfo> forward, List<FaceInfo> backward )
        {
            // add backward faces
            int numFaces = backward.Count;
            for ( int i = numFaces - 1; i >= 0; i-- )
                Faces.Add( backward[ i ] );

            // add forward faces
            numFaces = forward.Count;
            for ( var i = 0; i < numFaces; i++ )
                Faces.Add( forward[ i ] );
        }

        /// <summary>
        /// returns true if the face is "unique", i.e. has a vertex which doesn't exist in the faceVec.
        /// </summary>
        public bool Unique( List<FaceInfo> faceVec, FaceInfo face )
        {
            bool bv0, bv1, bv2; //bools to indicate whether a vertex is in the faceVec or not
            bv0 = bv1 = bv2 = false;

            for ( var i = 0; i < faceVec.Count; i++ )
            {
                if ( !bv0 )
                {
                    if ( faceVec[ i ].V0 == face.V0 ||
                         faceVec[ i ].V1 == face.V0 ||
                         faceVec[ i ].V2 == face.V0 )
                        bv0 = true;
                }

                if ( !bv1 )
                {
                    if ( faceVec[ i ].V0 == face.V1 ||
                         faceVec[ i ].V1 == face.V1 ||
                         faceVec[ i ].V2 == face.V1 )
                        bv1 = true;
                }

                if ( !bv2 )
                {
                    if ( faceVec[ i ].V0 == face.V2 ||
                         faceVec[ i ].V1 == face.V2 ||
                         faceVec[ i ].V2 == face.V2 )
                        bv2 = true;
                }

                //the face is not unique, all it's vertices exist in the face vector
                if ( bv0 && bv1 && bv2 )
                    return false;
            }

            //if we get out here, it's unique
            return true;
        }

        /// <summary>
        /// If either the faceInfo has a real strip index because it is
        /// already assign to a committed strip OR it is assigned in an
        /// experiment and the experiment index is the one we are building
        /// for, then it is marked and unavailable
        /// </summary>
        /// <param name="faceInfo"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool IsMarked( FaceInfo faceInfo )
        {
            return faceInfo.StripId >= 0 || IsExperiment() && faceInfo.ExperimentId == ExperimentId;
        }

        /// <summary>
        ///  Marks the face with the current strip ID.
        /// </summary>
        /// <param name="faceInfo"></param>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void MarkTriangle( FaceInfo faceInfo )
        {
            // TODO: why does this get hit?
            //Debug.Assert( !IsMarked( faceInfo ) );
            if ( IsExperiment() )
            {
                faceInfo.ExperimentId = ExperimentId;
                faceInfo.TestStripId = StripId;
            }
            else
            {
                Debug.Assert( faceInfo.StripId == -1 );
                faceInfo.ExperimentId = -1;
                faceInfo.StripId = StripId;
            }
        }

        /// <summary>
        /// Builds a strip forward as far as we can go, then builds backwards, and joins the two lists.
        /// </summary>
        public void Build( List<EdgeInfo> edgeInfos, List<FaceInfo> faceInfos )
        {
            // used in building the strips forward and backward
            var scratchIndices = new List<ushort>();

            // build forward... start with the initial face
            var forwardFaces = new List<FaceInfo>();
            var backwardFaces = new List<FaceInfo>();
            forwardFaces.Add( StartInfo.StartFace );

            MarkTriangle( StartInfo.StartFace );

            int v0 = StartInfo.ToV1 ? StartInfo.StartEdge.V0 : StartInfo.StartEdge.V1;
            int v1 = StartInfo.ToV1 ? StartInfo.StartEdge.V1 : StartInfo.StartEdge.V0;

            // easiest way to get v2 is to use this function which requires the
            // other indices to already be in the list.
            scratchIndices.Add( ( ushort )v0 );
            scratchIndices.Add( ( ushort )v1 );
            int v2 = Stripifier.GetNextIndex( scratchIndices, StartInfo.StartFace );
            scratchIndices.Add( ( ushort )v2 );

            //
            // build the forward list
            //
            int nv0 = v1;
            int nv1 = v2;

            var nextFace = Stripifier.FindOtherFace( edgeInfos, nv0, nv1, StartInfo.StartFace );
            while ( nextFace != null && !IsMarked( nextFace ) )
            {
                //check to see if this next face is going to cause us to die soon
                int testnv0 = nv1;
                int testnv1 = Stripifier.GetNextIndex( scratchIndices, nextFace );

                var nextNextFace = Stripifier.FindOtherFace( edgeInfos, testnv0, testnv1, nextFace );

                if ( nextNextFace == null || IsMarked( nextNextFace ) )
                {
                    //uh, oh, we're following a dead end, try swapping
                    var testNextFace = Stripifier.FindOtherFace( edgeInfos, nv0, testnv1, nextFace );

                    if ( testNextFace != null && !IsMarked( testNextFace ) )
                    {
                        //we only swap if it buys us something

                        //add a "fake" degenerate face
                        var tempFace = new FaceInfo( nv0, nv1, nv0, true );

                        forwardFaces.Add( tempFace );
                        MarkTriangle( tempFace );

                        scratchIndices.Add( ( ushort )nv0 );
                        testnv0 = nv0;

                        ++DegenerateCount;
                    }

                }

                // add this to the strip
                forwardFaces.Add( nextFace );

                MarkTriangle( nextFace );

                // add the index
                //nv0 = nv1;
                //nv1 = Stripifier.GetNextIndex(scratchIndices, nextFace);
                scratchIndices.Add( ( ushort )testnv1 );

                // and get the next face
                nv0 = testnv0;
                nv1 = testnv1;

                nextFace = Stripifier.FindOtherFace( edgeInfos, nv0, nv1, nextFace );

            }

            // tempAllFaces is going to be forwardFaces + backwardFaces
            // it's used for Unique()
            var tempAllFaces = new List<FaceInfo>();
            for ( var i = 0; i < forwardFaces.Count; i++ )
                tempAllFaces.Add( forwardFaces[ i ] );

            //
            // reset the indices for building the strip backwards and do so
            //
            scratchIndices.Clear();
            scratchIndices.Add( ( ushort )v2 );
            scratchIndices.Add( ( ushort )v1 );
            scratchIndices.Add( ( ushort )v0 );
            nv0 = v1;
            nv1 = v0;
            nextFace = Stripifier.FindOtherFace( edgeInfos, nv0, nv1, StartInfo.StartFace );
            while ( nextFace != null && !IsMarked( nextFace ) )
            {
                //this tests to see if a face is "unique", meaning that its vertices aren't already in the list
                // so, strips which "wrap-around" are not allowed
                if ( !Unique( tempAllFaces, nextFace ) )
                    break;

                //check to see if this next face is going to cause us to die soon
                int testnv0 = nv1;
                int testnv1 = Stripifier.GetNextIndex( scratchIndices, nextFace );

                var nextNextFace = Stripifier.FindOtherFace( edgeInfos, testnv0, testnv1, nextFace );

                if ( nextNextFace == null || IsMarked( nextNextFace ) )
                {
                    //uh, oh, we're following a dead end, try swapping
                    var testNextFace = Stripifier.FindOtherFace( edgeInfos, nv0, testnv1, nextFace );
                    if ( testNextFace != null && !IsMarked( testNextFace ) )
                    {
                        //we only swap if it buys us something

                        //add a "fake" degenerate face
                        var tempFace = new FaceInfo( nv0, nv1, nv0, true );

                        backwardFaces.Add( tempFace );
                        MarkTriangle( tempFace );
                        scratchIndices.Add( ( ushort )nv0 );
                        testnv0 = nv0;

                        ++DegenerateCount;
                    }

                }

                // add this to the strip
                backwardFaces.Add( nextFace );

                //this is just so Unique() will work
                tempAllFaces.Add( nextFace );

                MarkTriangle( nextFace );

                // add the index
                //nv0 = nv1;
                //nv1 = Stripifier.GetNextIndex(scratchIndices, nextFace);
                scratchIndices.Add( ( ushort )testnv1 );

                // and get the next face
                nv0 = testnv0;
                nv1 = testnv1;
                nextFace = Stripifier.FindOtherFace( edgeInfos, nv0, nv1, nextFace );
            }

            // Combine the forward and backwards stripification lists and put into our own face vector
            Combine( forwardFaces, backwardFaces );
        }
    }
}