using MemoryManager.Lib;
using MemoryManager.Lib.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Alloc_WriteString_ValidateStringSuccess()
        {
            // Arrange
            var manager = new VirtualMemoryManager(5);
            var dummy = "dummy";

            // Act
            var block = manager.Alloc(5);
            for (int i = 0; i < dummy.Length; ++i)
            {
                block.Write(i, dummy[i]);
            }

            // Assert
            var observed = new StringBuilder();
            for (int i = 0; i < dummy.Length; ++i)
            {
                observed.Append(block.Read(i));
            }

            Assert.IsTrue(dummy.Equals(observed.ToString()));
        }

        [Test]
        public void Alloc_FragmentedBlocksSize2_ShouldSucceed()
        {
            // Arrange
            var manager = new VirtualMemoryManager(5);

            // Act
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

            // Assert
            try
            {
                var expectedSize = 2;
                var observed = manager.Alloc(expectedSize);
                Assert.AreEqual(expectedSize, observed.Size);
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception but got {e}");
            }
        }

        [Test]
        public void Alloc_FragmentedBlocksSize2_ShouldFail()
        {
            // Arrange
            var manager = new VirtualMemoryManager(5);

            // Act
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
                        
            manager.Free(block4);

            // Assert
            Assert.Throws<InvalidOperationException>(() => manager.Alloc(2));
        }

        [Test]
        public void Alloc_AllocFullMemory_FullMemoryAllocated()
        {
            // Arrange
            var capacity = 5;
            var manager = new VirtualMemoryManager(capacity);

            // Act
            var block = manager.Alloc(capacity);

            // Assert
            Assert.AreEqual(capacity, block.Size);
            Assert.AreEqual(0, manager.MemAvailable);
        }

        [Test]
        public void Alloc_AllocFullMemoryAndFreeFullMemory_FullMemoryAvailable()
        {
            // Arrange
            var capacity = 5;
            var manager = new VirtualMemoryManager(capacity);

            // Act
            var block = manager.Alloc(5);
            manager.Free(block);

            // Assert
            Assert.AreEqual(capacity, manager.MemAvailable);
        }

        [Test]
        public void Alloc_RandomBlocksAllocation_AccurateAllocatedBlockCount()
        {
            // Arrange
            var capacity = 100_000;
            var manager = new VirtualMemoryManager(capacity);

            // Act
            // Allocate random blocks
            var rnd = new Random();
            var allocatedBlocksCount = 0;
            while (manager.MemAvailable > 10)
            {
                // Random number in range [1, 9]
                var randomBlockSize = rnd.Next(1, 10);
                manager.Alloc(randomBlockSize);
                ++allocatedBlocksCount;
            }

            // Assert
            Assert.AreEqual(allocatedBlocksCount, manager.AllocatedBlocksCount);
        }

        [Test]
        public void Free_RandomBlocksAllocationAndFree_AccurateAllocatedBlockCount()
        {
            // Arrange
            var capacity = 100_000;
            var manager = new VirtualMemoryManager(capacity);

            // Act
            // Allocate random blocks
            var rnd = new Random();
            var allocatedBlocks = new LinkedList<IMemBlock>();
           
            for (var i = 0; i < 10_000; ++i)
            {
                // Whether to Alloc or free
                if (rnd.NextDouble() < 0.5)
                {
                    // Alloc
                    // Random number in range [1, 9]
                    var randomBlockSize = rnd.Next(1, 10);
                    allocatedBlocks.AddLast(
                        new LinkedListNode<IMemBlock>(manager.Alloc(randomBlockSize)));
                }
                else
                {
                    // Free the first one
                    if (allocatedBlocks.Count != 0)
                    {
                        manager.Free(allocatedBlocks.First.Value);
                        allocatedBlocks.RemoveFirst();
                    }
                }
            }

            // Assert
            Assert.AreEqual(allocatedBlocks.Count, manager.AllocatedBlocksCount);
        }
    }
}