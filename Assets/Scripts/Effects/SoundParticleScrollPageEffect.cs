using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundParticleScrollPageEffect : MonoBehaviour
{
    TimedColorEffect colorEffect;
    ParticleSystem pSystem;

    [Range(0, 3.1f)] public float threshold = 2.5f;

    float bottomLimit;
    float topLimit;

    bool isPlaying = false;
    
    void Awake(){
        bottomLimit = -threshold;
        topLimit = threshold;

        colorEffect = this.GetComponent<TimedColorEffect>();
        colorEffect.AddMaterial(FileAsset.GetMaterialOfSoundParticleByColorName("Orange"));
        colorEffect.AddMaterial(FileAsset.GetMaterialOfSoundParticleByColorName("Cyan"));
        colorEffect.AddMaterial(FileAsset.GetMaterialOfSoundParticleByColorName("Purple"));

        pSystem = this.GetComponent<ParticleSystem>();
    }


    void Update()
    {
        Vector3 currentPosition = transform.position;
        if(bottomLimit <= currentPosition.y && currentPosition.y <= topLimit){
            if(!isPlaying){
                isPlaying = true;
                pSystem.Play();
            }
        }else{
            if(isPlaying){
                isPlaying = false;
                pSystem.Stop();
            }
        }
    }
}
