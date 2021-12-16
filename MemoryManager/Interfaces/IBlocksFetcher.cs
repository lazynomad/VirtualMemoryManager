using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManager.Lib.Interfaces
{
    public interface IBlocksFetcher
    {
        /// <summary>
        /// Fetches the free blocks
        /// </summary>
        /// <param name="size">Requested size</param>
        /// <returns>Enumerable of <see cref="RangeBlock"/></returns>
        IEnumerable<RangeBlock> GetFreeBlocks(int size);
    }
}
