using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

// James Williamson

// Basic minheap used for pathfinding. Sorts nodes by fvalue (though it can be used for anything else)

public class Heap<T> where T : IHeapItem<T> {
	T[] items;
	int currentItemCount;

	public Heap(int maxHeapSize){
		items = new T[maxHeapSize];
	}

	public bool Contains(T item){
		return Equals (items [item.HeapIndex], item);
	}

	public int Count{
		get{
			return currentItemCount;
		}
	}

	public void UpdateItem(T item){
		SortUp (item);
	}

	public void Add(T item){
		item.HeapIndex = currentItemCount;
		items [currentItemCount] = item;
		SortUp (item);
		currentItemCount++;
	}

	public T RemoveFirst(){
		T firstItem = items [0];
		currentItemCount--;
		items [0] = items [currentItemCount];
		items [0].HeapIndex = 0;
		SortDown (items[0]);
		return firstItem;
	}

	// For adding a value to the heap; must find its way up the tree
	void SortUp(T item){
		while (true) {
			int parentIndex  = (item.HeapIndex - 1) / 2;

			T parentItem = items [parentIndex];
			if (item.CompareTo (parentItem) > 0) {
				Swap (item, parentItem);
			} else {
				break;
			}
		}

	}

	// Readjusts the tree after the top node is removed
	void SortDown(T item){
		while (true) {
			int childIndexLeft = item.HeapIndex*2+1;
			int childIndexRight = item.HeapIndex*2+2;
			int swapIndex;

			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;

				if (childIndexRight < currentItemCount) {
					if (items [childIndexLeft].CompareTo (items [childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}
				if (item.CompareTo (items [swapIndex]) < 0) {
					Swap (item, items [swapIndex]);
				} else {
					return;
				}
			} else {
				return;
			}

		}
	}

	// swap function used when the tree is being reorganized after an addition
	void Swap(T itemA, T itemB){
		items [itemA.HeapIndex] = itemB;
		items [itemB.HeapIndex] = itemA;

		int indexA = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = indexA;
	}
}

// 
public interface IHeapItem<T> : IComparable<T>{
	int HeapIndex {
		get;
		set;
	}
}

// CompareTo, since it's been a while.
// a.CompareTo(b): 
//	a is higher priority, return 1;
//	a is lower priority, return -1;
//	a == b return 0;