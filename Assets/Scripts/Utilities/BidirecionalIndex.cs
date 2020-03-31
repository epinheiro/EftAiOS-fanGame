using System.Collections.Generic;
using System;

public class BidirecionalIndex<T1, T2>
{
    Dictionary<T1, T2> a;
    Dictionary<T2, T1> b;

    public int Count{
        get{
            return a.Count;
        }
    }

    public BidirecionalIndex(){
        a = new Dictionary<T1, T2>();
        b = new Dictionary<T2, T1>();
    }

    public void Add(T1 key, T2 value){
        if(ContainsKey(key))   throw new ArgumentException(string.Format("Index already has key {0}", key));
        if(ContainsValue(value)) throw new ArgumentException(string.Format("Index already has value {0}", value));

        a.Add(key, value);
        b.Add(value, key);
    }

    public void RemoveKey(T1 key){
        if(!ContainsKey(key))   throw new ArgumentException(string.Format("Index does not have key {0}", key));

        T2 value;
        a.TryGetValue(key, out value);

        a.Remove(key);
        b.Remove(value);
    }

    public void RemoveValue(T2 value){
        if(ContainsValue(value)) throw new ArgumentException(string.Format("Index does not have value {0}", value));

        T1 key;
        b.TryGetValue(value, out key);

        a.Remove(key);
        b.Remove(value);
    }

    public bool ContainsKey(T1 key){
        return a.ContainsKey(key);
    }

    public bool ContainsValue(T2 value){
        return b.ContainsKey(value);
    }

    public T2 GetByKey(T1 key){
        if(a.ContainsKey(key)){
            T2 correspondent;
            a.TryGetValue(key, out correspondent);
            return correspondent;
        }else{
            throw new KeyNotFoundException(string.Format("Index does not have {0} key", key));
        }
    }

    public T1 GetByValue(T2 value){
        if(b.ContainsKey(value)){
            T1 correspondent;
            b.TryGetValue(value, out correspondent);
            return correspondent;
        }else{
            throw new KeyNotFoundException(string.Format("Index does not have {0} value", value));
        }
    }
}
