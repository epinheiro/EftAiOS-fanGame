using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public class ExtendedList<T> : List<T>{

    // Adapted from https://stackoverflow.com/a/1262619
    private System.Random rng = new System.Random();  
    public void SimpleShuffle()  
    {  
        int n = this.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = this[k];  
            this[k] = this[n];  
            this[n] = value;  
        }  
    }

    // Adapted from https://stackoverflow.com/a/1262619
    public void Shuffle()
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = this.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (System.Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = this[k];
            this[k] = this[n];
            this[n] = value;
        }
    }
    public void AddRedundant(T listEntry, int numberOfTimes){
        for( int i=0 ; i<numberOfTimes ; i++ ){
            this.Add(listEntry);
        }
    }
    public T PopValue(){
        if(this.Count == 0){
            return default(T);
        }else{
            int lastIndex = this.Count-1;
            T popValue = this[lastIndex];
            this.RemoveAt(lastIndex);
            return popValue;
        }
    }

}
