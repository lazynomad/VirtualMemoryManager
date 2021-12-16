using MemoryManager.Lib;
using MemoryManager.Lib.Interfaces;
using System;

namespace Main
{
    /// <summary>
    /// Console application. 
    /// You can either run tests directly or run this console application.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            IVirtualMemoryManager manager = new VirtualMemoryManager(5);

            var block1 = manager.Alloc(1);
            var block2 = manager.Alloc(1);
            var block3 = manager.Alloc(1);
            var block4 = manager.Alloc(1);
            var block5 = manager.Alloc(1);

            block1.Write(0, 'a');
            block2.Write(0, 'b');
            block3.Write(0, 'c');
            block4.Write(0, 'd');
            block5.Write(0, 'e');

            manager.Free(block2);
            manager.Free(block4);

            var observed = manager.Alloc(2);
            Console.WriteLine($"Allocated size {observed} successfully");
            Console.WriteLine("Enter any key to exit...");
            Console.ReadKey();
        }
    }
}
