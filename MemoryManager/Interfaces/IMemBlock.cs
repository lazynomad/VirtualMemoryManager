using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManager.Lib.Interfaces
{
    public interface IMemBlock
    {
        /// <summary>
        /// Size of the block
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Reads the character at index
        /// </summary>
        /// <param name="ind">Index</param>
        /// <returns>Characted</returns>
        char Read(int ind);

        /// <summary>
        /// Writes the character to index
        /// </summary>
        /// <param name="ind">Index</param>
        /// <param name="ch">Character to write</param>
        void Write(int ind, char ch);

        // To-Do: Below can be implemented as extended features
        // void WriteRange(int start, IReadOnlyList<Char> chars);
        // IEnumerable<Char> ReadRange(int start, int count);
    }
}
