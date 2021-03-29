using UnityEngine;
using System.Collections;
using System;

public class MinHeap<T> where T : IHeapItem<T> {
	// What is a heap (int heap): https://www.geeksforgeeks.org/heap-data-structure/
	// Wikipedia article: https://en.wikipedia.org/wiki/Heap_(data_structure)

	//? Variables
	T[] items;
	int currentItemCount;

	//? Properties
	public int Count { get => currentItemCount; }

	//? Constructor
	public MinHeap(int maxHeapSize) {
		items = new T[maxHeapSize];
	}

	//? Methods
	public bool Add(T item) {
		if (items.Length == currentItemCount) return false; // Heap is full

		item.HeapIndex = currentItemCount;
		items[currentItemCount] = item;
		SortUp(item);
		currentItemCount++;

		return true;
	} // Adds the item to the end and sorts it up

	public T RemoveFirst() {
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	} // Removes the first item and sorts the heap's last item down

	public void UpdateItem(T item) {
		SortUp(item);
	} // Re-sorts an existing item up

	public bool Contains(T item) {
		return Equals(items[item.HeapIndex], item);
	} // Checks if the heap contains an item

	private void SortDown(T item) {
		while (true) {
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;

				if (childIndexRight < currentItemCount) {
					//if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
					//	swapIndex = childIndexRight;
					//}
					swapIndex = (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) ? childIndexRight : childIndexLeft;
				}

				if (item.CompareTo(items[swapIndex]) < 0) {
					Swap(item, items[swapIndex]);
				} else return;

			} else return;
		}
	} // Sorts an item up the heap "tree", untill its bigger than its children

	private void SortUp(T item) {
		int parentIndex = (item.HeapIndex - 1) / 2;

		while (true) {
			T parentItem = items[parentIndex];
			if (item.CompareTo(parentItem) > 0) {
				Swap(item, parentItem);
			} else {
				break;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	} // Sorts an item up the heap "tree", untill its smaller than its children

	private void Swap(T itemA, T itemB) {
		// Swap the items
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;
		// Swap the index variables
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	} // Swaps two items
}

public interface IHeapItem<T> : IComparable<T> {
	int HeapIndex {
		get;
		set;
	}
}