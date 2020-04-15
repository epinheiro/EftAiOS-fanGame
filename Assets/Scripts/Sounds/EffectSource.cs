using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EffectSource : AudioSourceWrapper{
    public EffectSource(GameObject sourceOwner, float volume = 0.5f) : base(sourceOwner, volume, volume){
    }
}
