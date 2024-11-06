using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Otus.Multithreading.Vasileva
{
    internal class SumCalculation
    {
        //Обычное вычисление суммы элементов массива
        public static int SumArraySequential(int[] arr)
        {
            int sum = 0;
            foreach (var num in arr)
            {
                sum += num;
            }
            return sum;
        }

        //Параллельное вычисление суммы с использованием потоков (Thread)
        public static long SumArrayParallel(int[] arr, int numThreads)
        {
            int length = arr.Length;
            int chunkSize = length / numThreads;
            int remainder = length % numThreads;

            long[] partialSums = new long[numThreads];
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < numThreads; i++)
            {
                int start = i * chunkSize;
                int end = (i == numThreads - 1) ? length : (i + 1) * chunkSize;

                if (i == numThreads - 1)
                    end += remainder;

                // Локальная переменная threadIndex для каждого потока
                int threadIndex = i;

                var thread = new Thread(() =>
                {
                    long sum = 0;
                    for (int j = start; j < end; j++)
                    {
                        sum += arr[j];
                    }
                    partialSums[threadIndex] = sum;
                });
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return partialSums.Sum();
        }

        // Параллельное вычисление суммы с использованием LINQ
        public static long SumArrayLinq(int[] arr)
        {
            return arr.AsParallel()
                .Select(x => (long)x)  // Преобразуем каждый элемент в long
                .Sum();  // Суммируем значения типа long
        }
    }

   
}
