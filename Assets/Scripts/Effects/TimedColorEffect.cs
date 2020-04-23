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
        if(materials.Length == 0){
            ChangeParticleSystemMaterial(GetMaterialByColorName("White"));
        }else{
            StartCoroutine(RotateThroughColors());
        }
    }

    IEnumerator RotateThroughColors(){
        while(true){
            currentMaterial = (currentMaterial + 1)%materials.Length;
            ChangeParticleSystemMaterial(materials[currentMaterial]);

            yield return new WaitForSecondsRealtime(interval);
        }
    }

    Material GetMaterialByColorName(string colorName){
        return (Material) Resources.Load<Material>(string.Format("Materials/SoundParticle{0}", colorName));
    }

    void ChangeParticleSystemMaterial(Material material){
        ParticleSystemRenderer psr = GetComponent<ParticleSystemRenderer>();
        psr.material = material;
    }
}
