using MemoryManager.Lib.Interfaces;
using System;
using System.Collections.Generic;

namespace MemoryManager.Lib
{
    /// <summary>
    /// Main class of the library.
    /// Users use this library to allocate and free memory blocks as needed.
    /// Thread Safety: "FreeBlocksManager" is the only one that needs to be Thread-Safe. Refer to the comment on that class for more details.
    /// </summary>
    public class VirtualMemoryManager : IVirtualMemoryManager
    {
        // .Net array size cannot be more than this value. Though we can configure it to be smaller if needed as per the system 
        // requirements to prevent possible user agnostic errors while creating larger arrays.
        private static int maxSize = Int32.MaxValue;

        private readonly char[] buffer;

        // Ideally these dependencies must be injected (Inversion of Control pattern). 
        // However for this exercise, it's okay to construct them within the constructors.
        private readonly FreeBlocksManager freeBlocksManager;

        public VirtualMemoryManager(int size)
        {
            if (size <= 0 || size > maxSize)
            {
                throw new ArgumentOutOfRangeException($"Size must be ({0}, {maxSize}]. Provided: {size}");
            }

            this.buffer = new char[size];
            this.freeBlocksManager = new FreeBlocksManager(0, size);
            this.AllocatedBlocksCount = 0;
        }

        /// <inheritdoc />
        public int AllocatedBlocksCount { get; private set; }

        /// <inheritdoc />
        public int MemAvailable => this.freeBlocksManager.MemAvailable;

        /// <inheritdoc />
        public IMemBlock Alloc(int size)
        {
            var blocks = new List<RangeBlock>();
            foreach (var block in this.freeBlocksManager.GetFreeBlocks(size))
            {
                blocks.Add(block);
            }

            ++this.AllocatedBlocksCount;
            return new MemBlock(this.buffer, blocks, size, this.freeBlocksManager);
        }

        /// <inheritdoc />
        public void Free(IMemBlock memBlock)
        {
            --this.AllocatedBlocksCount;
            if (this.AllocatedBlocksCount < 0)
            {
                this.AllocatedBlocksCount = 0;
            }

            // Downcasting from interface is considered "Code-Smell" in Engineering. 
            // However, this is a safe operation given the exercise. 
            // A better design is to maintain a dictionary (hash map) of memBlock Ids (start Index can be used) to store the corresponding MemBlock type object reference. It would also mean extra space of 8 bytes per object to store reference.
            var block = memBlock as MemBlock;
            block.Free();
        }
    }
}
