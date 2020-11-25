using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikuMikuLibrary.Objects.Extra;
using MikuMikuLibrary.Objects.Extra.Blocks;

namespace MikuMikuLibrary.Extensions
{
    public static class BlockEx
    {
        public static IEnumerable<NodeBlock> TraverseParents( this NodeBlock nodeBlock, IEnumerable<NodeBlock> nodeBlocks )
        {
            var currentBlock = nodeBlock;

            while ( ( currentBlock = nodeBlocks.FirstOrDefault( x => x.Name == currentBlock.ParentName ) ) != null )
                yield return currentBlock;
        }
    }
}