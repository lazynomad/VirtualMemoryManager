using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManager.Lib.Interfaces
{
    public interface IBlocksReleaser
    {
        /// <summary>
        /// Releases the free blocks
        /// </summary>
        /// <param name="blocks"></param>
        void ReturnFreeBlocks(IEnumerable<RangeBlock> blocks);
    }
}
