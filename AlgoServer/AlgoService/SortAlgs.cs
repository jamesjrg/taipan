using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoService
{
    class SortAlgs
    {
        //Insertion sort
        public static void InsertionSort(int[] data)
        {                        
            for (int j = 1; j < data.Length; ++j)
            {
                var key = data[j];
                var i = j - 1;

                while (i >= 0 && data[i] > key)
                {
                    data[i + 1] = data[i];
                    i -= 1;
                }
                data[i + 1] = key;
            }
        }

        //Merge sort
        public static void MergeSort(int[] data)
        {
            MergeSort(data, 0, data.Length - 1);
        }

        private static void MergeSort(int[] data, int p, int r)
        {
            if (p < r)
            {
                //floor
                int q = (p + r) / 2;
                MergeSort(data, p, q);
                MergeSort(data, q + 1, r);
                Merge(data, p, q, r);
            }
        }

        private static void Merge(int[] data, int p, int q, int r)
        {
            int n1 = q - p + 1;
            int n2 = r - q;

            int[] left = new int[n1 + 1];
            int[] right = new int[n2 + 1];

            for (int i = 0; i != n1; ++i)
                left[i] = data[p + i];

            for (int i = 0; i != n2; ++i)
                right[i] = data[q + i + 1];

            //max signed int means sentinel
            left[n1] = 2147483647;
            right[n2] = 2147483647;

            int a = 0;
            int b = 0;

            for (int i = p; i != r + 1; ++i)
            {
                if (left[a] <= right[b])
                {
                    data[i] = left[a];
                    a += 1;
                }
                else
                {
                    data[i] = right[b];
                    b += 1;
                }
            }
        }

        //Heap sort
        public static void HeapSort(int[] data)
        {
            //this line not in algorithms book, but necessary to avoid index out of range:
            if (data.Length == 0)
                return;

            Heap heap = new Heap(data);
            BuildMaxHeap(heap);

            for (int i = heap.data.Length - 1; i != 0; --i)
            {   //exchange values
                int tmp = heap.data[i];
                heap.data[i] = heap.data[0];
                heap.data[0] = tmp;

                heap.size -= 1;

                MaxHeapify(heap, 0);
            }
        }

        private class Heap
        {
            public Heap(int[] data)
            {
                this.data = data;
                size = data.Length;
            }

            public int[] data;
            public int size;
        }

        private static int HeapParent(int i)
        {
            //floor
            return (i >> 1) - 1;
        }

        private static int HeapLeft(int i)
        {
            //floor
            return (i << 1) + 1;
        }

        private static int HeapRight(int i)
        {
            //floor
            return (i << 1) + 2;
        }

        private static void BuildMaxHeap(Heap heap)
        {
            //floor
            for (int i = heap.data.Length / 2; i != -1; --i)
                MaxHeapify(heap, i);
        }

        private static void MaxHeapify(Heap heap, int i)
        {
            int l = HeapLeft(i);
            int r = HeapRight(i);
            int largest;

            if (l <= heap.size - 1 && heap.data[l] > heap.data[i])
                largest = l;
            else
                largest = i;

            if (r <= heap.size - 1 && heap.data[r] > heap.data[largest])
                largest = r;

            if (largest != i)
            {
                //exchange values
                int tmp = heap.data[i];
                heap.data[i] = heap.data[largest];
                heap.data[largest] = tmp;

                MaxHeapify(heap, largest);
            }
        }

        //Quick sort
        public static void QuickSort(int[] data)
        {
            QuickSort(data, 0, data.Length - 1);
        }

        private static void QuickSort(int[] data, int p, int r)
        {
            if (p < r)
            {
                int q = Partition(data, p, r);
                QuickSort(data, p, q - 1);
                QuickSort(data, q + 1, r);
            }
        }

        private static int Partition(int[] data, int p, int r)
        {
            int x = data[r];
            int i = p - 1;

            for (int j = p; j != r; ++j)
            {
                if (data[j] <= x)
                {
                    i += 1;
                    //exchange values
                    int tmp = data[i];
                    data[i] = data[j];
                    data[j] = tmp;
                }
            }

            //exchange values
            int tmp2 = data[i + 1];
            data[i + 1] = data[r];
            data[r] = tmp2;

            return i + 1;
        }

        //Randomized quicksort
        public static void RandomizedQuickSort(int[] data)
        {
            RandomizedQuickSort(data, 0, data.Length - 1);
        }

        private static void RandomizedQuickSort(int[] data, int p, int r)
        {
            Random rand = new Random();

            if (p < r)
            {
                int q = RandomizedPartition(data, p, r, rand);
                RandomizedQuickSort(data, p, q - 1);
                RandomizedQuickSort(data, q + 1, r);
            }
        }

        private static int RandomizedPartition(int[] data, int p, int r, Random rand)
        {
            int i = rand.Next(p, r + 1);
            //swap values
            int tmp = data[i];
            data[i] = data[r];
            data[r] = tmp;

            return Partition(data, p, r);
        }

        //CountingSort

        //this first method isn't really supposed to be part of counting sort, normally you provide the maximum value manually
        public static void CountingSort(int[] data)
        {
            CountingSort(data, data.Max());
        }

        public static void CountingSort(int[] data, int maxPossible)
        {
            int[] tmp = new int[data.Length];
            CountingSort(data, tmp, maxPossible);
            data = tmp;
        }

        public static void CountingSort(int[] data, int[] tmp, int maxPossible)
        {
            int[] counts = new int[maxPossible];

            //actually unnecessary in C# because everything gets 0 initialized anyway
            for (int i = 0; i != maxPossible; ++i)
                counts[i] = 0;

            //find number of elements equal to each value
            for (int i = 0; i != data.Length; ++i)
                counts[data[i]] += 1;

            //find the number of elements less than or equal to each value
            for (int i = 1; i != maxPossible; ++i)
                counts[i] = counts[i] + counts[i - 1];

            for (int i = data.Length - 1; i != -1; --i)
            {
                tmp[counts[data[i]]] = data[i];
                counts[data[i]] -= 1;
            }
        }
    }
}
