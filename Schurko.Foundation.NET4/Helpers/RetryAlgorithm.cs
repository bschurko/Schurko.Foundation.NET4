
using System;


#nullable enable
namespace Schurko.Foundation.Utilities
{
    public static class RetryAlgorithm
    {
        private static readonly Random RandomNumber = new Random();

        public static int NthPower(int n, int ceiling = 8) => (int)Math.Pow(2.0, Math.Min(n, ceiling));

        public static int Fibonacci(int n, int ceiling = 15)
        {
            int num1 = 0;
            int num2 = 1;
            for (int index = 0; index < n && index < ceiling; ++index)
            {
                int num3 = num1;
                num1 = num2;
                int num4 = num2;
                num2 = num3 + num4;
            }
            return num1;
        }

        public static int RandomZeroToNth(int n, int ceiling = 8) => RandomNumber.Next(0, NthPower(n, ceiling));

        public static int RandomHalfToNth(int n, int ceiling = 8) => RandomNumber.Next(NthPower(n - 1, ceiling), NthPower(n, ceiling));

        public static int RandomFibonacci(int n, int ceiling = 15) => RandomNumber.Next(Fibonacci(n - 1, ceiling), Fibonacci(n, ceiling));
    }
}
