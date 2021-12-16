using MemoryManager.Lib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManager.Lib
{
    /// <summary>
    /// Actual Memory Block the users of the library can use to read/write characters. 
    /// The MemBlock can encapsulate several smaller blocks
    /// Until the block is disposed/freed, the block belongs to the caller.
    /// Once disposed/freed, the blocks are released back to the Free Memory Manager.
    /// </summary>
    public class MemBlock : IMemBlock, IDisposable
    {
        private readonly char[] buffer;
        private readonly IList<RangeBlock> blocks;
        private readonly IBlocksReleaser blocksReleaser;

        public MemBlock(char[] buffer, IList<RangeBlock> blocks, int size, IBlocksReleaser blocksReleaser)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException($"size must be positive");
            }
            this.Size = size;

            this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            this.blocks = blocks ?? throw new ArgumentNullException(nameof(blocks));
            this.blocksReleaser = blocksReleaser ?? throw new ArgumentNullException(nameof(blocksReleaser));
        }

        /// <inheritdoc />
        public int Size { get; }

        /// <inheritdoc />
        public char Read(int ind)
        {
            this.ValidateIndex(ind);

            // Find the block where the requested index belongs
            foreach (var block in blocks)
            {
                if (ind < block.Size)
                {
                    // Calculate the offset index and return the character
                    return buffer[block.Start + ind];
                }

                ind -= block.Size;
            }

            // This exception should never be hit as long as the provided index is within range
            throw new ArgumentOutOfRangeException($"{ind} index is out of range");
        }

        /// <inheritdoc />
        public void Write(int ind, char ch)
        {
            this.ValidateIndex(ind);

            // Find the block where the character belongs
            foreach (var block in blocks)
            {
                if (ind < block.Size)
                {
                    // Calculate the offset index and write the character
                    buffer[block.Start + ind] = ch;
                }

                ind -= block.Size;
            }
        }

        // It's better to have both Free() as well as Dipose() methods.
        // Dispose() is auto invoked by .net runtime if the user encapsulates the block usage in "using(var block = **)" syntax.
        public void Free()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            // Return the blocks to be added to the free blocks
            this.blocksReleaser.ReturnFreeBlocks(this.blocks);
        }

        private void ValidateIndex(int ind)
        {
            if (ind < 0 || ind >= this.Size)
            {
                throw new ArgumentOutOfRangeException($"Ind must be in [{0}, {this.Size - 1}]. Provided: {ind}");
            }
        }
    }
}
