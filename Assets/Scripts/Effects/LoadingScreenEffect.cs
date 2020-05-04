using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenEffect : MonoBehaviour
{
    string[] texts = new string[]{
        "loading",
        "loading.",
        "loading..",
        "loading..."
    };

    int currentText = 0;

    public float secondsToChange = 0.3f;

    Text loadingText;

    void Awake(){
        loadingText = transform.GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EffectCoroutine());
    }

    IEnumerator EffectCoroutine(){
        while(true){
            currentText = (currentText + 1) % texts.Length;
            loadingText.text = texts[currentText];
            yield return new WaitForSeconds(secondsToChange);
        }
    }
}
