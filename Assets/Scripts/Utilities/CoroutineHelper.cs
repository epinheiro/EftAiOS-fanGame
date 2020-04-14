using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineHelper
{
    public delegate void BaseAction();

    static public Coroutine DelayedCall(MonoBehaviour owner, BaseAction function, float delay, bool repeat = false){
        return owner.StartCoroutine(DelayedCallCoroutine(function, delay, repeat));
    }

    static IEnumerator DelayedCallCoroutine(BaseAction function, float delay, bool repeat){
        do{
            yield return new WaitForSeconds(delay);
            function();
        }while(repeat);
    }
}
