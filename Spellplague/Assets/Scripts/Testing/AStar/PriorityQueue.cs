using System;
using System.Collections;
using System.Collections.Generic;

namespace Spellplague.Utility
{
    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        private readonly List<T> binaryHeap;
        public int Count { get { return binaryHeap.Count; } }
        public bool IsEmpty { get { return binaryHeap == null || Count == 0; } }

        public T this[int i]
        {
            get
            {
                if (Count <= 0) return default;
                return binaryHeap[i];
            }
        }

        /// <summary>
        /// Initialize an empty priority queue.
        /// </summary>
        public PriorityQueue() => binaryHeap = new List<T>();

        /// <summary>
        /// Initialize the priority queue with a predetermined amount of memory.
        /// </summary>
        public PriorityQueue(int heapStartSize) => binaryHeap = new List<T>(heapStartSize);

        /// <summary>
        /// Initialize the priority queue with predetermined data (data gets min-heapified before applied).
        /// </summary>
        /// <param name="data"></param>
        public PriorityQueue(List<T> data)
        {
            MinHeapify(data);
            binaryHeap = data;
        }

        private int LastIndex { get { return Count - 1; } }
        private T LastItem
        {
            get { return binaryHeap[Count - 1]; }
            set { binaryHeap[Count - 1] = value; }
        }
        private T FirstItem
        {
            get { return binaryHeap[0]; }
            set { binaryHeap[0] = value; }
        }
        private enum Child
        {
            Left = 1,
            Right = 2
        }

        /// <summary>
        /// Push a new element and sort the heap.
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            if (item == null) { return; }

            binaryHeap.Add(item);
            int currentIndex = LastIndex; // Start from the bottom.
            int currentParentIndex = GetParentIndex(currentIndex);
            while (item.CompareTo(binaryHeap[currentParentIndex]) < 0)
            {
                T itemParent = binaryHeap[currentParentIndex];
                binaryHeap[currentParentIndex] = item;
                binaryHeap[currentIndex] = itemParent;
                currentIndex = currentParentIndex;
                currentParentIndex = GetParentIndex(currentIndex);
            }
        }

        /// <summary>
        /// Return and remove the first element, then sort the heap.
        /// </summary>
        /// <returns>T</returns>
        public T Pop()
        {
            if (Count == 0) { return default; }

            T itemToReturn;
            if (Count == 1)
            {
                itemToReturn = FirstItem;
                binaryHeap.RemoveAt(LastIndex);
                return itemToReturn;
            }
            else if (Count == 2)
            {
                RemoveFirst();
                return itemToReturn;
            }

            RemoveFirst();
            int currentIndex = 0;
            int leftChild = GetChildIndex(Child.Left, currentIndex);
            int rightChild = GetChildIndex(Child.Right, currentIndex);
            int currentChildIndex = GetChildDirection(leftChild, rightChild, binaryHeap);
            while (currentChildIndex <= LastIndex
                   && binaryHeap[currentIndex].CompareTo(binaryHeap[currentChildIndex]) > 0)
            {
                T currentItem = binaryHeap[currentIndex];
                T currentChildItem = binaryHeap[currentChildIndex];
                binaryHeap[currentIndex] = currentChildItem;
                binaryHeap[currentChildIndex] = currentItem;
                currentIndex = currentChildIndex;
                leftChild = GetChildIndex(Child.Left, currentIndex);
                rightChild = GetChildIndex(Child.Right, currentIndex);
                currentChildIndex = GetChildDirection(leftChild, rightChild, binaryHeap);
            }

            return itemToReturn;

            void RemoveFirst()
            {
                itemToReturn = FirstItem;
                FirstItem = LastItem;
                LastItem = FirstItem;
                binaryHeap.RemoveAt(LastIndex);
            }
        }

        /// <summary>
        /// Convert data to minimum-heap form (min-heapify).
        /// </summary>
        /// <param name="data"></param>
        public void MinHeapify(List<T> data)
        {
            int lastIndex = data.Count - 1;
            int currentParentIndex = GetParentIndex(lastIndex); // Start at the last parent.
            while (currentParentIndex >= 0)
            {
                while (GetChildIndex(Child.Left, currentParentIndex) <= lastIndex)
                {
                    int leftChild = GetChildIndex(Child.Left, currentParentIndex);
                    int rightChild = GetChildIndex(Child.Right, currentParentIndex);
                    int currentChildIndex = GetChildDirection(leftChild, rightChild, data);
                    if (data[currentParentIndex].CompareTo(data[currentChildIndex]) > 0)
                    {
                        T temp = data[currentParentIndex];
                        data[currentParentIndex] = data[currentChildIndex];
                        data[currentChildIndex] = temp;
                        currentParentIndex = currentChildIndex;
                        continue;
                    }

                    break;
                }

                currentParentIndex--; // Move on to the next parent.
            }
        }

        /// <summary>
        /// Return the first element.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return FirstItem;
        }

        private static int GetChildIndex(Child child, int index)
        {
            return index * 2 + (int)child;
        }

        private static int GetParentIndex(int index)
        {
            return (int)Math.Floor((float)index / 2);
        }

        private int GetChildDirection(int leftChild, int rightChild, List<T> data)
        {
            int lastIndex = data.Count - 1;
            if (rightChild <= lastIndex
                && data[rightChild].CompareTo(data[leftChild]) < 0)
            {
                return rightChild;
            }

            return leftChild;
        }

        #region Support iteration
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)binaryHeap).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)binaryHeap).GetEnumerator();
        }
        #endregion
    }
}