using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManager.Lib.Interfaces
{
    public interface IVirtualMemoryManager
    {
        /// <summary>
        /// Count of the current allocated blocks
        /// </summary>
        int AllocatedBlocksCount { get; }

        /// <summary>
        /// Available memory by respective unit (For this excercise, it's the number of slots available in an array)
        /// </summary>
        int MemAvailable { get; }

        /// <summary>
        /// Allocates a block of requested size
        /// </summary>
        /// <param name="size">Requested size</param>
        /// <returns><see cref="IMemBlock"/></returns>
        IMemBlock Alloc(int size);

        /// <summary>
        /// Frees the memory block
        /// </summary>
        /// <param name="memBlock"></param>
        void Free(IMemBlock memBlock);
    }
}
