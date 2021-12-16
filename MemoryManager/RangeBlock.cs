using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManager.Lib
{
    /// <summary>
    /// Metadata of a range block, which represents certain [start, end] range of the buffer.
    /// This is primarily used by FreeBlocksManager to store the next available blocks.
    /// This is also returned to the Memroy Manager when Alloc() is called so that the manager can construct MemBlock over the smaller sub blocks
    /// </summary>
    public class RangeBlock
    {  
        public RangeBlock(int start, int size)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException($"start must be >= 0. Provided: {start}");
            }

            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException($"size must be positive. Provided: {size}");
            }

            this.Start = start;
            this.Size = size;
        }

        public int Start { get; }
        public int Size { get; }
    }
}
