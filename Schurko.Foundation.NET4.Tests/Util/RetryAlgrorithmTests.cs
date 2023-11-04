using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schurko.Foundation.Utilities;

namespace Schurko.Foundation.Tests.Util
{
    [TestClass]
    public class RetryAlgrorithmTests
    {
        [TestMethod]
        public void NthPowerTests()
        {
            Assert.AreEqual(1, RetryAlgorithm.NthPower(0));
            Assert.AreEqual(2, RetryAlgorithm.NthPower(1));
            Assert.AreEqual(4, RetryAlgorithm.NthPower(2));
            Assert.AreEqual(8, RetryAlgorithm.NthPower(3));
            Assert.AreEqual(256, RetryAlgorithm.NthPower(8));
            Assert.AreEqual(256, RetryAlgorithm.NthPower(10));
            Assert.AreEqual(1024, RetryAlgorithm.NthPower(10, 10));
        }

        [TestMethod]
        public void RandomHalfToNthTests()
        {
            for (int i = 0; i < 1000; i++)
            {
                var value = RetryAlgorithm.RandomHalfToNth(i + 8);
                Debug.Write(value + ";");
                Assert.IsTrue(value >= 128);
                Assert.IsTrue(value <= 256);
            }
        }

        [TestMethod]
        public void RandomZeroToNthTests()
        {
            for (int i = 0; i < 1000; i++)
            {
                var value = RetryAlgorithm.RandomZeroToNth(i + 8);
                Debug.Write(value + ";");
                Assert.IsTrue(value >= 0);
                Assert.IsTrue(value <= 256);
            }
        }

        [TestMethod]
        public void FibonacciTests()
        {
            Assert.AreEqual(0, RetryAlgorithm.Fibonacci(0));
            Assert.AreEqual(1, RetryAlgorithm.Fibonacci(1));
            Assert.AreEqual(1, RetryAlgorithm.Fibonacci(2));
            Assert.AreEqual(2, RetryAlgorithm.Fibonacci(3));
            Assert.AreEqual(3, RetryAlgorithm.Fibonacci(4));
            Assert.AreEqual(5, RetryAlgorithm.Fibonacci(5));
            Assert.AreEqual(8, RetryAlgorithm.Fibonacci(6));
            Assert.AreEqual(13, RetryAlgorithm.Fibonacci(7));
            Assert.AreEqual(21, RetryAlgorithm.Fibonacci(8));
            Assert.AreEqual(34, RetryAlgorithm.Fibonacci(9));
            Assert.AreEqual(55, RetryAlgorithm.Fibonacci(10));
            Assert.AreEqual(89, RetryAlgorithm.Fibonacci(11));
            Assert.AreEqual(144, RetryAlgorithm.Fibonacci(12));
            Assert.AreEqual(233, RetryAlgorithm.Fibonacci(13));
            Assert.AreEqual(377, RetryAlgorithm.Fibonacci(14));
            Assert.AreEqual(610, RetryAlgorithm.Fibonacci(15));
            Assert.AreEqual(610, RetryAlgorithm.Fibonacci(16));
            Assert.AreEqual(987, RetryAlgorithm.Fibonacci(16, 20));
        }

        [TestMethod]
        public void RandomFibonacciTests()
        {
            for (int i = 0; i < 100; i++)
            {
                var value = RetryAlgorithm.RandomFibonacci(i, 13);
                Debug.Write(value + ";");
                Assert.IsTrue(value >= 0);
                Assert.IsTrue(value <= 256);
            }
        }
    }
}