//========================================================//
// Taken from: https://github.com/TGEnigma/NvTriStrip.Net //
//========================================================//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NvTriStripDotNet
{
    /// <summary>
    /// The actual stripifier.
    /// </summary>
    internal class Stripifier
    {
        private const int CACHE_INEFFICIENCY = 6;

        private List<ushort> mIndices;
        private int mCacheSize;
        private int mMinStripLength;
        private float mMeshJump;
        private bool mFirstTimeResetPoint;

        /// <summary>
        /// </summary>
        /// <param name="inIndices">the input indices of the mesh to stripify.</param>
        /// <param name="inCacheSize">the target cache size .</param>
        public void Stripify( List<ushort> inIndices, int inCacheSize, int inMinStripLength,
                              ushort maxIndex, List<StripInfo> outStrips, List<FaceInfo> outFaceList )
        {
            mMeshJump = 0.0f;
            mFirstTimeResetPoint = true; //used in FindGoodResetPoint()

            //the number of times to run the experiments
            var numSamples = 10;

            //the cache size, clamped to one
            mCacheSize = Math.Max( 1, inCacheSize - CACHE_INEFFICIENCY );

            mMinStripLength = inMinStripLength; //this is the strip size threshold below which we dump the strip into a list

            mIndices = inIndices;

            // build the stripification info
            var allFaceInfos = new List<FaceInfo>();
            var allEdgeInfos = new List<EdgeInfo>();

            BuildStripifyInfo( allFaceInfos, allEdgeInfos, maxIndex );

            var allStrips = new List<StripInfo>();

            // stripify
            FindAllStrips( allStrips, allFaceInfos, allEdgeInfos, numSamples );

            //split up the strips into cache friendly pieces, optimize them, then dump these into outStrips
            SplitUpStripsAndOptimize( allStrips, outStrips, allEdgeInfos, outFaceList );

            for ( var i = 0; i < allEdgeInfos.Count; i++ )
            {
                var info = allEdgeInfos[ i ];
                while ( info != null )
                {
                    var next = info.V0 == i ? info.NextV0 : info.NextV1;
                    info = next;
                }
            }
        }

        /// <summary>
        /// Generates actual strips from the list-in-strip-order.
        /// </summary>
        public void CreateStrips( List<StripInfo> allStrips, List<int> stripIndices, bool bStitchStrips, ref uint numSeparateStrips, bool bRestart,
                                  uint restartVal )
        {
            Debug.Assert( numSeparateStrips == 0 );

            var tLastFace = new FaceInfo( 0, 0, 0 );
            var tPrevStripLastFace = new FaceInfo( 0, 0, 0 );
            Debug.Assert( allStrips.Count > 0 );

            //we infer the cw/ccw ordering depending on the number of indices
            //this is screwed up by the fact that we insert -1s to denote changing strips
            //this is to account for that
            var accountForNegatives = 0;

            for ( var i = 0; i < allStrips.Count; i++ )
            {
                var strip = allStrips[ i ];
                int nStripFaceCount = strip.Faces.Count;
                Debug.Assert( nStripFaceCount > 0 );

                // Handle the first face in the strip
                {
                    var tFirstFace = new FaceInfo( strip.Faces[ 0 ].V0, strip.Faces[ 0 ].V1, strip.Faces[ 0 ].V2 );

                    // If there is a second face, reorder vertices such that the
                    // unique vertex is first
                    if ( nStripFaceCount > 1 )
                    {
                        int nUnique = Stripifier.GetUniqueVertexInB( strip.Faces[ 1 ], tFirstFace );
                        if ( nUnique == tFirstFace.V1 )
                        {
                            Utils.Swap( ref tFirstFace.V0, ref tFirstFace.V1 );
                        }
                        else if ( nUnique == tFirstFace.V2 )
                        {
                            Utils.Swap( ref tFirstFace.V0, ref tFirstFace.V2 );
                        }

                        // If there is a third face, reorder vertices such that the
                        // shared vertex is last
                        if ( nStripFaceCount > 2 )
                        {
                            if ( IsDegenerate( strip.Faces[ 1 ] ) )
                            {
                                int pivot = strip.Faces[ 1 ].V1;
                                if ( tFirstFace.V1 == pivot )
                                {
                                    Utils.Swap( ref tFirstFace.V1, ref tFirstFace.V2 );
                                }
                            }
                            else
                            {
                                GetSharedVertices( strip.Faces[ 2 ], tFirstFace, out int nShared0, out int nShared1 );
                                if ( nShared0 == tFirstFace.V1 && nShared1 == -1 )
                                {
                                    Utils.Swap( ref tFirstFace.V1, ref tFirstFace.V2 );
                                }
                            }
                        }
                    }

                    if ( i == 0 || !bStitchStrips || bRestart )
                    {
                        if ( !IsCw( strip.Faces[ 0 ], tFirstFace.V0, tFirstFace.V1 ) )
                            stripIndices.Add( tFirstFace.V0 );
                    }
                    else
                    {
                        // Double tap the first in the new strip
                        stripIndices.Add( tFirstFace.V0 );

                        // Check CW/CCW ordering
                        if ( NextIsCw( stripIndices.Count - accountForNegatives ) != IsCw( strip.Faces[ 0 ], tFirstFace.V0, tFirstFace.V1 ) )
                        {
                            stripIndices.Add( tFirstFace.V0 );
                        }
                    }

                    stripIndices.Add( tFirstFace.V0 );
                    stripIndices.Add( tFirstFace.V1 );
                    stripIndices.Add( tFirstFace.V2 );

                    // Update last face info
                    tLastFace = tFirstFace;
                }

                for ( var j = 1; j < nStripFaceCount; j++ )
                {
                    int nUnique = GetUniqueVertexInB( tLastFace, strip.Faces[ j ] );
                    if ( nUnique != -1 )
                    {
                        stripIndices.Add( nUnique );

                        // Update last face info
                        tLastFace.V0 = tLastFace.V1;
                        tLastFace.V1 = tLastFace.V2;
                        tLastFace.V2 = nUnique;
                    }
                    else
                    {
                        //we've hit a degenerate
                        stripIndices.Add( strip.Faces[ j ].V2 );
                        tLastFace.V0 = strip.Faces[ j ].V0; //tLastFace.m_v1;
                        tLastFace.V1 = strip.Faces[ j ].V1; //tLastFace.m_v2;
                        tLastFace.V2 = strip.Faces[ j ].V2; //tLastFace.m_v1;

                    }
                }

                // Double tap between strips.
                if ( bStitchStrips && !bRestart )
                {
                    if ( i != allStrips.Count - 1 )
                        stripIndices.Add( tLastFace.V2 );
                }
                else if ( bRestart )
                {
                    stripIndices.Add( ( int )restartVal );
                }
                else
                {
                    //-1 index indicates next strip
                    stripIndices.Add( -1 );
                    accountForNegatives++;
                    numSeparateStrips++;
                }

                // Update last face info
                tLastFace.V0 = tLastFace.V1;
                tLastFace.V1 = tLastFace.V2;

                // TGE: assignment to same variable?
                //tLastFace.m_v2 = tLastFace.m_v2;
            }

            if ( bStitchStrips || bRestart )
                numSeparateStrips = 1;
        }

        /// <summary>
        /// Returns the vertex unique to faceB.
        /// </summary>
        public static int GetUniqueVertexInB( FaceInfo faceA, FaceInfo faceB )
        {
            int facev0 = faceB.V0;
            if ( facev0 != faceA.V0 &&
                 facev0 != faceA.V1 &&
                 facev0 != faceA.V2 )
                return facev0;

            int facev1 = faceB.V1;
            if ( facev1 != faceA.V0 &&
                 facev1 != faceA.V1 &&
                 facev1 != faceA.V2 )
                return facev1;

            int facev2 = faceB.V2;
            if ( facev2 != faceA.V0 &&
                 facev2 != faceA.V1 &&
                 facev2 != faceA.V2 )
                return facev2;

            // nothing is different
            return -1;
        }

        public static void GetSharedVertices( FaceInfo faceA, FaceInfo faceB, out int vertex0, out int vertex1 )
        {
            vertex0 = -1;
            vertex1 = -1;

            int facev0 = faceB.V0;
            if ( facev0 == faceA.V0 ||
                 facev0 == faceA.V1 ||
                 facev0 == faceA.V2 )
            {
                if ( vertex0 == -1 )
                    vertex0 = facev0;
                else
                {
                    vertex1 = facev0;
                    return;
                }
            }

            int facev1 = faceB.V1;
            if ( facev1 == faceA.V0 ||
                 facev1 == faceA.V1 ||
                 facev1 == faceA.V2 )
            {
                if ( vertex0 == -1 )
                    vertex0 = facev1;
                else
                {
                    vertex1 = facev1;
                    return;
                }
            }

            int facev2 = faceB.V2;
            if ( facev2 == faceA.V0 ||
                 facev2 == faceA.V1 ||
                 facev2 == faceA.V2 )
            {
                if ( vertex0 == -1 )
                    vertex0 = facev2;
                else
                {
                    vertex1 = facev2;
                }
            }
        }

        public static bool IsDegenerate( FaceInfo face )
        {
            if ( face.V0 == face.V1 )
                return true;
            else if ( face.V0 == face.V2 )
                return true;
            else if ( face.V1 == face.V2 )
                return true;
            else
                return false;
        }

        public static bool IsDegenerate( ushort v0, ushort v1, ushort v2 )
        {
            if ( v0 == v1 )
                return true;
            else if ( v0 == v2 )
                return true;
            else if ( v1 == v2 )
                return true;
            else
                return false;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Big mess of functions called during stripification
        //
        /////////////////////////////////////////////////////////////////////////////////

        //********************
        private bool IsMoneyFace( FaceInfo face )
        {
            if ( FaceContainsIndex( face, 800 ) &&
                 FaceContainsIndex( face, 812 ) &&
                 FaceContainsIndex( face, 731 ) )
                return true;

            return false;
        }

        private bool FaceContainsIndex( FaceInfo face, uint index )
        {
            return face.V0 == index || face.V1 == index || face.V2 == index;
        }

        /// <summary>
        /// Returns true if the face is ordered in CW fashion.
        /// </summary>
        private bool IsCw( FaceInfo faceInfo, int v0, int v1 )
        {
            if ( faceInfo.V0 == v0 )
                return faceInfo.V1 == v1;

            else if ( faceInfo.V1 == v0 )
                return faceInfo.V2 == v1;

            else
                return faceInfo.V0 == v1;
        }

        /// <summary>
        /// Returns true if the next face should be ordered in CW fashion.
        /// </summary>
        private bool NextIsCw( int numIndices )
        {
            return numIndices % 2 == 0;
        }

        /// <summary>
        /// Returns vertex of the input face which is "next" in the input index list.
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        internal static int GetNextIndex( List<ushort> indices, FaceInfo face )
        {
            int numIndices = indices.Count;
            Debug.Assert( numIndices >= 2 );

            int v0 = indices[ numIndices - 2 ];
            int v1 = indices[ numIndices - 1 ];

            int fv0 = face.V0;
            int fv1 = face.V1;
            int fv2 = face.V2;

            if ( fv0 != v0 && fv0 != v1 )
            {
                if ( fv1 != v0 && fv1 != v1 || fv2 != v0 && fv2 != v1 )
                {
                    Debug.Write( "GetNextIndex: Triangle doesn't have all of its vertices\n" );
                    Debug.Write( "GetNextIndex: Duplicate triangle probably got us derailed\n" );
                }

                return fv0;
            }

            if ( fv1 != v0 && fv1 != v1 )
            {
                if ( fv0 != v0 && fv0 != v1 || fv2 != v0 && fv2 != v1 )
                {
                    Debug.Write( "GetNextIndex: Triangle doesn't have all of its vertices\n" );
                    Debug.Write( "GetNextIndex: Duplicate triangle probably got us derailed\n" );
                }

                return fv1;
            }

            if ( fv2 != v0 && fv2 != v1 )
            {
                if ( fv0 != v0 && fv0 != v1 || fv1 != v0 && fv1 != v1 )
                {
                    Debug.Write( "GetNextIndex: Triangle doesn't have all of its vertices\n" );
                    Debug.Write( "GetNextIndex: Duplicate triangle probably got us derailed\n" );
                }

                return fv2;
            }

            // shouldn't get here, but let's try and fail gracefully
            if ( fv0 == fv1 || fv0 == fv2 )
                return fv0;
            else if ( fv1 == fv0 || fv1 == fv2 )
                return fv1;
            else if ( fv2 == fv0 || fv2 == fv1 )
                return fv2;
            else
                return -1;
        }

        /// <summary>
        /// Find the edge info for these two indices.
        /// </summary>
        internal static EdgeInfo FindEdgeInfo( List<EdgeInfo> edgeInfos, int v0, int v1 )
        {
            // we can get to it through either array
            // because the edge infos have a v0 and v1
            // and there is no order except how it was
            // first created.
            var infoIter = edgeInfos[ v0 ];
            while ( infoIter != null )
            {
                if ( infoIter.V0 == v0 )
                {
                    if ( infoIter.V1 == v1 )
                        return infoIter;
                    else
                        infoIter = infoIter.NextV0;
                }
                else
                {
                    Debug.Assert( infoIter.V1 == v0 );
                    if ( infoIter.V0 == v1 )
                        return infoIter;
                    else
                        infoIter = infoIter.NextV1;
                }
            }

            return null;
        }

        /// <summary>
        /// find the other face sharing these vertices
        /// exactly like the edge info above
        /// </summary>
        internal static FaceInfo FindOtherFace( List<EdgeInfo> edgeInfos, int v0, int v1, FaceInfo faceInfo )
        {
            var edgeInfo = FindEdgeInfo( edgeInfos, v0, v1 );

            if ( edgeInfo == null && v0 == v1 )
            {
                //we've hit a degenerate
                return null;
            }

            Debug.Assert( edgeInfo != null );
            return edgeInfo.Face0 == faceInfo ? edgeInfo.Face1 : edgeInfo.Face0;
        }

        /// <summary>
        /// A good reset point is one near other commited areas so that
        /// we know that when we've made the longest strips its because
        /// we're stripifying in the same general orientation.
        /// </summary>
        private FaceInfo FindGoodResetPoint( List<FaceInfo> faceInfos, List<EdgeInfo> edgeInfos )
        {
            // we hop into different areas of the mesh to try to get
            // other large open spans done.  Areas of small strips can
            // just be left to triangle lists added at the end.
            FaceInfo result = null;

            if ( result == null )
            {
                int numFaces = faceInfos.Count;
                int startPoint;
                if ( mFirstTimeResetPoint )
                {
                    //first time, find a face with few neighbors (look for an edge of the mesh)
                    startPoint = FindStartPoint( faceInfos, edgeInfos );
                    mFirstTimeResetPoint = false;
                }
                else
                    startPoint = ( int )( ( ( float )numFaces - 1 ) * mMeshJump );

                if ( startPoint == -1 )
                {
                    startPoint = ( int )( ( ( float )numFaces - 1 ) * mMeshJump );

                    //meshJump += 0.1f;
                    //if (meshJump > 1.0f)
                    //	meshJump = .05f;
                }

                int i = startPoint;
                do
                {

                    // if this guy isn't visited, try him
                    if ( faceInfos[ i ].StripId < 0 )
                    {
                        result = faceInfos[ i ];
                        break;
                    }

                    // update the index and clamp to 0-(numFaces-1)
                    if ( ++i >= numFaces )
                        i = 0;

                }
                while ( i != startPoint );

                // update the meshJump
                mMeshJump += 0.1f;
                if ( mMeshJump > 1.0f )
                    mMeshJump = .05f;
            }

            // return the best face we found
            return result;
        }

        /// <summary>
        /// Does the stripification, puts output strips into vector allStrips.
        /// Works by setting runnning a number of experiments in different areas of the mesh, and
        /// accepting the one which results in the longest strips.  It then accepts this, and moves
        /// on to a different area of the mesh.  We try to jump around the mesh some, to ensure that
        /// large open spans of strips get generated.
        /// </summary>
        private void FindAllStrips( List<StripInfo> allStrips, List<FaceInfo> allFaceInfos, List<EdgeInfo> allEdgeInfos, int numSamples )
        {
            // the experiments
            var experimentId = 0;
            var stripId = 0;
            var done = false;

            var loopCtr = 0;

            while ( !done )
            {
                loopCtr++;

                //
                // PHASE 1: Set up numSamples * numEdges experiments
                //
                var experiments = new List<StripInfo>[ numSamples * 6 ];
                for ( var i = 0; i < experiments.Length; ++i )
                    experiments[ i ] = new List<StripInfo>();

                var experimentIndex = 0;
                var resetPoints = new HashSet<FaceInfo>();
                for ( var i = 0; i < numSamples; i++ )
                {

                    // Try to find another good reset point.
                    // If there are none to be found, we are done
                    var nextFace = FindGoodResetPoint( allFaceInfos, allEdgeInfos );
                    if ( nextFace == null )
                    {
                        done = true;
                        break;
                    }
                    // If we have already evaluated starting at this face in this slew
                    // of experiments, then skip going any further
                    else if ( resetPoints.Contains( nextFace ) )
                    {
                        continue;
                    }

                    // trying it now...
                    resetPoints.Add( nextFace );

                    // otherwise, we shall now try experiments for starting on the 01,12, and 20 edges
                    Debug.Assert( nextFace.StripId < 0 );

                    // build the strip off of this face's 0-1 edge
                    var edge01 = FindEdgeInfo( allEdgeInfos, nextFace.V0, nextFace.V1 );
                    var strip01 = new StripInfo( new StripStartInfo( nextFace, edge01, true ), stripId++, experimentId++ );
                    experiments[ experimentIndex++ ].Add( strip01 );

                    // build the strip off of this face's 1-0 edge
                    var edge10 = FindEdgeInfo( allEdgeInfos, nextFace.V0, nextFace.V1 );
                    var strip10 = new StripInfo( new StripStartInfo( nextFace, edge10, false ), stripId++, experimentId++ );
                    experiments[ experimentIndex++ ].Add( strip10 );

                    // build the strip off of this face's 1-2 edge
                    var edge12 = FindEdgeInfo( allEdgeInfos, nextFace.V1, nextFace.V2 );
                    var strip12 = new StripInfo( new StripStartInfo( nextFace, edge12, true ), stripId++, experimentId++ );
                    experiments[ experimentIndex++ ].Add( strip12 );

                    // build the strip off of this face's 2-1 edge
                    var edge21 = FindEdgeInfo( allEdgeInfos, nextFace.V1, nextFace.V2 );
                    var strip21 = new StripInfo( new StripStartInfo( nextFace, edge21, false ), stripId++, experimentId++ );
                    experiments[ experimentIndex++ ].Add( strip21 );

                    // build the strip off of this face's 2-0 edge
                    var edge20 = FindEdgeInfo( allEdgeInfos, nextFace.V2, nextFace.V0 );
                    var strip20 = new StripInfo( new StripStartInfo( nextFace, edge20, true ), stripId++, experimentId++ );
                    experiments[ experimentIndex++ ].Add( strip20 );

                    // build the strip off of this face's 0-2 edge
                    var edge02 = FindEdgeInfo( allEdgeInfos, nextFace.V2, nextFace.V0 );
                    var strip02 = new StripInfo( new StripStartInfo( nextFace, edge02, false ), stripId++, experimentId++ );
                    experiments[ experimentIndex++ ].Add( strip02 );
                }

                //
                // PHASE 2: Iterate through that we setup in the last phase
                // and really build each of the strips and strips that follow to see how
                // far we get
                //
                int numExperiments = experimentIndex;
                for ( var i = 0; i < numExperiments; i++ )
                {

                    // get the strip set

                    // build the first strip of the list
                    experiments[ i ][ 0 ].Build( allEdgeInfos, allFaceInfos );
                    experimentId = experiments[ i ][ 0 ].ExperimentId;

                    var stripIter = experiments[ i ][ 0 ];
                    var startInfo = new StripStartInfo( null, null, false );
                    while ( FindTraversal( allFaceInfos, allEdgeInfos, stripIter, startInfo ) )
                    {

                        // create the new strip info
                        stripIter = new StripInfo( startInfo, stripId++, experimentId );

                        // build the next strip
                        stripIter.Build( allEdgeInfos, allFaceInfos );

                        // add it to the list
                        experiments[ i ].Add( stripIter );
                    }
                }

                //
                // Phase 3: Find the experiment that has the most promise
                //
                var bestIndex = 0;
                double bestValue = 0;
                for ( var i = 0; i < numExperiments; i++ )
                {
                    var avgStripSizeWeight = 1.0f;
                    //var numTrisWeight = 0.0f;
                    var numStripsWeight = 0.0f;
                    float avgStripSize = AvgStripSize( experiments[ i ] );
                    var numStrips = ( float )experiments[ i ].Count;
                    float value = avgStripSize * avgStripSizeWeight + numStrips * numStripsWeight;
                    //float value = 1.f / numStrips;
                    //float value = numStrips * avgStripSize;

                    if ( value > bestValue )
                    {
                        bestValue = value;
                        bestIndex = i;
                    }
                }

                //
                // Phase 4: commit the best experiment of the bunch
                //
                CommitStrips( allStrips, experiments[ bestIndex ] );

                // and destroy all of the others
                for ( var i = 0; i < numExperiments; i++ )
                {
                    if ( i != bestIndex )
                    {
                        int numStrips = experiments[ i ].Count;
                        for ( var j = 0; j < numStrips; j++ )
                        {
                            var currStrip = experiments[ i ][ j ];
                            //delete all bogus faces in the experiments
                            for ( var k = 0; k < currStrip.Faces.Count; ++k )
                            {
                                if ( currStrip.Faces[ k ].IsFake )
                                {
                                    //delete currStrip.m_faces[k];
                                    currStrip.Faces[ k ] = null;
                                }
                            }

                            //delete currStrip;
                            currStrip = null;
                            experiments[ i ][ j ] = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Splits the input vector of strips (allBigStrips) into smaller, cache friendly pieces, then
        /// reorders these pieces to maximize cache hits
        /// The final strips are output through outStrips
        /// </summary>
        private void SplitUpStripsAndOptimize( List<StripInfo> allStrips, List<StripInfo> outStrips, List<EdgeInfo> edgeInfos,
                                                List<FaceInfo> outFaceList )
        {
            int threshold = mCacheSize;
            var tempStrips = new List<StripInfo>();

            //split up strips into threshold-sized pieces
            for ( var i = 0; i < allStrips.Count; i++ )
            {
                StripInfo currentStrip;
                var startInfo = new StripStartInfo( null, null, false );

                var actualStripSize = 0;
                for ( var j = 0; j < allStrips[ i ].Faces.Count; ++j )
                {
                    if ( !IsDegenerate( allStrips[ i ].Faces[ j ] ) )
                        actualStripSize++;
                }

                if ( actualStripSize /*allStrips[i].m_faces.Count*/ > threshold )
                {

                    int numTimes = actualStripSize /*allStrips[i].m_faces.Count*/ / threshold;
                    int numLeftover = actualStripSize /*allStrips[i].m_faces.Count*/ % threshold;

                    var degenerateCount = 0;
                    int j;
                    for ( j = 0; j < numTimes; j++ )
                    {
                        currentStrip = new StripInfo( startInfo, 0, -1 );

                        int faceCtr = j * threshold + degenerateCount;
                        var bFirstTime = true;
                        while ( faceCtr < threshold + j * threshold + degenerateCount )
                        {
                            if ( IsDegenerate( allStrips[ i ].Faces[ faceCtr ] ) )
                            {
                                degenerateCount++;

                                //last time or first time through, no need for a degenerate
                                if ( ( faceCtr + 1 != threshold + j * threshold + degenerateCount ||
                                       j == numTimes - 1 && numLeftover < 4 && numLeftover > 0 ) &&
                                     !bFirstTime )
                                {
                                    currentStrip.Faces.Add( allStrips[ i ].Faces[ faceCtr++ ] );
                                }
                                else
                                {
                                    //but, we do need to delete the degenerate, if it's marked fake, to avoid leaking
                                    // if(allStrips[i].m_faces[faceCtr].m_bIsFake)
                                    // {
                                    // delete allStrips[i].m_faces[faceCtr], allStrips[i].m_faces[faceCtr] = null;
                                    // }
                                    ++faceCtr;
                                }
                            }
                            else
                            {
                                currentStrip.Faces.Add( allStrips[ i ].Faces[ faceCtr++ ] );
                                bFirstTime = false;
                            }
                        }

                        /*
                        for(int faceCtr = j*threshold; faceCtr < threshold+(j*threshold); faceCtr++)
                        {
                            currentStrip.m_faces.Add(allStrips[i].m_faces[faceCtr]);
                        }
                        */
                        if ( j == numTimes - 1 ) //last time through
                        {
                            if ( numLeftover < 4 && numLeftover > 0 ) //way too small
                            {
                                //just add to last strip
                                var ctr = 0;
                                while ( ctr < numLeftover )
                                {
                                    if ( IsDegenerate( allStrips[ i ].Faces[ faceCtr ] ) )
                                        ++degenerateCount;
                                    else
                                        ++ctr;

                                    currentStrip.Faces.Add( allStrips[ i ].Faces[ faceCtr++ ] );
                                }

                                numLeftover = 0;
                            }
                        }

                        tempStrips.Add( currentStrip );
                    }

                    int leftOff = j * threshold + degenerateCount;

                    if ( numLeftover != 0 )
                    {
                        currentStrip = new StripInfo( startInfo, 0, -1 );

                        var ctr = 0;
                        var bFirstTime = true;
                        while ( ctr < numLeftover )
                        {
                            if ( !IsDegenerate( allStrips[ i ].Faces[ leftOff ] ) )
                            {
                                ctr++;
                                bFirstTime = false;
                                currentStrip.Faces.Add( allStrips[ i ].Faces[ leftOff++ ] );
                            }
                            else if ( !bFirstTime )
                                currentStrip.Faces.Add( allStrips[ i ].Faces[ leftOff++ ] );
                            else
                            {
                                //don't leak
                                // if(allStrips[i].m_faces[leftOff].m_bIsFake)
                                // {
                                // delete allStrips[i].m_faces[leftOff], allStrips[i].m_faces[leftOff] = null;
                                // }

                                leftOff++;
                            }
                        }
                        /*
                        for(int k = 0; k < numLeftover; k++)
                        {
                            currentStrip.m_faces.Add(allStrips[i].m_faces[leftOff++]);
                        }
                        */

                        tempStrips.Add( currentStrip );
                    }
                }
                else
                {
                    //we're not just doing a tempStrips.Add(allBigStrips[i]) because
                    // this way we can delete allBigStrips later to free the memory
                    currentStrip = new StripInfo( startInfo, 0, -1 );

                    for ( var j = 0; j < allStrips[ i ].Faces.Count; j++ )
                        currentStrip.Faces.Add( allStrips[ i ].Faces[ j ] );

                    tempStrips.Add( currentStrip );
                }
            }

            //add small strips to face list
            var tempStrips2 = new List<StripInfo>();
            RemoveSmallStrips( tempStrips, tempStrips2, outFaceList );

            outStrips.Clear();
            //screw optimization for now
            //	for(i = 0; i < tempStrips.Count; ++i)
            //    outStrips.Add(tempStrips[i]);

            if ( tempStrips2.Count != 0 )
            {
                //Optimize for the vertex cache
                var vcache = new VertexCache( mCacheSize );

                float bestNumHits = -1.0f;
                float numHits;
                var bestIndex = 0;
                //var done = false;

                var firstIndex = 0;
                var minCost = 10000.0f;

                for ( var i = 0; i < tempStrips2.Count; i++ )
                {
                    var numNeighbors = 0;

                    //find strip with least number of neighbors per face
                    for ( var j = 0; j < tempStrips2[ i ].Faces.Count; j++ )
                    {
                        numNeighbors += NumNeighbors( tempStrips2[ i ].Faces[ j ], edgeInfos );
                    }

                    float currCost = ( float )numNeighbors / ( float )tempStrips2[ i ].Faces.Count;
                    if ( currCost < minCost )
                    {
                        minCost = currCost;
                        firstIndex = i;
                    }
                }

                UpdateCacheStrip( vcache, tempStrips2[ firstIndex ] );
                outStrips.Add( tempStrips2[ firstIndex ] );

                tempStrips2[ firstIndex ].WasVisited = true;

                bool bWantsCw = tempStrips2[ firstIndex ].Faces.Count % 2 == 0;

                //this n^2 algo is what slows down stripification so much....
                // needs to be improved
                while ( true )
                {
                    bestNumHits = -1.0f;

                    //find best strip to add next, given the current cache
                    for ( var i = 0; i < tempStrips2.Count; i++ )
                    {
                        if ( tempStrips2[ i ].WasVisited )
                            continue;

                        numHits = CalcNumHitsStrip( vcache, tempStrips2[ i ] );
                        if ( numHits > bestNumHits )
                        {
                            bestNumHits = numHits;
                            bestIndex = i;
                        }
                        else if ( numHits >= bestNumHits )
                        {
                            //check previous strip to see if this one requires it to switch polarity
                            var strip = tempStrips2[ i ];
                            int nStripFaceCount = strip.Faces.Count;

                            var tFirstFace = new FaceInfo( strip.Faces[ 0 ].V0, strip.Faces[ 0 ].V1, strip.Faces[ 0 ].V2 );

                            // If there is a second face, reorder vertices such that the
                            // unique vertex is first
                            if ( nStripFaceCount > 1 )
                            {
                                int nUnique = Stripifier.GetUniqueVertexInB( strip.Faces[ 1 ], tFirstFace );
                                if ( nUnique == tFirstFace.V1 )
                                {
                                    Utils.Swap( ref tFirstFace.V0, ref tFirstFace.V1 );
                                }
                                else if ( nUnique == tFirstFace.V2 )
                                {
                                    Utils.Swap( ref tFirstFace.V0, ref tFirstFace.V2 );
                                }

                                // If there is a third face, reorder vertices such that the
                                // shared vertex is last
                                if ( nStripFaceCount > 2 )
                                {
                                    GetSharedVertices( strip.Faces[ 2 ], tFirstFace, out int nShared0, out int nShared1 );
                                    if ( nShared0 == tFirstFace.V1 && nShared1 == -1 )
                                    {
                                        Utils.Swap( ref tFirstFace.V1, ref tFirstFace.V2 );
                                    }
                                }
                            }

                            // Check CW/CCW ordering
                            if ( bWantsCw == IsCw( strip.Faces[ 0 ], tFirstFace.V0, tFirstFace.V1 ) )
                            {
                                //I like this one!
                                bestIndex = i;
                            }
                        }
                    }

                    if ( bestNumHits == -1.0f )
                        break;
                    tempStrips2[ bestIndex ].WasVisited = true;
                    UpdateCacheStrip( vcache, tempStrips2[ bestIndex ] );
                    outStrips.Add( tempStrips2[ bestIndex ] );
                    bWantsCw = tempStrips2[ bestIndex ].Faces.Count % 2 == 0 ? bWantsCw : !bWantsCw;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allStrips">the whole strip vector...all small strips will be deleted from this list, to avoid leaking mem.</param>
        /// <param name="allBigStrips">an out parameter which will contain all strips above minStripLength.</param>
        /// <param name="faceList">an out parameter which will contain all faces which were removed from the striplist.</param>
        private void RemoveSmallStrips( List<StripInfo> allStrips, List<StripInfo> allBigStrips, List<FaceInfo> faceList )
        {
            faceList.Clear();
            allBigStrips.Clear(); //make sure these are empty
            var tempFaceList = new List<FaceInfo>();

            for ( var i = 0; i < allStrips.Count; i++ )
            {
                if ( allStrips[ i ].Faces.Count < mMinStripLength )
                {
                    //strip is too small, add faces to faceList
                    for ( var j = 0; j < allStrips[ i ].Faces.Count; j++ )
                        tempFaceList.Add( allStrips[ i ].Faces[ j ] );
                }
                else
                {
                    allBigStrips.Add( allStrips[ i ] );
                }
            }

            if ( tempFaceList.Count > 0 )
            {
                var bVisitedList = new bool[ tempFaceList.Count ];

                var vcache = new VertexCache( mCacheSize );

                int bestNumHits = -1;
                int numHits;
                var bestIndex = 0;

                while ( true )
                {
                    bestNumHits = -1;

                    //find best face to add next, given the current cache
                    for ( var i = 0; i < tempFaceList.Count; i++ )
                    {
                        if ( bVisitedList[ i ] )
                            continue;

                        numHits = CalcNumHitsFace( vcache, tempFaceList[ i ] );
                        if ( numHits > bestNumHits )
                        {
                            bestNumHits = numHits;
                            bestIndex = i;
                        }
                    }

                    if ( bestNumHits == -1.0f )
                        break;
                    bVisitedList[ bestIndex ] = true;
                    UpdateCacheFace( vcache, tempFaceList[ bestIndex ] );
                    faceList.Add( tempFaceList[ bestIndex ] );
                }
            }
        }

        private bool FindTraversal( List<FaceInfo> faceInfos, List<EdgeInfo> edgeInfos, StripInfo strip, StripStartInfo startInfo )
        {
            // if the strip was v0.v1 on the edge, then v1 will be a vertex in the next edge.
            int v = strip.StartInfo.ToV1 ? strip.StartInfo.StartEdge.V1 : strip.StartInfo.StartEdge.V0;

            FaceInfo untouchedFace = null;
            var edgeIter = edgeInfos[ v ];
            while ( edgeIter != null )
            {
                var face0 = edgeIter.Face0;
                var face1 = edgeIter.Face1;
                if ( face0 != null && !strip.IsInStrip( face0 ) && face1 != null && !strip.IsMarked( face1 ) )
                {
                    untouchedFace = face1;
                    break;
                }

                if ( face1 != null && !strip.IsInStrip( face1 ) && face0 != null && !strip.IsMarked( face0 ) )
                {
                    untouchedFace = face0;
                    break;
                }

                // find the next edgeIter
                edgeIter = edgeIter.V0 == v ? edgeIter.NextV0 : edgeIter.NextV1;
            }

            startInfo.StartFace = untouchedFace;
            startInfo.StartEdge = edgeIter;
            if ( edgeIter != null )
            {
                if ( strip.SharesEdge( startInfo.StartFace, edgeInfos ) )
                    startInfo.ToV1 = edgeIter.V0 == v; //note! used to be m_v1
                else
                    startInfo.ToV1 = edgeIter.V1 == v;
            }

            return startInfo.StartFace != null;
        }

        // internal int  CountRemainingTris(std.list<FaceInfo>.iterator iter, std.list<FaceInfo>.iterator  end)
        // {
        // std.list<StripInfo*>.iterator  end)
        // int count = 0;
        // while (iter != end){
        // count += (*iter).m_faces.Count;
        // iter++;
        // }
        // return count;
        // }

        /// <summary>
        /// "Commits" the input strips by setting their m_experimentId to -1 and adding to the allStrips
        ///  vector
        /// </summary>
        private void CommitStrips( List<StripInfo> allStrips, List<StripInfo> strips )
        {
            // Iterate through strips
            for ( var i = 0; i < strips.Count; i++ )
            {

                // Tell the strip that it is now real
                var strip = strips[ i ];
                strip.ExperimentId = -1;

                // add to the list of real strips
                allStrips.Add( strip );

                // Iterate through the faces of the strip
                // Tell the faces of the strip that they belong to a real strip now
                var faces = strips[ i ].Faces;

                for ( var j = 0; j < faces.Count; j++ )
                {
                    strip.MarkTriangle( faces[ j ] );
                }
            }
        }

        /// <summary>
        /// Finds the average strip size of the input vector of strips.
        /// </summary>
        private float AvgStripSize( List<StripInfo> strips )
        {
            var sizeAccum = 0;
            for ( var i = 0; i < strips.Count; i++ )
            {
                var strip = strips[ i ];
                sizeAccum += strip.Faces.Count;
                sizeAccum -= strip.DegenerateCount;
            }

            return ( float )sizeAccum / ( float )strips.Count;
        }

        /// <summary>
        /// Finds a good starting point, namely one which has only one neighbor
        /// </summary>
        private int FindStartPoint( List<FaceInfo> faceInfos, List<EdgeInfo> edgeInfos )
        {
            int bestCtr = -1;
            int bestIndex = -1;

            for ( var i = 0; i < faceInfos.Count; i++ )
            {
                var ctr = 0;

                if ( FindOtherFace( edgeInfos, faceInfos[ i ].V0, faceInfos[ i ].V1, faceInfos[ i ] ) == null )
                    ctr++;
                if ( FindOtherFace( edgeInfos, faceInfos[ i ].V1, faceInfos[ i ].V2, faceInfos[ i ] ) == null )
                    ctr++;
                if ( FindOtherFace( edgeInfos, faceInfos[ i ].V2, faceInfos[ i ].V0, faceInfos[ i ] ) == null )
                    ctr++;
                if ( ctr > bestCtr )
                {
                    bestCtr = ctr;
                    bestIndex = i;
                    //return i;
                }
            }
            //return -1;

            if ( bestCtr == 0 )
                return -1;
            else
                return bestIndex;
        }

        /// <summary>
        /// Updates the input vertex cache with this strip's vertices.
        /// </summary>
        private void UpdateCacheStrip( VertexCache vcache, StripInfo strip )
        {
            for ( var i = 0; i < strip.Faces.Count; ++i )
            {
                if ( !vcache.InCache( strip.Faces[ i ].V0 ) )
                    vcache.AddEntry( strip.Faces[ i ].V0 );

                if ( !vcache.InCache( strip.Faces[ i ].V1 ) )
                    vcache.AddEntry( strip.Faces[ i ].V1 );

                if ( !vcache.InCache( strip.Faces[ i ].V2 ) )
                    vcache.AddEntry( strip.Faces[ i ].V2 );
            }
        }

        /// <summary>
        /// Updates the input vertex cache with this face's vertices.
        /// </summary>
        private void UpdateCacheFace( VertexCache vcache, FaceInfo face )
        {
            if ( !vcache.InCache( face.V0 ) )
                vcache.AddEntry( face.V0 );

            if ( !vcache.InCache( face.V1 ) )
                vcache.AddEntry( face.V1 );

            if ( !vcache.InCache( face.V2 ) )
                vcache.AddEntry( face.V2 );
        }

        /// <summary>
        /// Returns the number of cache hits per face in the strip.
        /// </summary>
        /// <param name="vcache"></param>
        /// <param name="strip"></param>
        /// <returns></returns>
        private float CalcNumHitsStrip( VertexCache vcache, StripInfo strip )
        {
            var numHits = 0;
            var numFaces = 0;

            for ( var i = 0; i < strip.Faces.Count; i++ )
            {
                if ( vcache.InCache( strip.Faces[ i ].V0 ) )
                    ++numHits;

                if ( vcache.InCache( strip.Faces[ i ].V1 ) )
                    ++numHits;

                if ( vcache.InCache( strip.Faces[ i ].V2 ) )
                    ++numHits;

                numFaces++;
            }

            return ( float )numHits / ( float )numFaces;
        }

        /// <summary>
        /// Returns the number of cache hits in the face.
        /// </summary>
        private int CalcNumHitsFace( VertexCache vcache, FaceInfo face )
        {
            var numHits = 0;

            if ( vcache.InCache( face.V0 ) )
                numHits++;

            if ( vcache.InCache( face.V1 ) )
                numHits++;

            if ( vcache.InCache( face.V2 ) )
                numHits++;

            return numHits;
        }

        /// <summary>
        /// Returns the number of neighbors that this face has.
        /// </summary>
        private int NumNeighbors( FaceInfo face, List<EdgeInfo> edgeInfoVec )
        {
            var numNeighbors = 0;

            if ( FindOtherFace( edgeInfoVec, face.V0, face.V1, face ) != null )
            {
                numNeighbors++;
            }

            if ( FindOtherFace( edgeInfoVec, face.V1, face.V2, face ) != null )
            {
                numNeighbors++;
            }

            if ( FindOtherFace( edgeInfoVec, face.V2, face.V0, face ) != null )
            {
                numNeighbors++;
            }

            return numNeighbors;
        }


        /// <summary>
        /// Builds the list of all face and edge infos.
        /// </summary>
        private void BuildStripifyInfo( List<FaceInfo> faceInfos, List<EdgeInfo> edgeInfos, ushort maxIndex )
        {
            // reserve space for the face infos, but do not resize them.
            int numIndices = mIndices.Count;
            faceInfos.Capacity = numIndices / 3;

            // we actually resize the edge infos, so we must initialize to null
            for ( var i = 0; i < maxIndex + 1; i++ )
            {
                edgeInfos.Add( null );
            }


            // iterate through the triangles of the triangle list
            int numTriangles = numIndices / 3;
            var index = 0;
            var bFaceUpdated = new bool[ 3 ];

            for ( var i = 0; i < numTriangles; i++ )
            {
                var bMightAlreadyExist = true;
                bFaceUpdated[ 0 ] = false;
                bFaceUpdated[ 1 ] = false;
                bFaceUpdated[ 2 ] = false;

                // grab the indices
                int v0 = mIndices[ index++ ];
                int v1 = mIndices[ index++ ];
                int v2 = mIndices[ index++ ];

                //we disregard degenerates
                if ( IsDegenerate( ( ushort )v0, ( ushort )v1, ( ushort )v2 ) )
                    continue;

                // create the face info and add it to the list of faces, but only if this exact face doesn't already 
                //  exist in the list
                var faceInfo = new FaceInfo( v0, v1, v2 );

                // grab the edge infos, creating them if they do not already exist
                var edgeInfo01 = FindEdgeInfo( edgeInfos, v0, v1 );
                if ( edgeInfo01 == null )
                {
                    //since one of it's edges isn't in the edge data structure, it can't already exist in the face structure
                    bMightAlreadyExist = false;

                    // create the info
                    edgeInfo01 = new EdgeInfo( v0, v1 );

                    // update the linked list on both 
                    edgeInfo01.NextV0 = edgeInfos[ v0 ];
                    edgeInfo01.NextV1 = edgeInfos[ v1 ];
                    edgeInfos[ v0 ] = edgeInfo01;
                    edgeInfos[ v1 ] = edgeInfo01;

                    // set face 0
                    edgeInfo01.Face0 = faceInfo;
                }
                else
                {
                    if ( edgeInfo01.Face1 != null )
                    {
                        Debug.Write( "BuildStripifyInfo: > 2 triangles on an edge... uncertain consequences\n" );
                    }
                    else
                    {
                        edgeInfo01.Face1 = faceInfo;
                        bFaceUpdated[ 0 ] = true;
                    }
                }

                // grab the edge infos, creating them if they do not already exist
                var edgeInfo12 = FindEdgeInfo( edgeInfos, v1, v2 );
                if ( edgeInfo12 == null )
                {
                    bMightAlreadyExist = false;

                    // create the info
                    edgeInfo12 = new EdgeInfo( v1, v2 );

                    // update the linked list on both 
                    edgeInfo12.NextV0 = edgeInfos[ v1 ];
                    edgeInfo12.NextV1 = edgeInfos[ v2 ];
                    edgeInfos[ v1 ] = edgeInfo12;
                    edgeInfos[ v2 ] = edgeInfo12;

                    // set face 0
                    edgeInfo12.Face0 = faceInfo;
                }
                else
                {
                    if ( edgeInfo12.Face1 != null )
                    {
                        Debug.Write( "BuildStripifyInfo: > 2 triangles on an edge... uncertain consequences\n" );
                    }
                    else
                    {
                        edgeInfo12.Face1 = faceInfo;
                        bFaceUpdated[ 1 ] = true;
                    }
                }

                // grab the edge infos, creating them if they do not already exist
                var edgeInfo20 = FindEdgeInfo( edgeInfos, v2, v0 );
                if ( edgeInfo20 == null )
                {
                    bMightAlreadyExist = false;

                    // create the info
                    edgeInfo20 = new EdgeInfo( v2, v0 );

                    // update the linked list on both 
                    edgeInfo20.NextV0 = edgeInfos[ v2 ];
                    edgeInfo20.NextV1 = edgeInfos[ v0 ];
                    edgeInfos[ v2 ] = edgeInfo20;
                    edgeInfos[ v0 ] = edgeInfo20;

                    // set face 0
                    edgeInfo20.Face0 = faceInfo;
                }
                else
                {
                    if ( edgeInfo20.Face1 != null )
                    {
                        Debug.Write( "BuildStripifyInfo: > 2 triangles on an edge... uncertain consequences\n" );
                    }
                    else
                    {
                        edgeInfo20.Face1 = faceInfo;
                        bFaceUpdated[ 2 ] = true;
                    }
                }

                if ( bMightAlreadyExist )
                {
                    if ( !AlreadyExists( faceInfo, faceInfos ) )
                        faceInfos.Add( faceInfo );
                    else
                    {
                        //cleanup pointers that point to this deleted face
                        if ( bFaceUpdated[ 0 ] )
                            edgeInfo01.Face1 = null;
                        if ( bFaceUpdated[ 1 ] )
                            edgeInfo12.Face1 = null;
                        if ( bFaceUpdated[ 2 ] )
                            edgeInfo20.Face1 = null;
                    }
                }
                else
                {
                    faceInfos.Add( faceInfo );
                }

            }
        }


        private bool AlreadyExists( FaceInfo faceInfo, List<FaceInfo> faceInfos )
        {
            for ( var i = 0; i < faceInfos.Count; ++i )
            {
                if ( faceInfos[ i ].V0 == faceInfo.V0 &&
                     faceInfos[ i ].V1 == faceInfo.V1 &&
                     faceInfos[ i ].V2 == faceInfo.V2 )
                    return true;
            }

            return false;
        }
    }
}