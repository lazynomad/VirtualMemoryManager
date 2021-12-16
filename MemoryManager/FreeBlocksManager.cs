using MemoryManager.Lib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManager.Lib
{
    /// <summary>
    /// Maintains the free blocks available. When Alloc() is called by the manager, the respective free blocks are returned. When Free() is called by the manager, the respective blocks are returned back to this data structure.
    /// Thread Safety design: (Any one of the below is sufficient)
    ///      1) Naive approach is to place a lock() around "freeBlocks access". However, this is in-efficient as only one Thread can any point in time be accessing free blocks. 
    ///      2) Lock the LinkedListNode with double-locking design pattern. Multiple threads can access and manipulate different LinkedListNodes. Locks ensure at most one Thread accesses a particular Node.
    ///      3) ConcurrentQueue data strcuture is a powerful dotnet library which ensures threadsafety and atomicity. However, we cannot use efficient data structures such as Heap unless we write an efficient and thread-safe custom implementation.
    /// </summary>
    public class FreeBlocksManager : IBlocksFetcher, IBlocksReleaser
    {
        // To-Do: Potential to make this a Min Heap by "size" so that we always retrieve the next closest freesize block if available.
        // An alternative is to use the MaxHeap (instead of the min heap) and perform a Greedy appraoch of allocating first Max block 
        // and continues.
        // Another option is to maintain heap over the "Start" attribute. We can then write a "Merge" algorithm to merge the consecutive free blocks.
        // Ex: [0, 5] and [5, 3] can be merged as [0, 8].
        private LinkedList<RangeBlock> freeBlocks;
        private int freeMemAvailable;

        public FreeBlocksManager(int start, int size)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException($"start must be >= 0. Provided: {start}");
            }

            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException($"size must be positive. Provided: {size}");
            }

            this.freeBlocks = new LinkedList<RangeBlock>();

            // Design choice: AddLast() is preferred over AddFirst() to provide fairness of memory blocks allocation.
            // Otherwise, some blocks gets overwritten more often than others. Generally this is bad for SolidState drives, which is not a problem for this project.
            this.freeBlocks.AddLast(
                new LinkedListNode<RangeBlock>(
                    new RangeBlock(start, size)));

            this.freeMemAvailable = size;
        }

        /// <inheritdoc />
        public int MemAvailable => this.freeMemAvailable;

        /// <inheritdoc />
        public IEnumerable<RangeBlock> GetFreeBlocks(int size)
        {
            // Quickly check if there's enough free memory.
            if (size > this.freeMemAvailable)
            {
                throw new InvalidOperationException($"Requested size {size} is not available");
            }

            // We don't need to check if this.freeBlocks.Count == 0 because the above if() check would have thrown exception
            // due to this.freeMemAvailable being "zero".
            LinkedListNode<RangeBlock> node;
            var anomaly = false;
            do
            {
                node = this.freeBlocks.First;
                if (node == null)
                {
                    // Indicates there are no free blocks. This happens if the freeMemAvailable property wasn't updated due to some sort of bug (could potentially happen if the code is not thread-safe)
                    anomaly = true;
                    break;
                }

                this.freeBlocks.RemoveFirst();

                RangeBlock blockToYield;
                if (size >= node.Value.Size)
                {
                    // The entire current node range must be yielded to the caller to be utilized
                    blockToYield = node.Value;
                }
                else
                {
                    // Only a part of this free block needs to be utilized. 
                    // Split this block and yield the sub free block.
                    blockToYield = new RangeBlock(node.Value.Start, size);

                    // To-Do: Using Object Pooling pattern to reuse nodes could significantly reduce the Garbage Collection overhead
                    this.freeBlocks.AddLast(
                        new LinkedListNode<RangeBlock>(
                            new RangeBlock(node.Value.Start + size, node.Value.Size - size)));
                }

                this.freeMemAvailable -= blockToYield.Size;
                size -= blockToYield.Size;

                yield return blockToYield;
            }
            while (size > 0);

            if (anomaly)
            {
                throw new Exception($"Critical error. No free nodes found in the LinkedList. This error is an anomaly and indicates a critical bug in the library, which typically could happen in a multi-threaded environment if this is not thread-safe");
            }
        }

        /// <inheritdoc />
        public void ReturnFreeBlocks(IEnumerable<RangeBlock> blocks)
        {
            foreach (var block in blocks)
            {
                // To-Do: Two possible optimizations
                //      1) Merge on the fly for consecutive free blocks
                //      2) If the caller reuses the FreeBlock object reference even though it's returned, potential for Garbage data overwriting. 
                //         Possible solution is to re-construct new FreeBlock objects. However, the trade off is Garbage Collection overhead of freeing the returned object references from Heap.
                this.freeBlocks.AddLast(
                    new LinkedListNode<RangeBlock>(block));

                // Updating this is very important. Otherwise could throw premature exceptions during GetFreeBlocks()
                this.freeMemAvailable += block.Size;
            }
        }
    }
}
