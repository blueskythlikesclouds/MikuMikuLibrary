//========================================================//
// Taken from: https://github.com/TGEnigma/NvTriStrip.Net //
//========================================================//

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NvTriStripDotNet
{
    /// <summary>
    /// Public interface for NvTriStrip stripifier.
    /// </summary>
    internal class NvStripifier
    {
        /// <summary>
        /// GeForce1 and 2 cache size
        /// </summary>
        public const int CACHESIZE_GEFORCE1_2 = 16;

        /// <summary>
        /// GeForce3 cache size
        /// </summary>
        public const int CACHESIZE_GEFORCE3 = 24;

        /// <summary>
        /// PS3 RSX cache size.
        /// </summary>
        public const int CACHESIZE_RSX = 63;

        /// <summary>
        /// For GPUs that support primitive restart, enabled restarting the strip using the specified restart value.
        /// Restart is meaningless if strips are not being stitched together, so enabling restart
        /// makes NvTriStrip forcing stitching. So, you'll get back one strip.
        /// </summary>
        /// <remarks>
        /// Default value: false.
        /// </remarks>
        public bool UseRestart { get; set; } = false;

        /// <summary>
        /// For GPUs that support primitive restart, this sets a value as the restart index.
        /// Only applicable when primitive restart is enabled.
        /// </summary>
        /// <remarks>
        /// Default value: -1 (0xFFFF)
        /// </remarks>
        public int RestartValue { get; set; } = -1;

        /// <summary>
        /// Sets the cache size which the stripfier uses to optimize the data.
        /// Controls the length of the generated individual strips.
        /// This is the "actual" cache size, so 24 for GeForce3 and 16 for GeForce1/2
        /// You may want to play around with this number to tweak performance.
        /// </summary>
        /// <remarks>
        /// Default value: 16.
        /// </remarks>
        public int CacheSize { get; set; } = CACHESIZE_GEFORCE1_2;

        /// <summary>
        /// bool to indicate whether to stitch together strips into one huge strip or not.
        /// If set to true, you'll get back one huge strip stitched together using degenerate
        /// triangles.
        /// If set to false, you'll get back a large number of separate strips.
        /// </summary>
        /// <remarks>
        /// Default value: true.
        /// </remarks>
        public bool StitchStrips { get; set; } = true;

        /// <summary>
        /// Sets the minimum acceptable size for a strip, in triangles.
        /// All strips generated which are shorter than this will be thrown into one big, separate list.
        /// </summary>
        /// <remarks>
        /// Default value: 0.
        /// </remarks>
        public int MinStripSize { get; set; } = 0;

        /// <summary>
        /// If set to true, will return an optimized list, with no strips at all.
        /// </summary>
        /// <remarks>
        /// Default value: false.
        /// </remarks>
        public bool ListsOnly { get; set; } = false;

        /// <summary>
        /// Generates strips from the given input indices.
        /// </summary>
        /// <param name="indices">Input index list, the indices you would use to render.</param>
        /// <param name="primitiveGroups">Array of optimized/stripified PrimitiveGroups</param>
        /// <param name="validateEnabled">Whether to validate the output or not.</param>
        /// <returns>A boolean indicating whether the operation completed successfully.</returns>
        public bool GenerateStrips( ushort[] indices, out PrimitiveGroup[] primitiveGroups, bool validateEnabled = false )
        {
            var numGroups = 0;

            //put data in format that the stripifier likes
            var tempIndices = new List<ushort>( indices.Length );
            ushort maxIndex = 0;
            ushort minIndex = 0xFFFF;
            for ( var i = 0; i < indices.Length; i++ )
            {
                tempIndices.Add( indices[ i ] );
                if ( indices[ i ] > maxIndex )
                    maxIndex = indices[ i ];
                if ( indices[ i ] < minIndex )
                    minIndex = indices[ i ];
            }

            var tempStrips = new List<StripInfo>();
            var tempFaces = new List<FaceInfo>();

            var stripifier = new Stripifier();

            //do actual stripification
            stripifier.Stripify( tempIndices, CacheSize, MinStripSize, maxIndex, tempStrips, tempFaces );

            //stitch strips together
            var stripIndices = new List<int>();
            uint numSeparateStrips = 0;

            if ( ListsOnly )
            {
                //if we're outputting only lists, we're done
                numGroups = 1;
                primitiveGroups = new PrimitiveGroup[ numGroups ];
                var primGroupArray = primitiveGroups;

                //count the total number of indices
                uint numIndices = 0;
                for ( var i = 0; i < tempStrips.Count; i++ )
                {
                    numIndices += ( uint )( tempStrips[ i ].Faces.Count * 3 );
                }

                //add in the list
                numIndices += ( uint )( tempFaces.Count * 3 );
                primGroupArray[ 0 ] = new PrimitiveGroup( PrimitiveType.TriangleList, new ushort[ numIndices ] );

                //do strips
                uint indexCtr = 0;
                for ( var i = 0; i < tempStrips.Count; i++ )
                {
                    for ( var j = 0; j < tempStrips[ i ].Faces.Count; j++ )
                    {
                        //degenerates are of no use with lists
                        if ( !Stripifier.IsDegenerate( tempStrips[ i ].Faces[ j ] ) )
                        {
                            primGroupArray[ 0 ].Indices[ indexCtr++ ] = ( ushort )tempStrips[ i ].Faces[ j ].V0;
                            primGroupArray[ 0 ].Indices[ indexCtr++ ] = ( ushort )tempStrips[ i ].Faces[ j ].V1;
                            primGroupArray[ 0 ].Indices[ indexCtr++ ] = ( ushort )tempStrips[ i ].Faces[ j ].V2;
                        }
                        else
                        {
                            //we've removed a tri, reduce the number of indices
                            var resizedIndices = primGroupArray[ 0 ].Indices;
                            Array.Resize( ref resizedIndices, primGroupArray[ 0 ].Indices.Length - 3 );
                            primGroupArray[ 0 ].Indices = resizedIndices;
                        }
                    }
                }

                //do lists
                for ( var i = 0; i < tempFaces.Count; i++ )
                {
                    primGroupArray[ 0 ].Indices[ indexCtr++ ] = ( ushort )tempFaces[ i ].V0;
                    primGroupArray[ 0 ].Indices[ indexCtr++ ] = ( ushort )tempFaces[ i ].V1;
                    primGroupArray[ 0 ].Indices[ indexCtr++ ] = ( ushort )tempFaces[ i ].V2;
                }
            }
            else
            {
                stripifier.CreateStrips( tempStrips, stripIndices, StitchStrips, ref numSeparateStrips, UseRestart, ( uint )RestartValue );

                //if we're stitching strips together, we better get back only one strip from CreateStrips()
                Debug.Assert( StitchStrips && numSeparateStrips == 1 || !StitchStrips );

                //convert to output format
                numGroups = ( ushort )numSeparateStrips; //for the strips
                if ( tempFaces.Count != 0 )
                    numGroups++; //we've got a list as well, increment
                primitiveGroups = new PrimitiveGroup[ numGroups ];

                var primGroupArray = primitiveGroups;

                //first, the strips
                var startingLoc = 0;
                for ( var stripCtr = 0; stripCtr < numSeparateStrips; stripCtr++ )
                {
                    var stripLength = 0;

                    if ( !StitchStrips )
                    {
                        //if we've got multiple strips, we need to figure out the correct length
                        int i;
                        for ( i = startingLoc; i < stripIndices.Count; i++ )
                        {
                            if ( stripIndices[ i ] == -1 )
                                break;
                        }

                        stripLength = i - startingLoc;
                    }
                    else
                        stripLength = stripIndices.Count;

                    primGroupArray[ stripCtr ] = new PrimitiveGroup( PrimitiveType.TriangleStrip, new ushort[ stripLength ] );

                    var indexCtr = 0;
                    for ( int i = startingLoc; i < stripLength + startingLoc; i++ )
                        primGroupArray[ stripCtr ].Indices[ indexCtr++ ] = ( ushort )stripIndices[ i ];

                    //we add 1 to account for the -1 separating strips
                    //this doesn't break the stitched case since we'll exit the loop
                    startingLoc += stripLength + 1;
                }

                //next, the list
                if ( tempFaces.Count != 0 )
                {
                    int faceGroupLoc = numGroups - 1; //the face group is the last one
                    primGroupArray[ faceGroupLoc ] = new PrimitiveGroup( PrimitiveType.TriangleList, new ushort[ tempFaces.Count * 3 ] );
                    var indexCtr = 0;
                    for ( var i = 0; i < tempFaces.Count; i++ )
                    {
                        primGroupArray[ faceGroupLoc ].Indices[ indexCtr++ ] = ( ushort )tempFaces[ i ].V0;
                        primGroupArray[ faceGroupLoc ].Indices[ indexCtr++ ] = ( ushort )tempFaces[ i ].V1;
                        primGroupArray[ faceGroupLoc ].Indices[ indexCtr++ ] = ( ushort )tempFaces[ i ].V2;
                    }
                }
            }

            //validate generated data against input
            if ( validateEnabled )
            {
                var numbins = 100;

                var inBins = new List<FaceInfo>[ numbins ];
                for ( var i = 0; i < inBins.Length; ++i )
                    inBins[ i ] = new List<FaceInfo>();

                //hash input indices on first index
                for ( var i = 0; i < indices.Length; i += 3 )
                {
                    var faceInfo = new FaceInfo( indices[ i ], indices[ i + 1 ], indices[ i + 2 ] );
                    inBins[ indices[ i ] % numbins ].Add( faceInfo );
                }

                for ( var i = 0; i < numGroups; ++i )
                {
                    switch ( primitiveGroups[ i ].Type )
                    {
                        case PrimitiveType.TriangleList:
                            {
                                for ( var j = 0; j < primitiveGroups[ i ].Indices.Length; j += 3 )
                                {
                                    ushort v0 = primitiveGroups[ i ].Indices[ j ];
                                    ushort v1 = primitiveGroups[ i ].Indices[ j + 1 ];
                                    ushort v2 = primitiveGroups[ i ].Indices[ j + 2 ];

                                    //ignore degenerates
                                    if ( Stripifier.IsDegenerate( v0, v1, v2 ) )
                                        continue;

                                    if ( !TestTriangle( v0, v1, v2, inBins, numbins ) )
                                    {
                                        Cleanup( tempStrips, tempFaces );
                                        return false;
                                    }
                                }

                                break;
                            }

                        case PrimitiveType.TriangleStrip:
                            {
                                var flip = false;
                                for ( var j = 2; j < primitiveGroups[ i ].Indices.Length; ++j )
                                {
                                    ushort v0 = primitiveGroups[ i ].Indices[ j - 2 ];
                                    ushort v1 = primitiveGroups[ i ].Indices[ j - 1 ];
                                    ushort v2 = primitiveGroups[ i ].Indices[ j ];

                                    if ( flip )
                                    {
                                        //swap v1 and v2
                                        ushort swap = v1;
                                        v1 = v2;
                                        v2 = swap;
                                    }

                                    //ignore degenerates
                                    if ( Stripifier.IsDegenerate( v0, v1, v2 ) )
                                    {
                                        flip = !flip;
                                        continue;
                                    }

                                    if ( !TestTriangle( v0, v1, v2, inBins, numbins ) )
                                    {
                                        Cleanup( tempStrips, tempFaces );
                                        return false;
                                    }

                                    flip = !flip;
                                }

                                break;
                            }

                        case PrimitiveType.TriangleFan:
                        default:
                            break;
                    }
                }

            }

            //clean up everything
            Cleanup( tempStrips, tempFaces );

            return true;
        }

        /// <summary>
        /// Function to remap your indices to improve spatial locality in your vertex buffer.
        /// </summary>
        /// <remarks>
        /// Note that, according to the remapping handed back to you, you must reorder your 
        /// vertex buffer.
        /// </remarks>
        /// <param name="primitiveGroups">Array of PrimitiveGroups you want remapped.</param>
        /// <param name="vertexCount">Number of vertices in your vertex buffer, also can be thought of as the range of acceptable values for indices in your primitive groups.</param>
        /// <param name="remappedPrimitiveGroups">Array of remapped PrimitiveGroups.</param>
        public void RemapIndices( PrimitiveGroup[] primitiveGroups, ushort vertexCount, out PrimitiveGroup[] remappedPrimitiveGroups )
        {
            remappedPrimitiveGroups = new PrimitiveGroup[ primitiveGroups.Length ];

            //caches oldIndex --> newIndex conversion
            var indexCache = new int[ vertexCount ];
            for ( var i = 0; i < indexCache.Length; ++i )
                indexCache[ i ] = -1;

            //loop over primitive groups
            uint indexCtr = 0;
            for ( var i = 0; i < primitiveGroups.Length; i++ )
            {
                int numIndices = primitiveGroups[ i ].Indices.Length;

                //init remapped group
                remappedPrimitiveGroups[ i ] = new PrimitiveGroup( primitiveGroups[ i ].Type, new ushort[ numIndices ] );

                for ( var j = 0; j < numIndices; j++ )
                {
                    int cachedIndex = indexCache[ primitiveGroups[ i ].Indices[ j ] ];
                    if ( cachedIndex == -1 ) //we haven't seen this index before
                    {
                        //point to "last" vertex in VB
                        remappedPrimitiveGroups[ i ].Indices[ j ] = ( ushort )indexCtr;

                        //add to index cache, increment
                        indexCache[ primitiveGroups[ i ].Indices[ j ] ] = ( ushort )indexCtr++;
                    }
                    else
                    {
                        //we've seen this index before
                        remappedPrimitiveGroups[ i ].Indices[ j ] = ( ushort )cachedIndex;
                    }
                }
            }
        }

        /// <summary>
        /// Cleanup strips / faces, used by generatestrips.
        /// </summary>
        private static void Cleanup( List<StripInfo> tempStrips, List<FaceInfo> tempFaces )
        {
            //delete strips
            for ( var i = 0; i < tempStrips.Count; i++ )
            {
                for ( var j = 0; j < tempStrips[ i ].Faces.Count; j++ )
                {
                    tempStrips[ i ].Faces[ j ] = null;
                }

                tempStrips[ i ].Faces.Clear();
                tempStrips[ i ] = null;
            }

            //delete faces
            for ( var i = 0; i < tempFaces.Count; i++ )
            {
                tempFaces[ i ] = null;
            }
        }

        /// <summary>
        /// Returns true if the two triangles defined by firstTri and secondTri are the same
        /// The "same" is defined in this case as having the same indices with the same winding order
        /// </summary>
        private static bool SameTriangle( ushort firstTri0, ushort firstTri1, ushort firstTri2,
                           ushort secondTri0, ushort secondTri1, ushort secondTri2 )
        {
            var isSame = false;

            if ( firstTri0 == secondTri0 )
            {
                if ( firstTri1 == secondTri1 )
                {
                    if ( firstTri2 == secondTri2 )
                        isSame = true;
                }
            }
            else if ( firstTri0 == secondTri1 )
            {
                if ( firstTri1 == secondTri2 )
                {
                    if ( firstTri2 == secondTri0 )
                        isSame = true;
                }
            }
            else if ( firstTri0 == secondTri2 )
            {
                if ( firstTri1 == secondTri0 )
                {
                    if ( firstTri2 == secondTri1 )
                        isSame = true;
                }
            }

            return isSame;
        }

        private static bool TestTriangle( ushort v0, ushort v1, ushort v2, List<FaceInfo>[] inBins, int numbins )
        {
            //hash this triangle
            var isLegit = false;
            int ctr = v0 % numbins;
            for ( var k = 0; k < inBins[ ctr ].Count; ++k )
            {
                //check triangles in this bin
                if ( SameTriangle( ( ushort )inBins[ ctr ][ k ].V0, ( ushort )inBins[ ctr ][ k ].V1, ( ushort )inBins[ ctr ][ k ].V2,
                                   v0, v1, v2 ) )
                {
                    isLegit = true;
                    break;
                }
            }

            if ( !isLegit )
            {
                ctr = v1 % numbins;
                for ( var k = 0; k < inBins[ ctr ].Count; ++k )
                {
                    //check triangles in this bin
                    if ( SameTriangle( ( ushort )inBins[ ctr ][ k ].V0, ( ushort )inBins[ ctr ][ k ].V1, ( ushort )inBins[ ctr ][ k ].V2,
                                       v0, v1, v2 ) )
                    {
                        isLegit = true;
                        break;
                    }
                }

                if ( !isLegit )
                {
                    ctr = v2 % numbins;
                    for ( var k = 0; k < inBins[ ctr ].Count; ++k )
                    {
                        //check triangles in this bin
                        if ( SameTriangle( ( ushort )inBins[ ctr ][ k ].V0, ( ushort )inBins[ ctr ][ k ].V1, ( ushort )inBins[ ctr ][ k ].V2,
                                           v0, v1, v2 ) )
                        {
                            isLegit = true;
                            break;
                        }
                    }

                }
            }

            return isLegit;
        }
    }
}