using System;
using System.Collections.Generic;
using System.Text;

namespace PAALab3
{
    public class HeapNode<T>
    {
        public int Key { get; set; }
        public T Value { get; set; }

        public HeapNode(int key, T value) { Key = key; this.Value = value; }
    }

    public class Heap<T>
    {
        private HeapNode<T>[] array;
        private int count;

        public Heap(int size)
        {
            Array = new HeapNode<T>[size];
            count = 0;
        }

        public void Insert(int key, T value)
        {
            if (count >= array.Length)
                return;
            HeapNode<T> node = new HeapNode<T>(key, value);
            array[count] = node;
            BoubleUp(count);
            count++;
        }

        public void DecreaseKey(int position, int value)
        {
            if (position >= count || array[position].Key <= value)
                return;
            array[position].Key = value;
            BoubleUp(position);
        }

        public void DecreaseKey(T value, int newKey)
        {
            int position = FindNode(value);
            if (position >= 0)
                DecreaseKey(position, newKey);
        }

        public int FindNode(T value)
        {
            int i = 0;
            while (i < Count && !Array[i].Value.Equals(value))
                i++;
            if (Array[i].Value.Equals(value))
                return i;
            else return -1;
        }

        private void BoubleUp(int position)
        {
            int parent = Parent(position);
            int ptr = position;
            while (parent >= 0 && array[parent].Key > array[ptr].Key)
            {
                Swap(parent, ptr);
                ptr = parent;
                parent = Parent(parent);
            }
        }
        private void Swap(int parent, int ptr)
        {
            HeapNode<T> tmp = array[parent];
            array[parent] = array[ptr];
            array[ptr] = tmp;
        }

        public void DeleteNode(int position)
        {
            DecreaseKey(position, int.MinValue);
            ExtractMinimum();
        }

        public HeapNode<T> ExtractMinimum()
        {
            HeapNode<T> ret = array[0];
            count--;
            array[0] = array[count];
            MinHeapify(0);
            return ret;
        }

        public void MinHeapify(int position)
        {
            int l = LeftChild(position);
            int r = RightChild(position);
            int min = position;
            if (l < count && array[l].Key < array[min].Key)
                min = l;
            if (r < count && array[r].Key < array[min].Key)
                min = r;
            if (min != position)
            {
                Swap(position, min);
                MinHeapify(min);
            }
        }

        HeapNode<T> GetMin()
        {
            return array[0];
        }

        public void PrintArray()
        {
            for (int i = 0; i < count; i++)
            {
                Console.Write(array[i].Key + " ");
            }
            Console.WriteLine();
        }
        int Parent(int i)
        {
            return (i - 1) / 2;
        }

        int LeftChild(int i)
        {
            return 2 * i + 1;
        }

        int RightChild(int i)
        {
            return 2 * i + 2;
        }

        public int Count
        {
            get
            {
                return count;
            }

            set
            {
                count = value;
            }
        }

        internal HeapNode<T>[] Array
        {
            get
            {
                return array;
            }

            set
            {
                array = value;
            }
        }
    }
}
