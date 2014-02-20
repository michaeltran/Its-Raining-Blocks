/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadList<T> {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    T[] arr;
    int size;

    // ===========================================================
    // Constructors
    // ===========================================================
    
    public MadList() : this(32) {
    }
    
    public MadList(int capacity) {
        arr = new T[capacity];
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    public T[] Array {
        get { return arr; }
        set { arr = value; }
    }
    
    public int Count {
        get { return size; }
    }

    public void Add(T e) {
        EnsureCapacity(size + 1);
        arr[size] = e;
        size++;
    }
    
    public T this[int index] {
        get {
            CheckRange(index);
            return arr[index];
        }
        
        set {
            CheckRange(index);
            arr[index] = value;
        }
    }
    
    void CheckRange(int index) {
        if (index >= size) {
            throw new IndexOutOfRangeException("index " + index + " out of range (size = " + size + ")");
        }
    }
    
    public void Clear() {
        size = 0;
    }
    
    public void Trim() {
        if (size != arr.Length) {
            System.Array.Resize(ref arr, size);
        }
    }
    
    void EnsureCapacity(int targetSize) {
        if (arr.Length < targetSize) {
            System.Array.Resize(ref arr, Mathf.Min(targetSize * 2, 1024 * 1024));
        }
    }
    

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif