using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedColorEffect : MonoBehaviour
{
    [Range(0.1f, 10f)] public float interval = 1;
    public Material[] materials;
    int currentMaterial = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RotateThroughColors());
    }

    IEnumerator RotateThroughColors(){
        while(true){
            ParticleSystemRenderer psr = GetComponent<ParticleSystemRenderer>();

            currentMaterial = (currentMaterial + 1)%materials.Length;
            psr.material = materials[currentMaterial];

            yield return new WaitForSecondsRealtime(interval);

        }
    }
}
