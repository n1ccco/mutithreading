using System;
using System.Diagnostics;
using System.Threading;

class ParallelMergeSort
{
    static void Main()
    {
        int[] data = GenerateLargeData(100000000); 
        Console.WriteLine("Unsorted: " + data[0] + ", ..., " + data[data.Length - 1]);

        // Однопотокова сортування
        Stopwatch stopwatch = Stopwatch.StartNew();
        MergeSorter.Sort(data);
        stopwatch.Stop();
        Console.WriteLine("Sorted: " + data[0] + ", ..., " + data[data.Length - 1]);
        Console.WriteLine("Single-threaded Execution Time: " + stopwatch.ElapsedMilliseconds + " ms");

        // Багатопотокова сортування
        int[] parallelData = GenerateLargeData(100000000);
        Console.WriteLine("Unsorted (Parallel): " + parallelData[0] + ", ..., " + parallelData[parallelData.Length - 1]);

        stopwatch.Restart();
        MergeSorterParallel.Sort(parallelData, 8);
        stopwatch.Stop();
        Console.WriteLine("Sorted (Parallel): " + parallelData[0] + ", ..., " + parallelData[parallelData.Length - 1]);
        Console.WriteLine("Multi-threaded Execution Time: " + stopwatch.ElapsedMilliseconds + " ms");
    }

    static int[] GenerateLargeData(int size)
    {
        Random random = new Random();
        int[] data = new int[size];

        for (int i = 0; i < size; i++)
        {
            data[i] = random.Next(1000000);
        }

        return data;
    }

    public static class MergeSorter
    {
        public static void Sort<T>(T[] array) where T : IComparable<T>
        {
            MergeSortRecursive(array, 0, array.Length - 1);
        }

        private static void MergeSortRecursive<T>(T[] array, int left, int right) where T : IComparable<T>
        {
            if (left < right)
            {
                int middle = (left + right) / 2;

                MergeSortRecursive(array, left, middle);
                MergeSortRecursive(array, middle + 1, right);

                Merge(array, left, middle, right);
            }
        }

        private static void Merge<T>(T[] array, int left, int middle, int right) where T : IComparable<T>
        {
            int n1 = middle - left + 1;
            int n2 = right - middle;

            T[] leftArray = new T[n1];
            T[] rightArray = new T[n2];

            Array.Copy(array, left, leftArray, 0, n1);
            Array.Copy(array, middle + 1, rightArray, 0, n2);

            int i = 0, j = 0, k = left;

            while (i < n1 && j < n2)
            {
                if (leftArray[i].CompareTo(rightArray[j]) <= 0)
                {
                    array[k++] = leftArray[i++];
                }
                else
                {
                    array[k++] = rightArray[j++];
                }
            }

            while (i < n1)
            {
                array[k++] = leftArray[i++];
            }

            while (j < n2)
            {
                array[k++] = rightArray[j++];
            }
        }
    }


    public static class MergeSorterParallel
    {
        static int depth = 0;
        public static void Sort<T>(T[] array, int threads) where T : IComparable<T>
        {
            depth = threads;
            MergeSortRecursive(array, 0, array.Length - 1);
        }

        private static void MergeSortRecursive<T>(T[] array, int left, int right) where T : IComparable<T>
        {
            if (left < right)
            {
                int middle = (left + right) / 2;

                if (depth-2 >= 0) {
                    depth -= 2;
                    Thread leftThread = new Thread(() => MergeSortRecursive(array, left, middle));
                    Thread rightThread = new Thread(() => MergeSortRecursive(array, middle + 1, right));
                    leftThread.Start();
                    rightThread.Start();
                    leftThread.Join();
                    rightThread.Join();

                }
                else
                {
                    MergeSortRecursive(array, left, middle);
                    MergeSortRecursive(array, middle + 1, right);
                }
                Merge(array, left, middle, right);
            }
        }

        private static void Merge<T>(T[] array, int left, int middle, int right) where T : IComparable<T>
        {
            int n1 = middle - left + 1;
            int n2 = right - middle;

            T[] leftArray = new T[n1];
            T[] rightArray = new T[n2];

            Array.Copy(array, left, leftArray, 0, n1);
            Array.Copy(array, middle + 1, rightArray, 0, n2);

            int i = 0, j = 0, k = left;

            while (i < n1 && j < n2)
                if (leftArray[i].CompareTo(rightArray[j]) <= 0)
                    array[k++] = leftArray[i++];
                else
                    array[k++] = rightArray[j++];

            while (i < n1)
                array[k++] = leftArray[i++];

            while (j < n2)
                array[k++] = rightArray[j++];
        }
    }

   
}


